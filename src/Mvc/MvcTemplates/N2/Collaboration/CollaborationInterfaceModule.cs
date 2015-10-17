using N2.Configuration;
using N2.Management.Api;
using N2.Security;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Collaboration
{
	[ManagementModule]
	public class CollaborationInterfaceModule : ManagementModuleBase
	{
		private ConfigurationManagerWrapper config;

		public CollaborationInterfaceModule(InterfaceBuilder builder, ConfigurationManagerWrapper config)
		{
			this.config = config;
			builder.InterfaceBuilt += builder_InterfaceBuilt;
		}

		void builder_InterfaceBuilt(object sender, InterfaceBuiltEventArgs e)
		{
			e.Data.ActionMenu.Add("info", new InterfaceMenuItem { Name = "notes", TemplateUrl = "{ManagementUrl}/Collaboration/Partials/Notes.html".ResolveUrlTokens(), RequiredPermission = Permission.Read });

		}

		public override IEnumerable<string> ScriptIncludes
		{
			get
			{
				yield return "{ManagementUrl}/Collaboration/Js/Collaboration.js";
			}
		}

		public override IEnumerable<string> StyleIncludes
		{
			get
			{
				yield return "{ManagementUrl}/Collaboration/Css/Collaboration.css";
			}
		}
	}
}