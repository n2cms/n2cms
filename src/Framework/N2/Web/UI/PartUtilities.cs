using System;
using System.Collections.Generic;
using System.IO;
using N2.Definitions;
using N2.Edit;
using N2.Engine;

namespace N2.Web.UI
{
    /// <summary>
    /// Helper methods for drag'n'drop support.
    /// </summary>
    [Service]
    public class PartUtilities
    {
        public const string TypeAttribute = "data-type";
        public const string TemplateAttribute = "data-template";
        public const string PathAttribute = "data-item";
        public const string ZoneAttribute = "data-zone";
        public const string AllowedAttribute = "data-allowed";

        IEditUrlManager managementUrls;
        IDefinitionManager definitions;

        public PartUtilities(IEditUrlManager managementUrls, IDefinitionManager definitions)
        {
            this.managementUrls = managementUrls;
            this.definitions = definitions;
        }

        public static string GetAllowedNames(string zoneName, IEnumerable<ItemDefinition> definitions)
        {
            var allowedDefinitions = new List<string>();
            foreach (ItemDefinition potentialChild in definitions)
            {
                allowedDefinitions.Add(potentialChild.Discriminator);
            }
            return string.Join(",", allowedDefinitions.ToArray());
        }

        [Obsolete("Use overload with returnUrl parameter.", true)]
        public static void WriteTitleBar(TextWriter writer, IEditUrlManager editUrlManager, IContentAdapterProvider adapters, ItemDefinition definition, ContentItem item)
        {
            Url returnUrl = Url.Parse(adapters.ResolveAdapter<NodeAdapter>(item).GetPreviewUrl(item)).AppendQuery("edit", "drag");
            
            N2.Context.Current.Resolve<PartUtilities>().WriteTitleBar(writer, item, returnUrl);
        }

        public void WriteTitleBar(TextWriter writer, ContentItem item, string returnUrl)
        {
            var definition = definitions.GetDefinition(item);

            writer.Write("<div class='titleBar ");
            writer.Write(definition.Discriminator);
            writer.Write("'>");

			string editUrl = Url.Parse(managementUrls.GetEditExistingItemUrl(item)).AppendQuery("returnUrl", returnUrl).Encode();
            WriteTitle(writer, definition, editUrl);

			writer.Write("<span class='commands'>");
			WriteCommand(writer, "Delete part", "command delete", "fa fa-trash-o", Url.Parse(managementUrls.GetDeleteUrl(item)).AppendQuery("returnUrl", returnUrl).Encode());
			WriteCommand(writer, "Edit part", "command edit", "fa fa-pencil-square", editUrl);
            WriteCommand(writer, "Move part", "command move", "fa fa-arrows", "#");
            writer.Write("</span>");

            writer.Write("</div>");
        }

        private static void WriteCommand(TextWriter writer, string title, string @class, string faClass, string url)
        {
            writer.Write("<a title='" + title + "' class='" + @class + "' href='" + url + "'>");
            writer.Write("<b class='" + faClass + "'></b>");
            writer.Write("</a>");
		}

        private static void WriteTitle(TextWriter writer, ItemDefinition definition, string editUrl)
        {
            if (string.IsNullOrEmpty(definition.IconUrl))
            {
                writer.Write("<a class='title' href='" + editUrl + "'>");
                writer.Write("<b class='" + definition.IconClass + "'></b>");
                writer.Write(definition.Title);
                writer.Write("</a>");
            }
            else
            {
                writer.Write("<a class='title' href='" + editUrl + "' style='background-image:url(");
                writer.Write(Url.ResolveTokens(definition.IconUrl));
                writer.Write(");'>");
                writer.Write(definition.Title);
                writer.Write("</a>");
            }
        }
    }
}
