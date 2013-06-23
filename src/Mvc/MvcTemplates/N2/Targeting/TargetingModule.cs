using N2.Configuration;
using N2.Management.Api;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace N2.Management.Targeting
{
	[ManagementModule]
	public class TargetingModule : ManagementModuleBase
	{
		private ConfigurationManagerWrapper config;

		public TargetingModule(InterfaceBuilder builder, ConfigurationManagerWrapper config)
		{
			this.config = config;
			builder.InterfaceBuilt += builder_InterfaceBuilt;
		}

		void builder_InterfaceBuilt(object sender, InterfaceBuiltEventArgs e)
		{
			e.Data.Partials.Preview = "{ManagementUrl}/Targeting/Partials/DevicePreview.html".ResolveUrlTokens();
			e.Data.ActionMenu.Add("preview",
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "devicepreview", Title = "Device preview", IconClass = "n2-icon-mobile-phone", TemplateUrl = "{ManagementUrl}/Targeting/Partials/DeviceMenu.html".ResolveUrlTokens() })
				{
					//Children = new Node<InterfaceMenuItem>[]
					//{
					//	new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "pc", IconClass = "n2-icon-laptop", Title = "PC", ClientAction = "$emit('device-restore', null)" }),
					//	new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "mobiledivider1", Divider = true }),
					//	new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "mobilevertical", IconClass = "n2-icon-mobile-phone", Title = "Mobile", ClientAction = "$emit('device-preview', { title:'Mobile', w: 480, h: 780 })" }),
					//	new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "mobilehorizontal", IconClass = "n2-icon-mobile-phone tilted90", Title = "Mobile horizontal", ClientAction = "$emit('device-preview', { title: 'Mobile (horizontal)', w: 780, h: 480 })" }),
					//	new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "mobiledivider2", Divider = true }),
					//	new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "tabletvertical", IconClass = "n2-icon-tablet", Title = "Tablet", ClientAction = "$emit('device-preview', { title: 'Tablet (horizontal)', w: 780, h: 1024 })" }),
					//	new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "tablethorizontal", IconClass = "n2-icon-tablet tilted90", Title = "Tablet horizontal", ClientAction = "$emit('device-preview', { title: 'Tablet (horizontal)', w: 1024, h: 780 })" }),
					//}
					Children = config.GetContentSection<Configuration.TargetingSection>("targeting", required: false).PreviewSizes.AllElements
						.Select(te => new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = te.Title, Name = te.Name, IconClass = te.IconClass, ClientAction = string.Format("$emit('device-preview', {0})", new { te.Title, te.Name, te.IconClass, te.Width, te.Height }.ToJson()) })).ToList()
				}, insertBeforeSiblingWithName: "previewdivider1");
		}

		public override IEnumerable<string> ScriptIncludes
		{
			get
			{
				yield return "{ManagementUrl}/Targeting/Js/Targeting.js";
			}
		}

		public override IEnumerable<string> StyleIncludes
		{
			get
			{
				yield return "{ManagementUrl}/Targeting/Css/Targeting.css";
			}
		}
	}
}