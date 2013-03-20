using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlienSync.Core.Wrappers;

namespace AlienSync.Core.Test
{
	[TestClass]
	public class GitWrapperTest
	{
		[TestMethod]
		public void IsValidGitRepository_True()
		{
			try
			{
				var repo = new GitWrapper(Settings.Instance);
				Assert.IsTrue(repo.IsValidLocalRepository, String.Format("Repository not found: {0}", repo.GitDirectoryPath));
			}
			catch (Exception ex)
			{
				Assert.Fail("Method has thrown an unexpected exception.\n\t" + ex.Message);
			}
		}
	}
}
