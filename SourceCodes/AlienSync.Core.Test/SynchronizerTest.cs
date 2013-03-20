using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlienSync.Core.Test
{
	[TestClass]
	public class SynchronizerTest
	{
		[TestMethod]
		public void WinScpInstalled_True()
		{
			try
			{
				var sync = new Synchronizer(null);
				Assert.IsTrue(sync.WinScpInstalled, "WinSCP not installed.");
			}
			catch (Exception ex)
			{
				Assert.Fail("Method has thrown an unexpected exception.\n\t" + ex.Message);
			}
		}

		[TestMethod]
		public void WinScpInstalled_False()
		{
			try
			{
				var sync = new Synchronizer(null);
				Assert.IsFalse(sync.WinScpInstalled, "WinSCP installed.");
			}
			catch (Exception ex)
			{
				Assert.Fail("Method has thrown an unexpected exception.\n\t" + ex.Message);
			}
		}

		[TestMethod]
		public void GitInstalled_True()
		{
			try
			{
				var sync = new Synchronizer(null);
				Assert.IsTrue(sync.GitInstalled, "Git not installed.");
			}
			catch (Exception ex)
			{
				Assert.Fail("Method has thrown an unexpected exception.\n\t" + ex.Message);
			}
		}
	}
}
