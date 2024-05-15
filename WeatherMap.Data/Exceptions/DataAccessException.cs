using System;

namespace WeatherMap.Data.Exceptions
{
	//extends the Exception object so methods throwing exceptions in the Data project
	//can return exception data to an exception logging service even though it is
	//in the Core project, of which the Data project has no dependency nor knowledge of.

	[Serializable]
	public class DataAccessException : Exception
	{
		public DataAccessException()
		{ }

		public DataAccessException(string message) : base(message)
		{ }

		public DataAccessException(string message, Exception inner) : base(message, inner)
		{ }

		// constructor for serialization
		protected DataAccessException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context
		) : base(info, context) { }
	}
}