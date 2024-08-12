using System;
using System.Collections.Generic;
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
			//initially VM are empty until map is clicked
			var allWeatherViewModel = new AllWeatherVM
			{
				CurrentConditions = new CurrentConditionsVM(),
				ExtendedForecast = new ExtendedForecastVM()
			};

			return View(allWeatherViewModel);
		}

		[HttpPost]
		public async Task<JsonResult> GetWeatherDataAsync(double latitude, double longitude)
		{
			try
			{
				// for office & location data
				var (office, gridX, gridY, officeData) = await _weatherService.CallLocalWeatherOfficeAPI(latitude, longitude);

				// for hourly forecasts...
				var hourlyForecasts = await _weatherService.CallForecastAPI(office, gridX, gridY, "/hourly");
				var hourlyViewModel = await MapDataToViewModel(hourlyForecasts, officeData);
				string hourlyHtmlContent = await RenderPartialViewToString("_CurrentConditions", hourlyViewModel);

				// for extended forecasts...
				var extendedForecasts = await _weatherService.CallForecastAPI(office, gridX, gridY, "");
				var extendedViewModel = await MapDataToExtendedViewModel(extendedForecasts, officeData);
				string extendedHtmlContent = await RenderPartialViewToString("_ExtendedForecast", extendedViewModel);

				//return status msg to user, updated views data
				return Json(new
				{
					HourlyViewHtml = hourlyHtmlContent,
					ExtendedViewHtml = extendedHtmlContent,
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
				//check response
				if (hourlyForecasts.type.Contains("problem"))
				{
					//msg logs to db, UI shows generic error alert
					throw new Exception("'Unexpected Error' response from weather API. Probably from choosing same location multiple times OR too many locations in a short timeframe. ");
				}

				//map properties from DTOs to ViewModels for Office Locations & Hourly Forecasts
				var officeProps = officeData.properties;
				var currentProps = hourlyForecasts.properties;
				var hrlyPeriods = hourlyForecasts.properties.periods;
				var currentPeriod = hrlyPeriods.FirstOrDefault(period => period.number == 1);

				//CurrentConditionsVM needs only 1 period, ExtendedForecastVM needs List<ForecastPeriodsVM>
				var forecastPeriodVM = new ForecastPeriodVM
				{
					Number = currentPeriod.number,
					Name = currentPeriod.name,
					LastUpdate = currentProps.updated,
					TemperatureF = $"{currentPeriod.temperature}°F",
					TemperatureC = $"{Math.Round((currentPeriod.temperature - 32) / 1.8, 0)}°C",
					ProbabilityOfPrecipitation = $"{currentPeriod.probabilityOfPrecipitation?.value ?? 0}%",
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
			}
			catch (Exception ex)
			{
				await _logExService.LogExceptionAsync(ex);
				throw;
			}
		}

		public async Task<ExtendedForecastVM> MapDataToExtendedViewModel(WeatherForecastsDTO.Rootobject extendedForecasts, WeatherOfficeDTO.Rootobject officeData)
		{
			try
			{
				//check response
				if (extendedForecasts.type.Contains("problem"))
				{
					//msg logs to db, UI shows generic error alert
					throw new Exception("'Unexpected Error' response from weather API. Probably from choosing same location multiple times OR too many locations in a short timeframe. ");
				}

				//map properties from DTOs to ViewModels for Office Locations & EXTENDED Forecasts
				var officeProps = officeData.properties;
				var extendedProps = extendedForecasts.properties;
				var extendPeriods = extendedForecasts.properties.periods;

				var forecastPeriodVMs = new List<ForecastPeriodVM>();

				foreach (var period in extendPeriods)
				{
					var forecastPeriodVM = new ForecastPeriodVM
					{
						Number = period.number,
						Name = period.name,
						IsDaytime = period.isDaytime,
						LastUpdate = extendedProps.updated,
						TemperatureF = $"{period.temperature}°F",
						TemperatureC = $"{Math.Round((period.temperature - 32) / 1.8, 0)}°C",
						ProbabilityOfPrecipitation = $"{period.probabilityOfPrecipitation?.value ?? 0}%",
						Dewpoint = period.dewpoint?.value != null ? $"{Math.Round(period.dewpoint.value * 1.8 + 32, 0)}°F" : "N/A", //convert C to F
						Humidity = period.relativeHumidity?.value != null ? $"{period.relativeHumidity.value}%" : "N/A",
						WindSpeed = period.windSpeed,
						WindDirection = period.windDirection,
						Icon = period.icon.Replace("size=small", "size=large"),
						ShortForecast = period.shortForecast,
						DetailedForecast = period.detailedForecast
					};

					forecastPeriodVMs.Add(forecastPeriodVM);
				}

				var extendedForecastVM = new ExtendedForecastVM
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
					Elevation = $"{Math.Round(extendedProps.elevation.value * 3.281, 0)}ft", //m to ft

					ForecastPeriods = forecastPeriodVMs,
				};

				return extendedForecastVM;
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
				//render partial view to string for the UI
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