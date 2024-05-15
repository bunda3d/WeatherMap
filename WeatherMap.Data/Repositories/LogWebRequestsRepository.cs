using System;
using System.Threading.Tasks;
using WeatherMap.Data.Exceptions;

namespace WeatherMap.Data.Repositories
{
	public class LogWebRequestsRepository
	{
		private readonly WeatherMap_DEV _context;

		public LogWebRequestsRepository()
		{
			_context = new WeatherMap_DEV();
		}

		// CRUD web Requests/Responses to "LogsWebRequests" table using entity framework

		public async Task<bool> LogRequestAsync(LogsWebRequest logEntry)
		{
			try
			{
				_context.LogsWebRequests.Add(logEntry);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				//see comment in DataAccessException class for why this is used
				throw new DataAccessException("An error occurred while logging web request. ", ex);
			}
		}
	}
}