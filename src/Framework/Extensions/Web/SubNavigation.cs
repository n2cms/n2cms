using N2.Details;
using N2.Engine;
using N2.Integrity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Web.Mvc;

namespace N2.Web
{
	public enum ParentDisplayOptions
	{
		CurrentItemInTree,
		ListParents,
		NestUnderParents
	}

	/// <summary>
	/// Provides navigation across child and sibiling pages. Ideal for sidebars. 
	/// </summary>
	[PartDefinition(Title = "Sub-Navigation", IconUrl = "{IconsUrl}/bullet.png")]
	[WithEditableTitle]
	public class SubNavigation : ContentItem, N2.Definitions.IPart
	{

		#region Hierarchy Options

		[EditableNumber("Num Child Levels", 100)]
		public virtual int NumChildLevels
		{
			get { return GetDetail("Levels", 1); }
			set { SetDetail("Levels", value); }
		}

		[EditableNumber("Start from Level", 100)]
		public virtual int StartFromLevel
		{
			get { return GetDetail("StartLevel", 1); }
			set { SetDetail("StartLevel", value); }
		}

		//[EditableCheckBox("", 200)]
		//public virtual bool CurrentItemInTree
		//{
		//	get { return GetDetail("ci_isLI", false); }
		//	set { SetDetail("ci_isLI", value); }
		//}

		//[EditableCheckBox("Also show parent(s)", 200)]
		//public virtual bool PushParents
		//{
		//	get { return GetDetail("includeparents", false); }
		//	set { SetDetail("includeparents", value); }
		//}

		//[EditableCheckBox("Also nest parent(s)", 200)]
		//public virtual bool NestParents
		//{
		//	get { return GetDetail("nestparents", false); }
		//	set { SetDetail("nestparents", value); }
		//}

		#endregion 

		#region Title Display

		[EditableEnum("TitleMode", 300, typeof(HeadingLevel), ContainerName = "Title")]
		public virtual HeadingLevel TitleMode
		{
			get { return GetDetail("TitleMode", HeadingLevel.H1); }
			set { SetDetail("TitleMode", value); }
		}

		[EditableCheckBox("Show current page title instead", 250, ContainerName = "Title")]
		public virtual bool CurrentItemAsTitle
		{
			get { return GetDetail("ci_asTitle", false); }
			set { SetDetail("ci_asTitle", value); }
		}

		#endregion 


	}

	/// <summary>
	/// Renders the SubNavigation part using the ASP.NET MVC framework.
	/// </summary>
	[Adapts(typeof (SubNavigation))]
	public class SubNavigationMvcAdapter : MvcAdapter
	{
		public override void RenderTemplate(System.Web.Mvc.HtmlHelper html, ContentItem model)
		{
			if (!(model is SubNavigation))
				throw new ArgumentException("model");
			html.ViewContext.Writer.Write(Engine.Resolve<INavigationAdapter>().GetHtml(model as SubNavigation));
		}
	}

	public interface INavigationAdapter
	{
		string GetHtml(SubNavigation sn);
	}


	[Service(typeof(INavigationAdapter))]
	public class NavigationAdapter : INavigationAdapter
	{

		private const string LI_TEMPLATE = @"<li class=""{0}""><a href=""{2}"">{1}</a></li>";
		private const string LI_TEMPLATE_SELECTED = @"<li class=""{0}"">{1}</li>";
		private const string BEGIN_UL = "\n<ul>";
		private const string END_UL = "</ul>";
		private const string liClass = "nav-item";
		private const string liClassSel = "nav-item nav-selected";

		protected class NavNode
		{
			public NavNode(ContentItem item)
			{
				Item = item;
			}

			public ContentItem Item { get; protected set; }
			public NavNode Parent { get; protected set; }
			public List<NavNode> Children { get; protected set; }

			public void Expand()
			{
				if (Children == null)
					Children = new List<NavNode>();
				foreach (var item in Item.GetChildren())
					Children.Add(new NavNode(item));
			}

			/// <summary>
			/// Writes a HTML &lt;li&gt; tag for this list item. 
			/// </summary>
			/// <returns></returns>
			public string ToHtmlLi()
			{
				var sb = new StringBuilder();
				if (Item.ID != Content.Current.Page.ID)
					sb.AppendFormat(LI_TEMPLATE, liClass, Item.Title, Item.Url).Append('\n');
				else
					sb.AppendFormat(LI_TEMPLATE_SELECTED, liClassSel, Item.Title).Append('\n');

				return sb.ToString();
			}

			public override string ToString()
			{
				return string.Format("[{0}] {{ {1} }}", Item.Name, String.Join(",\n", Children ?? new List<NavNode>()));
			}
		}

		public string GetHtml(SubNavigation sn)
		{
			StringBuilder sb = new StringBuilder();
			var ci = Content.Current.Page;
			if (sn.CurrentItemAsTitle)
				sb.Append(HeadingLevelUtility.DoTitle(sn.TitleMode, ci.Title));

			if (ci.Parent != null)
				ProcessChildren(ci.Parent, sb, sn.NumLevels);
			else 
				ProcessChildren(ci, sb, sn.NumLevels);

			return sb.ToString();
		}

		private void ProcessChildren(ContentItem ci, StringBuilder sb, int recursionLimit)
		{
			var items = ci.GetChildren().Where(p => p.IsPage).ToList();
			if (items.Count > 0)
			{
				sb.Append(BEGIN_UL).Append('\n');
				foreach (var cp in items)
				{
					
					if (recursionLimit > 0)
						ProcessChildren(cp, sb, recursionLimit-1);
				}
				sb.Append(END_UL);
			}
		}
	}
	

}
