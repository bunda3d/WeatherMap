using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WeatherMap.Core.DTOs;

namespace WeatherMap.Core
{
	public class WeatherService
	{
		private LogWebRequestsService _logWebRequestsService;
		private const string BaseUrl = "https://api.weather.gov";
		private const string UserAgent = "WeatherMap.com, contact@weathermap.com";

		public WeatherService()
		{
			_logWebRequestsService = new LogWebRequestsService();
		}

		public async Task<(WeatherForecastsDTO.Rootobject, WeatherForecastsDTO.Rootobject, WeatherOfficeDTO.Rootobject)> GetForecastsAsync(double latitude, double longitude)
		{
			//method for getting forecasts, hourly forecasts, and location/office data
			var (office, gridX, gridY, officeData) = await CallLocalWeatherOfficeAPI(latitude, longitude);
			var futureForecasts = await CallForecastAPI(office, gridX, gridY);
			var hourlyForecasts = await CallForecastAPI(office, gridX, gridY, "/hourly");

			return (futureForecasts, hourlyForecasts, officeData);
		}

		public async Task<(string office, int gridX, int gridY, WeatherOfficeDTO.Rootobject locData)> CallLocalWeatherOfficeAPI(double latitude, double longitude)
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("User-Agent", $"({UserAgent})"); //required by API

				var locUrl = $"{BaseUrl}/points/{latitude},{longitude}";
				var locUrlResponse = await client.GetAsync(locUrl);
				var locUrlContent = await locUrlResponse.Content.ReadAsStringAsync();

				await _logWebRequestsService.LogRequestAsync(locUrl, locUrlResponse, locUrlContent, UserAgent);

				var locData = JsonConvert.DeserializeObject<WeatherOfficeDTO.Rootobject>(locUrlContent);

				return (locData.properties.gridId, locData.properties.gridX, locData.properties.gridY, locData);
			}
		}

		public async Task<WeatherForecastsDTO.Rootobject> CallForecastAPI(string office, int gridX, int gridY, string hrly = "")
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("User-Agent", $"({UserAgent})"); //required by API

				var forecastUrl = $"{BaseUrl}/gridpoints/{office}/{gridX},{gridY}/forecast{hrly}";
				var forecastResponse = await client.GetAsync(forecastUrl);
				var forecastContent = await forecastResponse.Content.ReadAsStringAsync();

				await _logWebRequestsService.LogRequestAsync(forecastUrl, forecastResponse, forecastContent, UserAgent);

				var forecasts = JsonConvert.DeserializeObject<WeatherForecastsDTO.Rootobject>(forecastContent);

				return forecasts;
			}
		}
	}
}