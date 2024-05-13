using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeatherMap.UI.Models
{
	public class NWSLocalForecastsVM
	{
		public string Humidity { get; set; }
		public string WindSpeed { get; set; }
		public string Barometer { get; set; }
		public string Dewpoint { get; set; }
		public string Visibility { get; set; }
		public string HeatIndex { get; set; }
		public string LastUpdate { get; set; }
		public string SummaryCurrentConditions { get; set; }
		public string ImgCurrentConditions { get; set; }
		public string TempFahrenheit { get; set; }
		public string TempCelsius { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public string Elevation { get; set; }
	}
}