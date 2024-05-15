using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WeatherMap.Data.Exceptions;

namespace WeatherMap.Data.Repositories
{
	public class AppSettingsRepository
	{
		private readonly WeatherMap_DEV _context;

		public AppSettingsRepository()
		{
			_context = new WeatherMap_DEV();
		}

		//[AppSettings].[Name] and [Code] are like "Key" & "Value" pairs from the AppSetting config file

		public async Task<IEnumerable<AppSetting>> GetAllAppSettingsAsync()
		{
			try
			{
				var appSettings = await _context.AppSettings
					.ToListAsync();

				return appSettings;
			}
			catch (Exception ex)
			{
				throw new DataAccessException("An error occurred while getting all app setting records. ", ex);
			}
		}

		public async Task<AppSetting> GetAppSettingByNameAsync(string settingName)
		{
			try
			{
				var appSetting = await _context.AppSettings
					.FirstOrDefaultAsync(s => s.Name == settingName);

				return appSetting;
			}
			catch (Exception ex)
			{
				throw new DataAccessException("An error occurred while getting the app setting record by name. ", ex);
			}
		}

		public async Task<string> GetAppSettingCodeByNameAsync(string settingName)
		{
			try
			{
				string appSetting = await _context.AppSettings
					.Where(s => s.Name == settingName)
					.Select(s => s.Code)
					.FirstOrDefaultAsync();

				return appSetting;
			}
			catch (Exception ex)
			{
				throw new DataAccessException("An error occurred while getting the app setting by name. ", ex);
			}
		}
	}
}