using System;
using System.IO;
using System.Web.Mvc;
using N2.Definitions;
using N2.Persistence;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2.Edit.Versioning;

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
            state = helper.GetControlPanelState();
        }

        public ZoneHelper Title(string title)
        {
            ZoneTitle = title;

            return this;
        }

        public override void Render(TextWriter writer)
        {
            if (writer == null)
                throw new NullReferenceException("writer cannot be null");

            if (state.IsFlagSet(ControlPanelState.DragDrop))
            {
                if (String.IsNullOrWhiteSpace(ZoneName))
                    throw new N2Exception("Zone name cannot be null, empty, or consist only of whitespace.");
                if (ZoneName.IndexOfAny(new[] { '.', ',', ' ', '\'', '"', '\t', '\r', '\n' }) >= 0) 
                    throw new N2Exception("Zone '" + ZoneName + "' contains illegal characters.");
                if (CurrentItem == null)
                    throw new N2Exception("CurrentItem cannot be null");
                var dm = Html.ResolveService<IDefinitionManager>();
                if (dm == null)
                    throw new N2Exception("Failed to resolve the definition manager.");



                writer.Write("<div class=\"" + ZoneName + " dropZone\"");
				if (CurrentItem.ID != 0 && !CurrentItem.VersionOf.HasValue)
				{
					writer.WriteAttribute("data-id", CurrentItem.ID.ToString());
					writer.WriteAttribute(PartUtilities.PathAttribute, CurrentItem.Path);
				}
				else
				{
					writer.WriteAttribute(PartUtilities.PathAttribute, (Find.ClosestPage(CurrentItem) ?? CurrentItem).Path);
					writer.WriteAttribute("data-versionKey", CurrentItem.GetVersionKey());
					writer.WriteAttribute("data-versionIndex", CurrentItem.VersionIndex.ToString());
				}
                writer.WriteAttribute(PartUtilities.ZoneAttribute, ZoneName)
                    .WriteAttribute(PartUtilities.AllowedAttribute, PartUtilities.GetAllowedNames(ZoneName, PartsAdapter.GetAllowedDefinitions(CurrentItem, ZoneName, Html.ViewContext.HttpContext.User)))
                    .WriteAttribute("title", ZoneTitle ?? DroppableZone.GetToolTip(dm.GetDefinition(CurrentItem), ZoneName));
                writer.Write(">");

                RenderPreview(writer);
                
                writer.Write("</div>");
            }
            else if (state.IsFlagSet(ControlPanelState.Previewing))
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
            if (state.IsFlagSet(ControlPanelState.DragDrop) && model.Parent == CurrentItem)
            {
                ItemDefinition definition = Html.ResolveService<IDefinitionManager>().GetDefinition(model);

                writer.Write("<div class='" + definition.Discriminator + " zoneItem'");
				if (model.ID != 0 && !model.VersionOf.HasValue)
				{
					writer.WriteAttribute("data-id", model.ID.ToString());
					writer.WriteAttribute(PartUtilities.PathAttribute, model.Path);
				}
				else
				{
					writer.WriteAttribute(PartUtilities.PathAttribute, Find.ClosestPage(model).Path);
					writer.WriteAttribute("data-versionKey", model.GetVersionKey())
						.WriteAttribute("data-versionIndex", model.VersionIndex.ToString());
				}
                writer.WriteAttribute("data-sortOrder", model.SortOrder.ToString())
                    .WriteAttribute(PartUtilities.TypeAttribute, definition.Discriminator);

                writer.Write(">");

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
