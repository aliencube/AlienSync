using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlienSync.Core.Test
{
	[TestClass]
	public class SettingsTest
	{
		[TestMethod]
		public void GitLocalRepository_IsNotEmptyNorNull()
		{
			try
			{
				var settings = Settings.Instance;
				Assert.IsTrue(!String.IsNullOrEmpty(settings.GitLocalRepositoryPath), "Value is NULL or empty");
			}
			catch (Exception ex)
			{
				Assert.Fail("Method has thrown an unexpected exception.\n\t" + ex.Message);
			}
		}
	}
}
