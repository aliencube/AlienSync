using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dts.Runtime;

namespace AlienSync.Core.Events
{
	public class SsisPackageEventListener : DefaultEvents
	{
		public override bool OnError(DtsObject source, int errorCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError)
		{
			return base.OnError(source, errorCode, subComponent, description, helpFile, helpContext, idofInterfaceWithError);
		}
	}
}
