using N2.Definitions;
using N2.Installation;
using N2.Integrity;
using N2.Web;
using N2.Details;
using N2.Web.UI;
using N2.Security;
using N2.Management.Api;
using N2.Edit.Collaboration;
using N2.Persistence;
using System.Linq;
using System.Collections.Generic;

namespace N2.Management.Myself
{
    [PageDefinition("Root Page (fallback)", 
        Description = "A fallback root page used to organize start pages. This root can be replaced or inherited in a web application project.", 
        SortOrder = 0,
        IconClass = "fa fa-database",
        TemplateUrl = "{ManagementUrl}/Myself/Root.aspx",
		InstallerVisibility = InstallerHint.NeverStartPage)]
    [RestrictParents(AllowedTypes.None)]
    [AvailableZone("Left", "Left")]
    [AvailableZone("Center", "Center")]
    [AvailableZone("Right", "Right")]
    [AvailableZone("Above", "Above")]
	[AvailableZone("Below", "Below")]
	[AvailableZone("Collaboration", "Collaboration")]
    [RecursiveContainer("RootSettings", 120, RequiredPermission = Permission.Administer)]
	[TabContainer("Search", "Search", 120, ContainerName = "RootSettings")]
	//[TabContainer("Collaboration", "Collaboration", 130, ContainerName = "RootSettings")]
    [WithManageableSearch(ContainerName = "Search")]
    [Versionable(AllowVersions.No)]
    [InterfaceFlags(RemovedFlags = new [] { "Management" })]
    public class RootBase : ContentItem, IRootPage, ISystemNode, IMessageSource, IFlagSource
    {
        public override string Url
        {
            get { return FindPath(PathData.DefaultAction).GetRewrittenUrl(); }
        }

		//[EditableChildren(ContainerName = "Collaboration", ZoneName = "Collaboration")]
		//public virtual IEnumerable<IMessageSource> Messages { get; set; }

		public IEnumerable<CollaborationMessage> GetMessages(CollaborationContext context)
		{
			return Children.FindParts("Collaboration")
				.OfType<IMessageSource>()
				.SelectMany(ms => ms.GetMessages(context));
		}

		public IEnumerable<string> GetFlags(CollaborationContext context)
		{
			return Children.FindParts("Collaboration")
				.OfType<IFlagSource>()
				.SelectMany(ms => ms.GetFlags(context));
		}
	}
}
