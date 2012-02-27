using System.IO;
using System.Web.Mvc;
using N2.Definitions;
using N2.Persistence;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Web.Mvc.Html
{
    internal static class TextWriterExtensions
    {
        public static TextWriter WriteAttribute(this TextWriter writer, string attributeName, string value)
        {
            writer.Write(" {0}=\"{1}\"", attributeName, value);
            return writer;
        }
    }

	public class DroppableZoneHelper : ZoneHelper
	{
		protected string ZoneTitle { get; set; }

        ControlPanelState state = ControlPanelState.Hidden;

        public DroppableZoneHelper(HtmlHelper helper, string zoneName, ContentItem currentItem)
            : base(helper, zoneName, currentItem)
		{
            state = helper.ViewContext.HttpContext.ControlPanelState();
        }

		public ZoneHelper Title(string title)
		{
			ZoneTitle = title;

			return this;
		}

        public override void Render(TextWriter writer)
        {
			if (state == ControlPanelState.DragDrop)
			{
				if (ZoneName.IndexOfAny(new[] { '.', ',', ' ', '\'', '"', '\t', '\r', '\n' }) >= 0) throw new N2Exception("Zone '" + ZoneName + "' contains illegal characters.");

				writer.Write("<div class='" + ZoneName + " dropZone'");
				writer.WriteAttribute(PartUtilities.PathAttribute, CurrentItem.Path)
					.WriteAttribute(PartUtilities.ZoneAttribute, ZoneName)
					.WriteAttribute(PartUtilities.AllowedAttribute, PartUtilities.GetAllowedNames(ZoneName, PartsAdapter.GetAllowedDefinitions(CurrentItem, ZoneName, Html.ViewContext.HttpContext.User)))
					.WriteAttribute("title", ZoneTitle ?? DroppableZone.GetToolTip(Html.ResolveService<IDefinitionManager>().GetDefinition(CurrentItem), ZoneName))
					.Write(">");

                RenderPreview(writer);
				
				writer.Write("</div>");
			}
            else if (state == ControlPanelState.Previewing)
            {
                RenderPreview(writer);
            }
            else
                base.Render(writer);
		}

        private void RenderPreview(TextWriter writer)
        {
            if (string.IsNullOrEmpty(Html.ViewContext.HttpContext.Request["preview"]))
            {
                base.Render(writer);
            }
            else
            {
                string preview = Html.ViewContext.HttpContext.Request["preview"];
                RenderReplacingPreviewed(writer, preview);
            }
        }

        protected override string GetInterface()
        {
            return state.GetInterface();
        }

		protected void RenderReplacingPreviewed(TextWriter writer, string preview)
		{
			int itemID;
			if (int.TryParse(preview, out itemID))
			{
				ContentItem previewedItem = Html.ResolveService<IPersister>().Get(itemID);
				if (previewedItem != null && previewedItem.VersionOf.HasValue)
				{
					foreach (var child in PartsAdapter.GetParts(CurrentItem, ZoneName, GetInterface()))
					{
						if (previewedItem.VersionOf.Value == child)
							RenderTemplate(writer, previewedItem);
						else
							RenderTemplate(writer, child);
					}
				}
			}
		}

        protected override void RenderTemplate(TextWriter writer, ContentItem model)
        {			
            if (state == ControlPanelState.DragDrop)
            {
				ItemDefinition definition = Html.ResolveService<IDefinitionManager>().GetDefinition(model);

                writer.Write("<div class='" + definition.Discriminator + " zoneItem'");
                writer.WriteAttribute(PartUtilities.PathAttribute, model.Path)
                    .WriteAttribute(PartUtilities.TypeAttribute, definition.Discriminator)
                    .Write(">");

				Html.ResolveService<PartUtilities>().WriteTitleBar(writer, model, Html.ViewContext.HttpContext.Request.RawUrl);
                
                base.RenderTemplate(writer, model);

                writer.Write("</div>");
            }
            else
                base.RenderTemplate(writer, model);
        }

        public DroppableZoneHelper AllowExternalManipulation()
        {
            // TODO: Not implemented
            return this;
        }
	}
}