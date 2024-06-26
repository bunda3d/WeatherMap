﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherMap.Data;
using WeatherMap.Data.Exceptions;
using WeatherMap.Data.Repositories;

namespace WeatherMap.Core
{
	public class LogWebRequestsService
	{
		private LogWebRequestsRepository _repo;
		private LogExceptionsService _logExService;

		public LogWebRequestsService()
		{
			_repo = new LogWebRequestsRepository();
			_logExService = new LogExceptionsService();
		}

		public async Task LogRequestAsync(
			string url,
			HttpResponseMessage response,
			string requestBody,
			string userAgent
		)
		{
			//map data to <LogsWebRequest> model from params/objects
			try
			{
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

				bool isLogged = await _repo.LogRequestAsync(logEntry);
				if (!isLogged)
				{
					throw new Exception("Failed to log web request. ");
				}
			}
			catch (DataAccessException ex)
			{
				await _logExService.LogExceptionAsync(ex.InnerException);
			}
			catch (Exception ex)
			{
				await _logExService.LogExceptionAsync(ex);
			}
		}
	}
}