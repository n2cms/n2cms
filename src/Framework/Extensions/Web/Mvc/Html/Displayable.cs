using System;
using System.Linq;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2.Details;
using System.Web.Routing;
using System.Diagnostics;
using N2.Engine;
using System.Collections.Generic;
using N2.Web.Rendering;

namespace N2.Web.Mvc.Html
{
	[Service]
	public class DisplayableRendererSelector
	{
		IDisplayableRenderer[] renderers;
		Dictionary<Type, IDisplayableRenderer> rendererForType = new Dictionary<Type, IDisplayableRenderer>();

		public DisplayableRendererSelector(IDisplayableRenderer[] renderers)
		{
			this.renderers = renderers
				.OrderByDescending(r => Utility.InheritanceDepth(r.HandledDisplayableType))
				.ThenByDescending(r => Utility.InheritanceDepth(r.GetType()))
				.ToArray();
		}

		public virtual IDisplayableRenderer ResolveRenderer(Type displayableType)
		{
			IDisplayableRenderer renderer;
			if (rendererForType.TryGetValue(displayableType, out renderer))
				return renderer;

			var temp = new Dictionary<Type, IDisplayableRenderer>(rendererForType);
			temp[displayableType] = renderer = renderers.FirstOrDefault(r => r.HandledDisplayableType.IsAssignableFrom(displayableType));
			rendererForType = temp;

			return renderer;
		}

		public void Render(RenderingContext context, TextWriter writer)
		{
			ResolveRenderer(context.Displayable.GetType()).Render(context, writer);
		}
	}

	public class Displayable : ItemHelper
	{
		readonly string propertyName;
		string path;
		bool swallowExceptions;

        public Displayable(HtmlHelper helper, string propertyName, ContentItem currentItem)
            : base(helper, currentItem)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            this.propertyName = propertyName;
        }

        protected System.Web.Mvc.TagBuilder Wrapper { get; set; }
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
            Wrapper = new System.Web.Mvc.TagBuilder(tagName);
            Wrapper.MergeAttributes(new RouteValueDictionary(attributes));

            return this;
        }

		public Displayable InPath(string path)
		{
			this.path = path;

			return this;
		}

		public Displayable SwallowExceptions()
		{
			swallowExceptions = true;

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
					
					Render();

					return writer.ToString();
				}
			}
			finally
			{
				Html.ViewContext.Writer = previousWriter;
			}
		}

        internal void Render()
        {
            if (!string.IsNullOrEmpty(path))
                CurrentItem = ItemUtility.WalkPath(CurrentItem, path);

            if (CurrentItem == null)
                return;

			if (swallowExceptions)
			{
				try
				{
					RenderDisplayable();
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
			else
				RenderDisplayable();
        }

		private void RenderDisplayable()
		{
			var displayable = Display.GetDisplayableAttribute(propertyName, CurrentItem, swallowExceptions);
			if (displayable == null) return;

			var writer = Html.ViewContext.Writer;
			if (Wrapper != null)
				writer.Write(Wrapper.ToString(TagRenderMode.StartTag));

			var ctx = new RenderingContext { Content = CurrentItem, Displayable = displayable, Html = Html, PropertyName = propertyName };
			Html.ResolveService<DisplayableRendererSelector>()
				.Render(ctx, writer);

			if (Wrapper != null)
				writer.Write(Wrapper.ToString(TagRenderMode.EndTag));
		}
	}
}