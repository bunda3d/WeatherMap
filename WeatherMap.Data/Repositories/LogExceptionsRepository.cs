using System;
using System.Threading.Tasks;

namespace WeatherMap.Data.Repositories
{
	public class LogExceptionsRepository
	{
		private WeatherMap_DEV _context;

		public LogExceptionsRepository()
		{
			_context = new WeatherMap_DEV();
		}

		// CRUD exceptions to "LogsExceptions" table using entity framework

		public async Task LogExceptionAsync(LogsException logEntry)
		{
			try
			{
				_context.LogsExceptions.Add(logEntry);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				//since this method records exceptions, if it throws one, it may not be logged unless:
				//consider logging the exception to a file or other logging mechanism
				//handle the exception, e.g., return an error message to the caller

				//for now, will just use this for when debugging:
				System.Diagnostics.Debug.WriteLine("Exception caught: " + ex.Message);
				System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
			}
		}
	}
}