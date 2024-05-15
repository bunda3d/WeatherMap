namespace WeatherMap.UI.Models
{
	public class AllWeatherVM
	{ //composite or "Parent" VM required for Index since multiple VM are used there
		public CurrentConditionsVM CurrentConditions { get; set; }
		public ExtendedForecastVM ExtendedForecast { get; set; }
	}
}