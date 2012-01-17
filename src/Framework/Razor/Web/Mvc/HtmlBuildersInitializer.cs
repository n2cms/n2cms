using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Plugin;
using N2.Web;
using N2.Collections;
using N2;
using N2.Web.Mvc;
using N2.Definitions;
using N2.Web.Rendering;
using N2.Definitions.Runtime;
using N2.Details;
using N2.Web.Mvc.Html;
using System.Web.Mvc;

namespace N2.Web.Mvc
{
	/// <summary>
	/// This is all a rather convoluted way of supporting IHtmlString builders without requiring
	/// .NET4 for N2.dll. This might very well disappear in favor for multiple targets.
	/// </summary>
	[AutoInitialize]
	public class HtmlBuildersInitializer : IPluginInitializer
	{
		static HtmlBuildersInitializer()
		{
			Link.LinkFactory = (linkTarget) => new LinkBuilder(linkTarget);
			Tree.TreeFactory = (hierarchy) => new TreeBuilder(hierarchy);
			RenderHelper.RendererFactory = new UnencodedDisplayRendererFactory();
			RegisterHelper.RendererFactory = new UnencodedRegisteringDisplayRendererFactory();
            DisplayExtensions.DisplayableFactory = (html, propertyName, item) => new UnencodedDisplayable(html, propertyName, item);
		}

		#region IPluginInitializer Members

		public void Initialize(N2.Engine.IEngine engine)
		{
			// do nothing, work already done
		}

		#endregion

		#region class ControlPanelBuilder
		public class ControlPanelBuilder : ControlPanelExtensions.ControlPanelHelper, IHtmlString
		{
		}
		#endregion

		#region class LinkBuilder
		class LinkBuilder : Link, IHtmlString
		{
			#region Constructor

			public LinkBuilder(ILink link)
			{
				UpdateFrom(link);
			}

			#endregion
		}
		#endregion

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

		#region class UnencodedRegisteringDisplayRendererFactory
		class UnencodedRegisteringDisplayRendererFactory : RegisterHelper.RegisteringDisplayRendererFactory
		{
			public override RegisteringDisplayRenderer<T> Create<T>(N2.Web.Rendering.RenderingContext context, N2.Definitions.Runtime.ContentRegistration re)
			{
				return new UnencodedRegisteringDisplayRenderer<T>(context, re);
			}

			class UnencodedRegisteringDisplayRenderer<T> : RegisteringDisplayRenderer<T>, IHtmlString
				where T : IEditable
			{
				public UnencodedRegisteringDisplayRenderer(RenderingContext context, ContentRegistration registration)
					: base(context, registration)
				{
				}
			}
		}
		#endregion

		#region class UnencodedDisplayRendererFactory
		class UnencodedDisplayRendererFactory : RenderHelper.DisplayRendererFactory
		{
			public override DisplayRenderer<T> Create<T>(RenderingContext context)
			{
				return new UnencodedDisplayRenderer<T>(context);
			}

			class UnencodedDisplayRenderer<T> : DisplayRenderer<T>, IHtmlString where T : IDisplayable
			{
				public UnencodedDisplayRenderer(RenderingContext context)
					: base(context)
				{
				}
			}
		}
		#endregion

        #region class UnencodedDisplayable
        class UnencodedDisplayable : Displayable , IHtmlString
        {
            public UnencodedDisplayable(HtmlHelper helper, string propertyName, ContentItem currentItem)
                : base(helper, propertyName, currentItem)
            {
            }
        }
        #endregion
    }
}