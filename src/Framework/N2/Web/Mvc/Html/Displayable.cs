using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Definitions.Static;
using N2.Web.Rendering;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using System.Web;

namespace N2.Web.Mvc.Html
{
    public class Displayable : ItemHelper, IHtmlString
    {
        private readonly Engine.Logger<Displayable> logger;
        readonly string propertyName;
        string path;
        bool swallowExceptions = RenderHelper.DefaultSwallowExceptions;
        bool isOptional = RenderHelper.DefaultOptional;
        bool isEditable = RenderHelper.DefaultEditable;

        public Displayable(HtmlHelper helper, string propertyName, ContentItem currentItem)
            : base(helper, currentItem)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            this.propertyName = propertyName;
        }

        protected TagBuilder Wrapper { get; set; }
        public string Value
        {
            get
            {
                try
                {
                    return Convert.ToString(CurrentItem[propertyName]);
                }
                catch
                {
                    if (swallowExceptions)
                        return String.Empty;

                    throw;
                }
            }
        }

        public Displayable WrapIn(string tagName, object attributes)
        {
            Wrapper = new TagBuilder(tagName);
            Wrapper.MergeAttributes(new RouteValueDictionary(attributes));

            return this;
        }

        public Displayable InPath(string path)
        {
            this.path = path;

            return this;
        }

        public Displayable Editable(bool isEditable)
        {
            this.isEditable = isEditable;

            return this;
        }

        public Displayable SwallowExceptions()
        {
            swallowExceptions = true;

            return this;
        }

        public Displayable SwallowExceptions(bool swallow)
        {
            swallowExceptions = swallow;

            return this;
        }

        /// <summary>Control whether the displayable will throw exceptions when no displayable with a matching name is found.</summary>
        /// <param name="isOptional">Optional is true by default.</param>
        /// <returns>The same <see cref="Displayable"/> object.</returns>
        public Displayable Optional(bool isOptional = true)
        {
            this.isOptional = isOptional;

            return this;
        }

        public override string ToString()
        {
            var previousWriter = Html.ViewContext.Writer;
            try
            {
                using (var writer = new StringWriter())
                {
                    Html.ViewContext.Writer = writer;
                    
                    Render(writer);

                    return writer.ToString();
                }
            }
            finally
            {
                Html.ViewContext.Writer = previousWriter;
            }
        }

        public string ToHtmlString()
        {
            return ToString();
        }

        internal void Render(TextWriter writer)
        {
            if (!string.IsNullOrEmpty(path))
                CurrentItem = ItemUtility.WalkPath(CurrentItem, path);

            if (CurrentItem == null)
                return;

            if (swallowExceptions)
            {
                try
                {
                    RenderDisplayable(writer);
                }
                catch (Exception ex)
                {
                    logger.Debug(ex);
                }
            }
            else
                RenderDisplayable(writer);
        }

        private void RenderDisplayable(TextWriter writer)
        {
            var displayable = DefinitionMap.Instance.GetOrCreateDefinition(CurrentItem).Displayables.FirstOrDefault(d => d.Name == propertyName);

            if (displayable == null)
            {
                if (isOptional || swallowExceptions)
                    return;

                throw new N2Exception("No attribute implementing IDisplayable found on the property '{0}' of the item #{1} of type {2}", propertyName, CurrentItem.ID, CurrentItem.GetContentType());
            }

            if (Wrapper != null)
                writer.Write(Wrapper.ToString(TagRenderMode.StartTag));

            var ctx = new RenderingContext
            { 
                Content = CurrentItem, 
                Displayable = displayable, 
                Html = Html, 
                PropertyName = propertyName, 
                IsEditable = isEditable && ControlPanelExtensions.GetControlPanelState(Html).IsFlagSet(ControlPanelState.DragDrop) 
            };
            Html.ResolveService<DisplayableRendererSelector>()
                .Render(ctx, writer);

            if (Wrapper != null)
                writer.Write(Wrapper.ToString(TagRenderMode.EndTag));
        }
    }
}
