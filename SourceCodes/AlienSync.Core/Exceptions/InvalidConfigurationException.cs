﻿using System;
using System.Runtime.Serialization;

namespace AlienSync.Core.Exceptions
{
	/// <summary>
	/// The exception that is thrown when an invalid configuration data is defined.
	/// </summary>
	public class InvalidConfigurationException : ApplicationException
	{
		/// <summary>
		/// Initialises a new instance of the InvalidConfigurationException object.
		/// </summary>
		public InvalidConfigurationException()
			: base()
		{
		}

		/// <summary>
		/// Initialises a new instance of the InvalidConfigurationException object with serialised data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		public InvalidConfigurationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// Initializes a new instance of the System.ApplicationException class with a specified error message.
		/// </summary>
		/// <param name="message">A message that describes the error.</param>
		public InvalidConfigurationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the System.ApplicationException class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
		public InvalidConfigurationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
