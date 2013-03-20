using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlienSync.Core.Exceptions;

namespace AlienSync.Core.Test
{
	[TestClass]
	public class AppConfigTest
	{
		[TestMethod]
		public void GetSessionOptions_WithArgs_ThrowsNotImplementedException()
		{
			try
			{
				var mi = typeof (Parameters).GetMethod("GetSessionOptions", BindingFlags.NonPublic | BindingFlags.Static);
				mi.Invoke(null, new object[] {new string[] {"foo"}});
				Assert.Fail("Method returns an object.");
			}
			catch (NotImplementedException ex)
			{
				var result = ex.GetType() == typeof (NotImplementedException);
				Assert.IsTrue(result, "Different exception invoked - " + ex.GetType().Name);
			}
			catch (TargetInvocationException ex)
			{
				var result = ex.InnerException != null && ex.InnerException.GetType() == typeof (NotImplementedException);
				Assert.IsTrue(result, "Different exception invoked - " + ex.GetType().Name);
			}
			catch (Exception ex)
			{
				Assert.Fail("Method has thrown an unexpected exception.\n\t" + ex.Message);
			}
		}

		[TestMethod]
		public void GetSessionOptions_WithoutConnectionString_ThrowsInvalidConfigurationException()
		{
			try
			{
				var mi = typeof(Parameters).GetMethod("GetSessionOptions", BindingFlags.NonPublic | BindingFlags.Static);
				mi.Invoke(null, new object[] {null});
				Assert.Fail("Method returns an object.");
			}
			catch (InvalidConfigurationException ex)
			{
				var result = ex.GetType() == typeof(InvalidConfigurationException);
				Assert.IsTrue(result, "Different exception invoked - " + ex.GetType().Name);
			}
			catch (TargetInvocationException ex)
			{
				var result = ex.InnerException != null && ex.InnerException.GetType() == typeof (InvalidConfigurationException);
				Assert.IsTrue(result, "Different exception invoked - " + ex.GetType().Name);
			}
			catch (Exception ex)
			{
				Assert.Fail("Method has thrown an unexpected exception.\n\t" + ex.Message);
			}
		}

		[TestMethod]
		public void LocalDirectories_ContainsDirectories()
		{
			try
			{
				var settings = Settings.Instance;
				var value = settings.ScpLocalDirectories;
				Assert.IsTrue(value != null && value.Any(), "No directory found");
			}
			catch (Exception ex)
			{
				Assert.Fail("Method has thrown an unexpected exception.\n\t" + ex.Message);
			}
		}
	}
}
