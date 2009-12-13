using System;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Web.Mvc.Html
{
	public class Displayable : ItemHelper
	{
		private readonly ITemplateRenderer _templateRenderer;

		private readonly string _detailName;
		private string _path;
		private bool _swallowExceptions;

        public Displayable(ViewContext viewContext, ITemplateRenderer templateRenderer, string detailName)
			: base(viewContext)
		{
			if (templateRenderer == null) throw new ArgumentNullException("templateRenderer");

			_templateRenderer = templateRenderer;
			_detailName = detailName;
		}

        public Displayable(ViewContext viewContext, ITemplateRenderer templateRenderer, string detailName, ContentItem currentItem)
            : base(viewContext, currentItem)
        {
            if (templateRenderer == null) throw new ArgumentNullException("templateRenderer");

            _templateRenderer = templateRenderer;
            _detailName = detailName;
        }

		public string Value
		{
			get
			{
				try
				{
					return Convert.ToString(CurrentItem[_detailName]);
				}
				catch
				{
					if (_swallowExceptions)
						return String.Empty;

					throw;
				}
			}
		}

		public Displayable InPath(string path)
		{
			_path = path;

			return this;
		}

		public Displayable SwallowExceptions()
		{
			_swallowExceptions = true;

			return this;
		}

		public override string ToString()
		{
			var display = new Display
			              	{
			              		CurrentItem = CurrentItem,
			              		PropertyName = _detailName,
			              		SwallowExceptions = _swallowExceptions
			              	};

			if (!String.IsNullOrEmpty(_path))
				display.Path = _path;

			try
			{
				if (display.Displayable == null)
					return Value;

				var container = CreateContainer(display);

				using (var writer = new StringWriter())
				{
					container.RenderControl(new HtmlTextWriter(writer));

					return writer.ToString();
				}
			}
			catch (Exception)
			{
				if (_swallowExceptions)
					return String.Empty;

				throw;
			}
		}

		private ViewUserControl CreateContainer(Display display)
		{
			var item = CurrentItem[_detailName] as ContentItem;

			if (item != null)
				return CreateContentItemContainer(item, display);

			var viewData = new ViewDataDictionary(display.CurrentItem);

			var container = new ViewUserControl
			                	{
			                		Page = (ViewContext.View is Control) ? ((Control) ViewContext.View).Page : null,
			                		ViewData = viewData,
			                	};
			display.Displayable.AddTo(display.CurrentItem, _detailName, container);

			return container;
		}

		protected ViewUserControl CreateContentItemContainer(ContentItem item, Display display)
		{
			var userControl = new ViewUserControl();

			userControl.Controls.Add(new LiteralControl(_templateRenderer.RenderTemplate(item, ViewContext)));

			return userControl;
		}
	}
}