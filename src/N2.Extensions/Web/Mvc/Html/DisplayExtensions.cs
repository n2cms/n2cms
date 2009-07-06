using System;
using System.IO;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.UI;
using N2.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Web.Mvc.Html
{
	public static class DisplayExtensions
	{
		public static Displayable Display<TItem>(this IItemContainer<TItem> container, string detailName)
			where TItem : ContentItem
		{
			return new Displayable(Context.Current.Resolve<ITemplateRenderer>(), container, detailName);
		}

		public static Displayable Display<TItem>(this IItemContainer<TItem> container,
		                                         Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
			var member = (MemberExpression) expression.Body;

			return container.Display(member.Member.Name);
		}

		public static Displayable Display<TItem>(this IItemContainer container, TItem item, string detailName)
			where TItem : ContentItem
		{
			return new Displayable(Context.Current.Resolve<ITemplateRenderer>(), container, detailName, item);
		}

		public static Displayable Display<TItem>(this IItemContainer container, TItem item, Expression<Func<TItem, object>> expression)
			where TItem : ContentItem
		{
			var member = (MemberExpression) expression.Body;

			return container.Display(item, member.Member.Name);
		}
	}

	public class Displayable : ItemHelper
	{
		private readonly ITemplateRenderer _templateRenderer;

		private readonly string _detailName;
		private string _path;
		private bool _swallowExceptions;

		public Displayable(ITemplateRenderer templateRenderer, IItemContainer container, string detailName)
			: base(container)
		{
			if (templateRenderer == null)
				throw new ArgumentNullException("templateRenderer");

			_templateRenderer = templateRenderer;
			_detailName = detailName;
		}

		public Displayable(ITemplateRenderer templateRenderer, IItemContainer container, string detailName, ContentItem item)
			: base(container, item)
		{
			if (templateRenderer == null)
				throw new ArgumentNullException("templateRenderer");

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

			var container = new ViewUserControl
			                	{
			                		Page = (Container is Control) ? ((Control) Container).Page : null,
			                		ViewData = new ViewDataDictionary((CurrentItem[_detailName] as ContentItem)
			                		                                  ?? display.CurrentItem),
			                	};
			display.Displayable.AddTo(display.CurrentItem, _detailName, container);

			return container;
		}

		protected ViewUserControl CreateContentItemContainer(ContentItem item, Display display)
		{
			var userControl = new ViewUserControl();

			userControl.Controls.Add(new LiteralControl(_templateRenderer.RenderTemplate(item, Container)));

			return userControl;
		}
	}
}