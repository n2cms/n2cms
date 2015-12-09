using System.Web.Mvc;
using System.Linq;
using N2.Details;
using N2.Web.Mvc.Html;
using N2.Definitions;
using System.Web.UI;

namespace N2.Web.Rendering
{
    public class ContentRenderingContext
    {
        /// <summary>The content beeing rendered.</summary>
        public ContentItem Content { get; set; }

        /// <summary>The container control where the content is beeing rendered. This property may be null.</summary>
        public Control Container { get; set; }
        
        /// <summary>The html helper. This property may be null.</summary>
        public HtmlHelper Html { get; set; }
    }

    public class RenderingContext : ContentRenderingContext
    {
        public string PropertyName { get; set; }
        public IDisplayable Displayable { get; set; }
        public bool IsEditable { get; set; }

        public static RenderingContext Create(HtmlHelper html, string propertyName, bool isEditable = true)
        {
            var context = new RenderingContext();
			context.IsEditable = isEditable;
            context.Content = html.CurrentItem();
            var template = html.ResolveService<ITemplateAggregator>().GetTemplate(context.Content);
            if (template != null)
                context.Displayable = template.Definition.Displayables.FirstOrDefault(d => d.Name == propertyName);
            context.Html = html;
            context.PropertyName = propertyName;
            return context;
        }
    }
}
