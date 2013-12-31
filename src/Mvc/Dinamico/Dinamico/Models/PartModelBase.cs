using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2;
using N2.Details;
using N2.Web.UI;
using N2.Definitions;

namespace Dinamico.Models
{
    /// <summary>
    /// Base implementation of parts on a dinamico site.
    /// </summary>
    [SidebarContainer(Defaults.Containers.Metadata, 100, HeadingText = "Metadata")]
    public abstract class PartModelBase : ContentItem, IPart
    {
    }
}
