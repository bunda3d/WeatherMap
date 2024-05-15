using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WeatherMap.Core.DTOs;
using System;
using System.Runtime.Caching;
using WeatherMap.Data.Repositories;

namespace WeatherMap.Core
{
	public class WeatherService
	{
		private LogWebRequestsService _logWebRequestsService;
		private LogExceptionsService _logExService;
		private AppSettingsRepository _settingsRepo;
		private static readonly ObjectCache Cache = MemoryCache.Default;
		private const string BaseUrlCacheKey = "apiBaseUrl_NWSforecasts";
		private const string UserAgentCacheKey = "userAgent_NWS";
		//private const string UserAgent = "WeatherMap.com, contact@weathermap.com";
		//private const string BaseUrl = "https://api.weather.gov";

		public WeatherService()
		{
			_logWebRequestsService = new LogWebRequestsService();
			_logExService = new LogExceptionsService();
			_settingsRepo = new AppSettingsRepository();
		}

		public async Task<(WeatherForecastsDTO.Rootobject, WeatherForecastsDTO.Rootobject, WeatherOfficeDTO.Rootobject)> GetForecastsAsync(double latitude, double longitude)
		{
			try
			{
				//method for getting forecasts, hourly forecasts, and location/office data
				var (office, gridX, gridY, officeData) = await CallLocalWeatherOfficeAPI(latitude, longitude);
				var futureForecasts = await CallForecastAPI(office, gridX, gridY);
				var hourlyForecasts = await CallForecastAPI(office, gridX, gridY, "/hourly");

				return (futureForecasts, hourlyForecasts, officeData);
			}
			catch (Exception ex)
			{
				await _logExService.LogExceptionAsync(ex);
				throw; //rethrow ex after logging
			}
		}

		public async Task<(string office, int gridX, int gridY, WeatherOfficeDTO.Rootobject locData)> CallLocalWeatherOfficeAPI(double latitude, double longitude)
		{
			try
			{
				using (var client = new HttpClient())
				{
					var userAgent = await GetCachedAppSettingAsync(UserAgentCacheKey);
					client.DefaultRequestHeaders.Add("User-Agent", $"({userAgent})"); //required by API

					var baseUrl = await GetCachedAppSettingAsync(BaseUrlCacheKey);
					var locUrl = $"{baseUrl}/points/{latitude},{longitude}";
					var locUrlResponse = await client.GetAsync(locUrl);
					var locUrlContent = await locUrlResponse.Content.ReadAsStringAsync();

					await _logWebRequestsService.LogRequestAsync(locUrl, locUrlResponse, locUrlContent, userAgent);

					var locData = JsonConvert.DeserializeObject<WeatherOfficeDTO.Rootobject>(locUrlContent);

					return (locData.properties.gridId, locData.properties.gridX, locData.properties.gridY, locData);
				}
			}
			catch (Exception ex)
			{
				await _logExService.LogExceptionAsync(ex);
				throw; //rethrow ex after logging
			}
		}

		public async Task<WeatherForecastsDTO.Rootobject> CallForecastAPI(string office, int gridX, int gridY, string hrly = "")
		{
			try
			{
				using (var client = new HttpClient())
				{
					var userAgent = await GetCachedAppSettingAsync(UserAgentCacheKey);
					client.DefaultRequestHeaders.Add("User-Agent", $"({userAgent})"); //required by API

					var baseUrl = await GetCachedAppSettingAsync(BaseUrlCacheKey);
					var forecastUrl = $"{baseUrl}/gridpoints/{office}/{gridX},{gridY}/forecast{hrly}";
					var forecastResponse = await client.GetAsync(forecastUrl);
					var forecastContent = await forecastResponse.Content.ReadAsStringAsync();

					await _logWebRequestsService.LogRequestAsync(forecastUrl, forecastResponse, forecastContent, userAgent);

					var forecasts = JsonConvert.DeserializeObject<WeatherForecastsDTO.Rootobject>(forecastContent);

					return forecasts;
				}
			}
			catch (Exception ex)
			{
				await _logExService.LogExceptionAsync(ex);
				throw; //rethrow ex after logging
			}
		}

		public async Task<string> GetCachedAppSettingAsync(string cacheKey)
		{
			//caching so service does not query value every cycle
			try
			{
				var baseUrl = Cache[cacheKey] as string; //check cache for value
				if (baseUrl == null)
				{
					//if not in cache, retrieve from db and add it
					baseUrl = await GetAppSettingFromDatabaseAsync(cacheKey);
					CacheItemPolicy policy = new CacheItemPolicy
					{
						AbsoluteExpiration = DateTimeOffset.Now.AddHours(24) 
					};
					Cache.Set(cacheKey, baseUrl, policy);
				}
				return baseUrl;
			}
			catch (Exception ex)
			{
				await _logExService.LogExceptionAsync(ex);
				return null;
			}
		}

		public async Task<string> GetAppSettingFromDatabaseAsync(string keyName)
		{
			try
			{
				string baseUrl = await _settingsRepo.GetAppSettingCodeByNameAsync(keyName);
				return baseUrl;
			}
			catch (Exception ex)
			{
				await _logExService.LogExceptionAsync(ex);
				return null;
			}
		}
	}
}