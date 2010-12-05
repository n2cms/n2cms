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
	public static class PartUtilities
	{
		public const string TypeAttribute = "data-type";
		public const string PathAttribute = "data-item";
		public const string ZoneAttribute = "data-zone";
		public const string AllowedAttribute = "data-allowed";

		public static string GetAllowedNames(string zoneName, IEnumerable<ItemDefinition> definitions)
		{
			var allowedDefinitions = new List<string>();
			foreach (ItemDefinition potentialChild in definitions)
			{
				allowedDefinitions.Add(potentialChild.Discriminator);
			}
			return string.Join(",", allowedDefinitions.ToArray());
		}

		[Obsolete("Use overload with adapters parameter.")]
		public static void WriteTitleBar(TextWriter writer, IEditUrlManager editUrlManager, ItemDefinition definition,
		                                 ContentItem item)
		{
			WriteTitleBar(writer, editUrlManager, Context.Current.Resolve<IContentAdapterProvider>(), definition, item);
		}

		public static void WriteTitleBar(TextWriter writer, IEditUrlManager editUrlManager, IContentAdapterProvider adapters,
		                                 ItemDefinition definition, ContentItem item)
		{
			writer.Write("<div class='titleBar ");
			writer.Write(definition.Discriminator);
			writer.Write("'>");

			Url returnUrl =
				Url.Parse(adapters.ResolveAdapter<NodeAdapter>(item.GetContentType()).GetPreviewUrl(item)).AppendQuery("edit",
				                                                                                                       "drag");
			WriteCommand(writer, "Edit part", "command edit",
			             Url.Parse(editUrlManager.GetEditExistingItemUrl(item)).AppendQuery("returnUrl", returnUrl).Encode());
			WriteCommand(writer, "Delete part", "command delete",
			             Url.Parse(editUrlManager.GetDeleteUrl(item)).AppendQuery("returnUrl", returnUrl).Encode());
			WriteTitle(writer, definition);

			writer.Write("</div>");
		}

		private static void WriteCommand(TextWriter writer, string title, string @class, string url)
		{
			writer.Write("<a title='" + title + "' class='" + @class + "' href='");
			writer.Write(url);
			writer.Write("'></a>");
		}

		private static void WriteTitle(TextWriter writer, ItemDefinition definition)
		{
			writer.Write("<span class='title' style='background-image:url(");
			writer.Write(Url.ResolveTokens(definition.IconUrl));
			writer.Write(");'>");
			writer.Write(definition.Title);
			writer.Write("</span>");
		}
	}
}