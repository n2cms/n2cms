using N2.Configuration;
using N2.Management.Api;
using N2.Security;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Statistics
{
	[ManagementModule]
	public class StatisticsInterfaceModule : ManagementModuleBase
	{
		private ConfigurationManagerWrapper config;

		public StatisticsInterfaceModule(InterfaceBuilder builder, ConfigurationManagerWrapper config)
		{
			this.config = config;
			builder.InterfaceBuilt += builder_InterfaceBuilt;
		}

		void builder_InterfaceBuilt(object sender, InterfaceBuiltEventArgs e)
		{
			e.Data.ActionMenu.Add("info", new InterfaceMenuItem { Name = "statistics", TemplateUrl = "{ManagementUrl}/Statistics/Partials/PageStats.html".ResolveUrlTokens(), RequiredPermission = Permission.Read });
		}

		public override IEnumerable<string> ScriptIncludes
		{
			get
			{
				yield return "{ManagementUrl}/Statistics/Js/Statistics.js";
			}
		}

		public override IEnumerable<string> StyleIncludes
		{
			get
			{
				yield return "{ManagementUrl}/Statistics/Css/Statistics.css";
			}
		}
	}
}