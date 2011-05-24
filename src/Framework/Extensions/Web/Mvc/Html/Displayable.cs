using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using N2.Definitions.Static;
using N2.Web.Rendering;
using N2.Web.UI;

namespace N2.Web.Mvc.Html
{
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

		public Displayable SwallowExceptions(bool swallow)
		{
			swallowExceptions = swallow;

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
			var displayable = DefinitionMap.Instance.GetOrCreateDefinition(CurrentItem).Displayables.FirstOrDefault(d => d.Name == propertyName);

			if (displayable == null)
			{
				if (!swallowExceptions)
					throw new N2Exception("No attribute implementing IDisplayable found on the property '{0}' of the item #{1} of type {2}", propertyName, CurrentItem.ID, CurrentItem.GetContentType());
				return;
			}

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