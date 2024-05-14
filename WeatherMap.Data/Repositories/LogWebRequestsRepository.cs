using System;
using System.Threading.Tasks;

namespace WeatherMap.Data.Repositories
{
	public class LogWebRequestsRepository
	{
		private readonly WeatherMap_DEV _context;

		public LogWebRequestsRepository()
		{
			_context = new WeatherMap_DEV();
		}

		// CRUD web Requests/Responses "LogsWebRequests" table using entity framework

		public async Task LogRequestAsync(LogsWebRequest logEntry)
		{
			try
			{
				_context.LogsWebRequests.Add(logEntry);
				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{
				//LogExceptionAsync(ex);
			}
		}
	}
}