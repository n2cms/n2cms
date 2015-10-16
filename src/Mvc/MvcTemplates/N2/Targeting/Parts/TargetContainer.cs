using N2.Definitions;
using N2.Details;
using N2.Web.Rendering;
using N2.Web.Targeting;
using N2.Web.UI.WebControls;
using N2.Web.Mvc.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using N2.Engine;
using N2.Web;
using N2.Security;
using System.Web.UI.HtmlControls;
using N2.Integrity;

namespace N2.Management.Targeting.Parts
{
	[ContentRenderer]
	public class TargetContainerRenderer : ContentRendererBase<TargetContainer>
	{
		private Logger<TargetContainerRenderer> logger;
		private IEngine engine;

		public TargetContainerRenderer(IEngine engine)
		{
			this.engine = engine;
		}

		public override void Render(ContentRenderingContext context, System.IO.TextWriter writer)
		{
			var item = (TargetContainer)context.Content;
			var isMatchedByTarget = engine.RequestContext.HttpContext.GetTargetingContext(engine).TargetedBy.Any(d => d.Name == item.Targets);
			bool isOrganizing = engine.RequestContext.HttpContext.Request["edit"] == "drag";
			if (!isMatchedByTarget)
				if (!isOrganizing || !engine.SecurityManager.IsEditor(engine.RequestContext.HttpContext.User))
					return;

			if (context.Container != null)
			{
				var zone = new DroppableZone { CurrentItem = context.Content, ZoneName = "Content" };
				if (isOrganizing)
				{
					var containingDiv = new HtmlGenericControl("div");
					containingDiv.Attributes["class"] = "target-container";
					context.Container.Controls.Add(containingDiv);

					var detectorDescription = GetDetector(item).Description;
					var nameSpan = new HtmlGenericControl("span");
					nameSpan.InnerHtml = "<b class='" + detectorDescription.IconClass + "'></b> " + detectorDescription.Title;
					nameSpan.Attributes["class"] = "target-name";
					containingDiv.Controls.Add(nameSpan);

					containingDiv.Controls.Add(zone);
				}
				else
					context.Container.Controls.Add(zone);
			}
			else if (context.Html != null)
			{
				if (isOrganizing)
				{
					var detectorDescription = GetDetector(item).Description;
					writer.Write("<div class='target-container'><span class='target-name'><b class='" + detectorDescription.IconClass + "'></b> " + detectorDescription.Title + "</span>");
				}
				var zone = context.Html.DroppableZone(context.Content, "Content");
				zone.Render(writer);
				if (isOrganizing)
					writer.Write("</div>");
			}
			else
				logger.WarnFormat("Unable to render part {0}", context.Content);
		}

		private DetectorBase GetDetector(TargetContainer item)
		{
			return engine.Resolve<N2.Web.Targeting.TargetingRadar>().Detectors.FirstOrDefault(d => d.Name == item.Targets) ?? new InvalidTarget();
		}
	}

	public class InvalidTarget : DetectorBase
	{
		public override bool IsTarget(TargetingContext context)
		{
			throw new NotImplementedException();
		}
		public override DetectorBase.DetectorDescription Description
		{
			get
			{
				var d = base.Description;
				d.IconClass = "fa fa-exclamation-sign";
				return d;
			}
		}
	}

	[PartDefinition("Targeted Content Container", IconClass = "fa fa-bullseye n2-magenta")]
	[AvailableZone("Targeted content", "Content")]
	public class TargetContainer : ContentItem, IPart
	{
		[EditableTargets]
		public virtual string Targets { get; set; }
	}
}
