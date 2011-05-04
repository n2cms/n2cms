using N2.Definitions;
using N2.Web;

namespace N2.Management.Externals
{
	[Definition(IconUrl = "{ManagementUrl}/Resources/icons/shape_square.png")]
	[Template(ExternalItem.ContainerTemplateKey, "{ManagementUrl}/Externals/External.ascx")]
	[Versionable(AllowVersions.No)]
	[Throwable(AllowInTrash.No)]
	public class ExternalItem : ContentItem, ISystemNode
	{
		public const string ContainerTemplateKey = "container";
		public const string ExternalContainerName = "Externals";
		public const string SingleItemKey = "Single";

		public override bool IsPage
		{
			get
			{
				return (TemplateKey == ContainerTemplateKey)
					? false
					: true;
			}
		}

		public override string IconUrl
		{
			get
			{
				if (TemplateKey != ContainerTemplateKey)
					return base.IconUrl;

				if (ZoneName == ExternalContainerName)
					return N2.Web.Url.ResolveTokens("{ManagementUrl}/Resources/icons/shape_move_forwards.png");

				return N2.Web.Url.ResolveTokens("{ManagementUrl}/Resources/icons/shape_move_front.png");
			}
		}

		public override string Url
		{
			get
			{
				if (TemplateKey == ContainerTemplateKey)
					return base.Url;

				return this["ExternalUrl"] as string;
			}
		}
	}
}