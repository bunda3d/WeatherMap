using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WeatherMap.Core;
using WeatherMap.Core.DTOs;
using WeatherMap.UI.Models;

namespace WeatherMap.UI.Controllers
{
	public class HomeController : Controller
	{
		private readonly WeatherService _weatherService;
		private readonly LogExceptionsService _logExService;

		public HomeController()
		{
			_weatherService = new WeatherService();
			_logExService = new LogExceptionsService();
		}

		public ActionResult Index()
		{
			var viewModel = new CurrentConditionsVM(); //initially empty VM until map is clicked

			return View(viewModel);
		}

		[HttpPost]
		public async Task<JsonResult> GetWeatherDataAsync(double latitude, double longitude)
		{
			try
			{
				var (office, gridX, gridY, officeData) = await _weatherService.CallLocalWeatherOfficeAPI(latitude, longitude);
				var hourlyForecasts = await _weatherService.CallForecastAPI(office, gridX, gridY, "/hourly");

				var viewModel = await MapDataToViewModel(hourlyForecasts, officeData);
				string htmlContent = await RenderPartialViewToString("_CurrentConditions", viewModel);

				//return status msg to user, updated view data
				return Json(new
				{
					RenderedPartialViewHtml = htmlContent,
					Status = "OK",
					Message = $"You selected {officeData.properties.relativeLocation.properties.city}, {officeData.properties.relativeLocation.properties.state}."
				});
			}
			catch (Exception ex)
			{
				await _logExService.LogExceptionAsync(ex);

				return Json(new
				{
					Status = "FAIL",
					Message = "You selected a map area that produces no weather data, please select somewhere else in the USA."
				});
			}
		}

		public async Task<CurrentConditionsVM> MapDataToViewModel(WeatherForecastsDTO.Rootobject hourlyForecasts, WeatherOfficeDTO.Rootobject officeData)
		{
			try
			{
				//map properties from DTOs to ViewModels
				var officeProps = officeData.properties;
				var currentProps = hourlyForecasts.properties;
				var hrlyPeriods = hourlyForecasts.properties.periods;
				var currentPeriod = hrlyPeriods.FirstOrDefault(period => period.number == 1);

				//CurrentConditionsVM needs only 1 period, ExtendedForecastVM needs a List<ForecastPeriodsVM>
				var forecastPeriodVM = new ForecastPeriodVM
				{
					Number = currentPeriod.number,
					Name = currentPeriod.name,
					LastUpdate = currentProps.updated,
					TemperatureF = $"{currentPeriod.temperature}°F",
					TemperatureC = $"{Math.Round((currentPeriod.temperature - 32) / 1.8, 0)}°C",
					ProbabilityOfPrecipitation = $"{currentPeriod.probabilityOfPrecipitation.value}%",
					Dewpoint = $"{Math.Round(currentPeriod.dewpoint.value * 1.8 + 32, 0)}°F", //convert C to F
					Humidity = $"{currentPeriod.relativeHumidity.value}%",
					WindSpeed = currentPeriod.windSpeed,
					WindDirection = currentPeriod.windDirection,
					Icon = currentPeriod.icon.Replace("size=small", "size=large"),
					ShortForecast = currentPeriod.shortForecast,
					DetailedForecast = currentPeriod.detailedForecast
				};

				var viewModel = new CurrentConditionsVM
				{
					GridID = officeProps.gridId,
					GridX = $"{officeProps.gridX}",
					GridY = $"{officeProps.gridY}",
					LocationID = officeData.id,
					City = officeProps.relativeLocation.properties.city,
					State = officeProps.relativeLocation.properties.state,
					TimeZone = officeProps.timeZone,
					ForecastOffice = officeProps.cwa,
					ForecastOfficeLink = officeProps.forecastOffice,
					RadarStation = officeProps.radarStation,
					Latitude = $"{Math.Round(officeData.geometry.coordinates[1], 3)}",
					Longitude = $"{Math.Round(officeData.geometry.coordinates[0], 3)}",
					Elevation = $"{Math.Round(currentProps.elevation.value * 3.281, 0)}ft", //m to ft

					ForecastPeriod = forecastPeriodVM,
				};

				return viewModel;

				//var futurePeriods = futureForecasts.properties.periods;
				//var listFuturePeriods = futurePeriods.Select(period => new ForecastPeriodVM
				//{
				//	WindSpeed = period.WindSpeed,
				//	//etc.
				//}.ToList());

				////way to get all futureForecast periods
				//var periodViewModels = periods.Select(period => new ForecastPeriodVM
				//{
				//	Name = period.name,
				//	Temperature = period.temperature.ToString(),
				//	WindSpeed = period.windSpeed,
				//	ShortForecast = period.shortForecast,
				//	// Map other properties as needed
				//}).ToList();
			}
			catch (Exception ex)
			{
				await _logExService.LogExceptionAsync(ex);
				throw;
			}
		}

		private async Task<string> RenderPartialViewToString(string viewName, object model)
		{
			try
			{
				//render partial view to string to use in JS for UI
				ViewData.Model = model;
				using (StringWriter sw = new StringWriter())
				{
					ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
					ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
					viewResult.View.Render(viewContext, sw);
					viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
					return sw.GetStringBuilder().ToString();
				}
			}
			catch (Exception ex)
			{
				await _logExService.LogExceptionAsync(ex);
				throw;
			}
		}

		public ActionResult About()
		{
			ViewBag.Message = "WeatherMap provides your local weather forecasts with the convenience of Google Maps.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Kris Bunda, Founder";

			return View();
		}
	}
}