using System.ComponentModel.DataAnnotations;

namespace WeatherMap.UI.Models
{
	public class CurrentConditionsVM
	{
		public string City { get; set; }
		public string State { get; set; }
		public string TimeZone { get; set; }
		public string RadarStation { get; set; }
		[Display(Name = "Lat")]
		public string Latitude { get; set; }
		[Display(Name = "Lon")]
		public string Longitude { get; set; }
		public string Elevation { get; set; }
		public string LocationID { get; set; }
		public string ForecastOffice { get; set; }
		public string ForecastOfficeLink { get; set; }
		public string GridID { get; set; }
		public string GridX { get; set; }
		public string GridY { get; set; }
		public ForecastPeriodVM ForecastPeriod { get; set; }

		//ExtendedForecastVM needs a List<> of ForecastPeriods
		//CurrentConditionsVM takes only the 1st period
	}
}