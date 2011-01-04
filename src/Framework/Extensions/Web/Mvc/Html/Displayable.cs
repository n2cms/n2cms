using System;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2.Details;
using System.Web.Routing;
using System.Diagnostics;

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

		public override string ToString()
		{
			var previousWriter = Html.ViewContext.Writer;
			try
			{
				using (var writer = new StringWriter())
				{
					Html.ViewContext.Writer = writer;
					
					Render(Html);

					return writer.ToString();
				}
			}
			finally
			{
				Html.ViewContext.Writer = previousWriter;
			}
		}

        internal void Render(HtmlHelper helper)
        {
            if (!string.IsNullOrEmpty(path))
                CurrentItem = ItemUtility.WalkPath(CurrentItem, path);

            if (CurrentItem == null)
                return;

			if (swallowExceptions)
			{
				try
				{
					RenderDisplayable(helper);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
			else
				RenderDisplayable(helper);
        }

		private void RenderDisplayable(HtmlHelper helper)
		{
			var displayable = Display.GetDisplayableAttribute(propertyName, CurrentItem, swallowExceptions);
			if (displayable == null) return;

			if (Wrapper != null)
				helper.ViewContext.Writer.Write(Wrapper.ToString(TagRenderMode.StartTag));

			var referencedItem = CurrentItem[propertyName] as ContentItem;
			if (referencedItem != null)
			{
				var adapter = Adapters.ResolveAdapter<MvcAdapter>(referencedItem);
				adapter.RenderTemplate(helper, referencedItem);
			}
			else
			{
				var container = CreateContainer(displayable);
				container.RenderControl(new HtmlTextWriter(helper.ViewContext.Writer));
			}
			if (Wrapper != null)
				helper.ViewContext.Writer.Write(Wrapper.ToString(TagRenderMode.EndTag));
		}

		private ViewUserControl CreateContainer(IDisplayable displayable)
		{
			var viewData = new ViewDataDictionary(CurrentItem);

			var container = new ViewUserControl
			                	{
									Page = (Html.ViewContext.View is Control) ? ((Control)Html.ViewContext.View).Page : null,
			                		ViewData = viewData,
			                	};
			displayable.AddTo(CurrentItem, propertyName, container);

			return container;
		}
	}
}