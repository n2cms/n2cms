using System;
using System.Web.UI;
using N2.Definitions;
using N2.Web.UI.WebControls;

namespace N2.Web.UI
{
	[Obsolete("The [TabPanel] is now renamed to [TabContainer] attribute to avoid confusion with a web control with a similar name.", true)]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class TabPanelAttribute : TabContainerAttribute
	{
		public TabPanelAttribute(string name, string tabText, int sortOrder)
			: base(name, tabText, sortOrder)
		{
		}
	}

	/// <summary>
	/// Defines a tab panel that can be used to contain editor controls.
	/// </summary>
	/// <example>
	///     [N2.Web.UI.TabContainer("default", "Default", 100)] // editables with the ContainerName="default" will be placed in a tab with the name "Default"
	///     public class MyPage : N2.ContentItem
	///     {
	///         [N2.Details.EditableFreeTextArea("Text", 110, ContainerName="default")]
	///         public virtual string Text { get; set; }
	///     }
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class TabContainerAttribute : EditorContainerAttribute
	{
		string tabText;
		bool registerTabCss;
		string cssClass;

		public TabContainerAttribute(string name)
			: base(name, 0)
		{
			TabText = name;
		}

		public TabContainerAttribute(string name, string tabText, int sortOrder)
			: base(name, sortOrder)
		{
			TabText = tabText;
		}

		/// <summary>Gets or sets this panel's tab text.</summary>
		public string TabText
		{
			get { return tabText; }
			set { tabText = value; }
		}

		/// <summary>Gets or sets wether default styles should be registered.</summary>
		[Obsolete]
		public bool RegisterTabCss
		{
			get { return registerTabCss; }
			set { registerTabCss = value; }
		}

		/// <summary>
		/// This needs to contain the css class tabPanel to receive default styles.
		/// </summary>
		public string CssClass
		{
			get { return cssClass; }
			set { cssClass = value; }
		}

		/// <summary>Adds the tab panel to a parent container and returns it.</summary>
		/// <param name="container">The parent container onto which to add the container defined by this interface.</param>
		/// <returns>The newly added tab panel.</returns>
		public override Control AddTo(Control container)
		{
			TabPanel p = new TabPanel();
			p.ID = Name;
			p.TabText = GetLocalizedText("TabText") ?? TabText;
			p.RegisterTabCss = registerTabCss;
			if (string.IsNullOrEmpty(CssClass))
			{
				p.CssClass = "tabPanel primaryTabs";

				var parentTab = container.Closest(c => c is TabPanel || c is ItemEditor);
				if (parentTab != null)
				{
					p.CssClass = "tabPanel " + parentTab.ClientID + "Tabs";
				}
			}
			else
				p.CssClass = CssClass;
			container.Controls.Add(p);
			return p;
		}
	}
}