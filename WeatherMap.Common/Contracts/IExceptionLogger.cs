using System;
using System.Threading.Tasks;

namespace WeatherMap.Common.Contracts
{
	public interface IExceptionLogger
	{
		Task LogExceptionAsync(Exception ex);
	}
}

//To avoid adding a reference between the business logic and data access projects, add a Common project they can both reference without violating separation of concerns:
//Use an Interface: Define an interface in the Data layer that represents the contract for logging exceptions.
//The Core layer can then implement this interface.
//This way, the Data layer only knows about the interface and not the concrete implementation.