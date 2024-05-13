using System;

namespace WeatherMap.UI.Models
{
	public class ForecastPeriodVM
	{
		public int Number { get; set; }
		public string Name { get; set; }
		public DateTime LastUpdate { get; set; }
		public string TemperatureF { get; set; }
		public string TemperatureC { get; set; }
		public string ProbabilityOfPrecipitation { get; set; }
		public string Dewpoint { get; set; }
		public string Humidity { get; set; }
		public string WindSpeed { get; set; }
		public string WindDirection { get; set; }
		public string Icon { get; set; }
		public string ShortForecast { get; set; }
		public string DetailedForecast { get; set; }
	}
}