using System.Web.Mvc;
using System.Web;
using N2.Details;
using N2.Definitions;
using N2.Web.Rendering;
using N2.Definitions.Runtime;
using N2.Collections;

namespace N2.Web.Mvc
{
	/// <remarks>This code is here since it has dependencies on ASP.NET 3.0 which isn't a requirement for N2 in general.</remarks>
	public class DynamicContentHelper : ContentHelper
	{
		public DynamicContentHelper(HtmlHelper html)
			: base(html)
		{
		}

		public dynamic Display
		{
			get { return new DisplayHelper { Html = Html, Current = CurrentItem }; }
		}

		public dynamic Has
		{
			get { return new HasValueHelper(HasValue); }
		}

		public dynamic Data
		{
			get
			{
				if (CurrentItem == null)
					return new DataHelper(() => CurrentItem);

				string key = "DataHelper" + CurrentItem.ID;
				var data = Html.ViewContext.ViewData[key] as DataHelper;
				if (data == null)
					Html.ViewContext.ViewData[key] = data = new DataHelper(() => CurrentItem);
				return data;
			}
		}

		public override RenderHelper Render
		{
			get { return new DynamicRenderHelper { Html = Html, Content = CurrentItem }; }
		}

		#region class DynamicRenderHelper
		class DynamicRenderHelper : RenderHelper
		{
			class UnencodedDisplayRenderer<T> : DisplayRenderer<T>, IHtmlString where T : IDisplayable
			{
				public UnencodedDisplayRenderer(RenderingContext context)
					: base(context)
				{
				}
			}

			protected override DisplayRenderer<T> CreateDisplayRenderer<T>(RenderingContext context)
			{
				return new UnencodedDisplayRenderer<T>(context);
			}
		}
		#endregion

		public override RegisterHelper Register
		{
			get { return new DynamicRegisterHelper(Html); }
		}

		#region class DynamicRegisterHelper
		class DynamicRegisterHelper : RegisterHelper
		{
			public DynamicRegisterHelper(HtmlHelper html)
				: base(html)
			{
			}

			class UnencodedRegisteringDisplayRenderer<T> : RegisteringDisplayRenderer<T>, IHtmlString
				where T : IEditable
			{
				public UnencodedRegisteringDisplayRenderer(RenderingContext context, ContentRegistration registration)
					: base(context, registration)
				{
				}
			}

			protected override RegisteringDisplayRenderer<T> CreateRegisteringDisplayRenderer<T>(RenderingContext context, Definitions.Runtime.ContentRegistration re)
			{
				return new UnencodedRegisteringDisplayRenderer<T>(context, re);
			}
		}
		#endregion

		protected override Link CreateLink(ContentItem item)
		{
			return new LinkBuilder(item);
		}

		#region class LinkBuilder
		class LinkBuilder : Link, IHtmlString
		{
			#region Constructor

			public LinkBuilder()
				: base(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
			{
			}

			public LinkBuilder(string text, string href)
				: base(text, string.Empty, string.Empty, href, string.Empty)
			{
			}

			public LinkBuilder(string text, string title, string target, string href)
				: base(text, title, target, href, string.Empty)
			{
			}

			public LinkBuilder(string text, string title, string target, string href, string className)
				: base(text, title, target, href, className)
			{
			}

			public LinkBuilder(ILink link)
			{
				UpdateFrom(link);
			}

			public LinkBuilder(ContentItem item)
				: base(item, string.Empty)
			{
			}

			public LinkBuilder(ContentItem item, string className)
				: base(item.Title, string.Empty, string.Empty, item.Url, className)
			{
				if (item is ILink)
					UpdateFrom(item as ILink);
			}

			#endregion
		}
		#endregion

		protected override Tree CreateTree(Collections.HierarchyBuilder hierarchy)
		{
			return new TreeBuilder(hierarchy);
		}

		#region class TreeBuilder
		class TreeBuilder : Tree, IHtmlString
		{
			#region Constructor

			public TreeBuilder(HierarchyBuilder builder)
				: base(builder)
			{
			}

			public TreeBuilder(HierarchyNode<ContentItem> root)
				: base(root)
			{
			}

			#endregion
		}
		#endregion
	}
}