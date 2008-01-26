using System;
using System.Collections.Generic;
using System.Text;
using N2.Integrity;
using System.Web.UI.WebControls;

namespace N2.Templates.Items.LayoutParts
{
	[N2.Definition("Top", "Top")]
	[N2.Web.UI.FieldSet("top", "Top", 100)]
	[RestrictParents(typeof(StartPage))] // The top region is placed on the start page and displayed on all underlying pages
	[AllowedZones("SiteTop")]
	public class Top : AbstractItem
	{
		[N2.Details.Displayable(typeof(N2.Web.UI.WebControls.H2), "Text")]
		[N2.Details.Editable("Top text", typeof(TextBox), "Text", 40, ContainerName = "top")]
		public override string Title
		{
			get { return base.Title; }
			set { base.Title = value; }
		}

		[N2.Details.EditableUrl("Top text url", 42, ContainerName = "top")]
		public virtual string TopTextUrl
		{
			get { return (string)(GetDetail("TopTextUrl") ?? "/"); }
			set { SetDetail("TopTextUrl", value, "/"); }
		}

		[N2.Details.EditableImage("Logo", 50, ContainerName = "top", Alt="Logo")]
		public virtual string LogoUrl
		{
			get { return (string)(GetDetail("LogoUrl") ?? string.Empty); }
			set { SetDetail("LogoUrl", value); }
		}

		[N2.Details.EditableUrl("Logo url", 52, ContainerName = "top")]
		public virtual string LogoLinkUrl
		{
			get { return (string)(GetDetail("LogoLinkUrl") ?? "/"); }
			set { SetDetail("LogoLinkUrl", value, "/"); }
		}

		public override string TemplateUrl
		{
			get
			{
				return "~/Layouts/Parts/Top.ascx";
			}
		}

		public override string IconUrl
		{
			get
			{
				return "~/Img/page_white_star.png";
			}
		}
	}
}
