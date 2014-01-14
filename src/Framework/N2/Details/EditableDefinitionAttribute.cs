using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using N2.Definitions;

namespace N2.Details
{
    /// <summary>
    /// Allows selection of a template
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableDefinitionAttribute : EditableListControlAttribute
    {
        public EditableDefinitionAttribute()
            : this("Definition", 110)
        {
        }

        public EditableDefinitionAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }



        /// <summary>
        /// Select templates rather than definitions.
        /// </summary>
        public bool TemplateSelection { get; set; }

        /// <summary>
        /// Types that are filtered.
        /// </summary>
        public Type[] RemovedTypes { get; set; }



        protected override System.Web.UI.WebControls.ListControl CreateEditor()
        {
            return new DropDownList();
        }

        protected override System.Web.UI.WebControls.ListItem[] GetListItems()
        {
            if(TemplateSelection)
                return Engine.Definitions.GetDefinitions()
                    .Where(IsUnfiltered)
                    .SelectMany(d => Engine.Resolve<ITemplateAggregator>().GetTemplates(d.ItemType))
                    .Select(t => new ListItem(t.Title, t.Definition.GetDiscriminatorWithTemplateKey()))
                    .ToArray();

            return Engine.Definitions.GetDefinitions()
                .Where(IsUnfiltered)
                .Select(d => new ListItem(d.Title, d.Discriminator))
                .ToArray();
        }

        public bool IsUnfiltered(ItemDefinition d)
        {
            if(RemovedTypes == null)
                return true;

            return !RemovedTypes.Any(rt => rt.IsAssignableFrom(d.ItemType));
        }
    }
}
