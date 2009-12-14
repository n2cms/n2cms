using System;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2.Details;
using System.Web.Routing;

namespace N2.Web.Mvc.Html
{
	public class Displayable : ItemHelper
	{
		private readonly ITemplateRenderer _templateRenderer;

		readonly string propertyName;
		string path;
		bool swallowExceptions;

        public Displayable(ViewContext viewContext, ITemplateRenderer templateRenderer, string propertyName, ContentItem currentItem)
            : base(viewContext, currentItem)
        {
            if (templateRenderer == null) throw new ArgumentNullException("templateRenderer");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            this._templateRenderer = templateRenderer;
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
            using (var writer = new StringWriter())
            {
                Render(writer);

                return writer.ToString();
            }
		}

        internal void Render(TextWriter writer)
        {
            if (!string.IsNullOrEmpty(path))
                CurrentItem = ItemUtility.WalkPath(CurrentItem, path);

            if (CurrentItem == null)
                return;

            try
            {
                var displayable = Display.GetDisplayableAttribute(propertyName, CurrentItem, !swallowExceptions);
                if (displayable == null) return;

                var container = CreateContainer(displayable);

                if (Wrapper != null)
                    writer.Write(Wrapper.ToString(TagRenderMode.StartTag));

                container.RenderControl(new HtmlTextWriter(writer));

                if (Wrapper != null)
                    writer.Write(Wrapper.ToString(TagRenderMode.EndTag));
            }
            catch (Exception)
            {
                if (swallowExceptions)
                    return;

                throw;
            }
        }

		private ViewUserControl CreateContainer(IDisplayable displayable)
		{
			var item = CurrentItem[propertyName] as ContentItem;
			if (item != null)
                return CreateContentItemContainer(item, displayable);

			var viewData = new ViewDataDictionary(CurrentItem);

			var container = new ViewUserControl
			                	{
			                		Page = (ViewContext.View is Control) ? ((Control) ViewContext.View).Page : null,
			                		ViewData = viewData,
			                	};
			displayable.AddTo(CurrentItem, propertyName, container);

			return container;
		}

        protected ViewUserControl CreateContentItemContainer(ContentItem item, IDisplayable displayable)
		{
			var userControl = new ViewUserControl();

			userControl.Controls.Add(new LiteralControl(_templateRenderer.RenderTemplate(item, ViewContext)));

			return userControl;
		}
	}
}