using System;
using System.Threading.Tasks;
using WeatherMap.Data;
using WeatherMap.Data.Repositories;

namespace WeatherMap.Core
{
	public class LogExceptionsService
	{
		private LogExceptionsRepository _repo;

		public LogExceptionsService()
		{
			_repo = new LogExceptionsRepository();
		}

		public async Task LogExceptionAsync(Exception ex)
		{
			//map data to <LogsException> model from Exception object
			try
			{
				var logEntry = new LogsException
				{
					Name = null,          //optional exception name
					Code = null,          //optional error code tracking
					Type = null,          //optional exception type
					Description = null,   //optional exception description
					Message = ex.Message,
					StackTrace = ex.StackTrace,
					Source = ex.Source,
					TargetSite = ex.TargetSite?.Name,
					ExceptionType = ex.GetType().FullName,
					InnerException = ex.InnerException?.Message,
					Version = null,       //optional app vsn
					CreatedDate = DateTime.UtcNow,
					CreatedBy = "WebClient",
					UpdatedBy = null,     //if details or error resolution codes are added
					UpdatedDate = null
				};

				await _repo.LogExceptionAsync(logEntry);
			}
			catch (Exception e)
			{
				//since this method records exceptions, if it throws one, it may not be logged unless:
				//consider logging the exception to a file or other logging mechanism
				//handle the exception, e.g., return an error message to the caller

				//for now, will just use this for when debugging:
				System.Diagnostics.Debug.WriteLine("Exception caught: " + e.Message);
				System.Diagnostics.Debug.WriteLine("Stack Trace: " + e.StackTrace);
			}
		}
	}
}