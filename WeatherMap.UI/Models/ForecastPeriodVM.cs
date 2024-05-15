using System;
using System.ComponentModel.DataAnnotations;

namespace WeatherMap.UI.Models
{
	public class ForecastPeriodVM
	{
		public int Number { get; set; }
		public string Name { get; set; }
		public bool IsDaytime { get; set; }

		[Display(Name = "Last Update")]
		public DateTime LastUpdate { get; set; }
		[Display(Name = "Temperature")]
		public string TemperatureF { get; set; }
		public string TemperatureC { get; set; }

		[Display(Name = "Probability Of Precipitation")]
		public string ProbabilityOfPrecipitation { get; set; }

		public string Dewpoint { get; set; }
		public string Humidity { get; set; }

		[Display(Name = "Wind Speed")]
		public string WindSpeed { get; set; }

		[Display(Name = "Wind Direction")]
		public string WindDirection { get; set; }

		public string Icon { get; set; }

		[Display(Name = "Forecast")]
		public string ShortForecast { get; set; }

		[Display(Name = "Detailed Forecast")]
		public string DetailedForecast { get; set; }
	}
}