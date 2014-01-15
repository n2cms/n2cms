using System;
using N2.Definitions;

namespace N2.Templates.Mvc.Models.Parts
{
    /// <summary>
    /// A base class for item parts in the templates project.
    /// </summary>
    public abstract class PartBase : ContentItem, IPart
    {
    }

    [Obsolete("Use PartBase and [PartDefinition]")]
    public abstract class AbstractItem : PartBase
    {
        public override bool IsPage
        {
            get { return false; }
        }
    }
}
