using N2.Configuration;
using N2.Management.Api;
using N2.Web;
using N2.Web.Targeting;
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
		private DetectorBase[] detectors;

		public TargetingModule(InterfaceBuilder builder, ConfigurationManagerWrapper config, DetectorBase[] detectors)
		{
			this.config = config;
			this.detectors = detectors;
			builder.InterfaceBuilt += builder_InterfaceBuilt;
		}

		void builder_InterfaceBuilt(object sender, InterfaceBuiltEventArgs e)
		{
			e.Data.Partials.Preview = "{ManagementUrl}/Targeting/Partials/DevicePreview.html".ResolveUrlTokens();
			e.Data.ActionMenu.Add("preview",
				new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "devicepreview", Title = "Device preview", IconClass = "fa fa-mobile", TemplateUrl = "{ManagementUrl}/Targeting/Partials/DeviceMenu.html".ResolveUrlTokens(), Url = "#" })
				{
					Children = config.GetContentSection<Configuration.TargetingSection>("targeting", required: false).PreviewSizes.AllElements
						.Select(te => new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = te.Title, Name = te.Name, IconClass = te.IconClass, ClientAction = string.Format("$emit('device-preview', {0})", new { te.Title, te.Name, te.IconClass, te.Width, te.Height }.ToJson()), SelectedBy = "Preview" + te.Name })).ToList()
				}, insertBeforeSiblingWithName: "previewdivider1");
			e.Data.ActionMenu.Add("preview",
							new Node<InterfaceMenuItem>(new InterfaceMenuItem { Name = "targetspreview", Title = "Target preview", IconClass = "fa fa-bullseye", TemplateUrl = "{ManagementUrl}/Targeting/Partials/TargetMenu.html".ResolveUrlTokens(), Url = "#" })
							{
								Children = detectors
									.Select(d => new Node<InterfaceMenuItem>(new InterfaceMenuItem { Title = d.Description.Title, Name = d.Name, IconClass = d.Description.IconClass, ClientAction = string.Format("$emit('target-preview', {0})", new { d.Name, d.Description.Title, d.Description.IconClass }.ToJson()), SelectedBy = "Target" + d.Name })).ToList()
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