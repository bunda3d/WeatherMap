using System.Threading.Tasks;
using WeatherMap.Data.Repositories;
using WeatherMap.Data;
using System.Net.Http;
using System.Security.Policy;
using System;

namespace WeatherMap.Core
{
	public class LogWebRequestsService
	{
		private LogWebRequestsRepository _repo;

		public LogWebRequestsService()
		{
			_repo = new LogWebRequestsRepository();
		}

		public async Task LogRequestAsync(string url, HttpResponseMessage response, string requestBody, string userAgent)
		{
			//map data to <LogsWebRequest> model from params/objects

			var logEntry = new LogsWebRequest
			{
				RequestURL = url,
				ResponseCode = (int?)response.StatusCode,
				RequestMethod = response.RequestMessage.Method.Method,
				Body = requestBody,
				UserAgent = userAgent,
				CreatedDate = DateTime.UtcNow,
				CreatedBy = "WebClient"
				// set other properties as needed
			};

			await _repo.LogRequestAsync(logEntry);
		}
	}
}