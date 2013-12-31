using System;
using N2.Addons.Tagging.Details;
using N2.Definitions;
using N2.Engine;
using N2.Plugin;

namespace N2.Addons.Tagging
{
    /// <summary>
    /// Registers components and services needed by the tagging addon.
    /// </summary>
    [AutoInitialize]
    public class TaggingInitializer : IPluginInitializer
    {
        bool usingTemplates = false;
        Type baseType = typeof (ContentItem);

        public void Initialize(IEngine engine)
        {
            Type contentPageType = Type.GetType("N2.Templates.Items.AbstractContentPage, N2.Templates");
            if (contentPageType != null)
            {
                usingTemplates = true;
                baseType = contentPageType;
            }

            foreach (ItemDefinition definition in engine.Definitions.GetDefinitions())
            {
                if (IsContentPage(definition))
                {
                    var tagEditable = new EditableTagsAttribute();
                    tagEditable.Name = "Tags";
                    tagEditable.SortOrder = 500;
                    if (usingTemplates)
                        tagEditable.ContainerName = "content";

                    definition.Add(tagEditable);
                }
            }
        }

        private bool IsContentPage(ItemDefinition definition)
        {
            return baseType.IsAssignableFrom(definition.ItemType);
        }
    }
}
