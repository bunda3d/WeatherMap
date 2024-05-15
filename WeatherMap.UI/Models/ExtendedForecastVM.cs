using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeatherMap.UI.Models
{
	public class ExtendedForecastVM
	{
		public string City { get; set; }
		public string State { get; set; }

		[Display(Name = "Time Zone")]
		public string TimeZone { get; set; }

		[Display(Name = "Radar Station")]
		public string RadarStation { get; set; }

		[Display(Name = "Lat")]
		public string Latitude { get; set; }

		[Display(Name = "Lon")]
		public string Longitude { get; set; }

		public string Elevation { get; set; }
		public string LocationID { get; set; }

		[Display(Name = "Forecast Office")]
		public string ForecastOffice { get; set; }

		[Display(Name = "Forecast Office Link")]
		public string ForecastOfficeLink { get; set; }

		[Display(Name = "County Warning Area")]
		public string GridID { get; set; }

		public string GridX { get; set; }
		public string GridY { get; set; }
		public List<ForecastPeriodVM> ForecastPeriods { get; set; }

		//must initialize the ForecastPeriods list in constructor of ExtendedForecastVM
		//this ensures it’s never null, as that CAN make the app puke
		public ExtendedForecastVM()
		{
			ForecastPeriods = new List<ForecastPeriodVM>();
		}
	}
}