using System;
using System.Web.UI;
using N2.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>Class applicable attribute used to add a name editor. The name represents the URL slug for a certain content item.</summary>
    /// <example>
    /// [N2.Details.WithEditableName("Address name", 20)]
    /// public abstract class AbstractBaseItem : N2.ContentItem 
    /// {
    ///	}
    /// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class WithEditableNameAttribute : AbstractEditableAttribute, IWritingDisplayable, IDisplayable
    {
    	private char? whitespaceReplacement = '-';
		private bool? toLower;
		private bool? ascii = false;
		private bool? showKeepUpdated;
		private string keepUpdatedText = "";
		private string keepUpdatedToolTip = "Keep the name up to date";

    	/// <summary>
		/// Creates a new instance of the WithEditableAttribute class with default values.
		/// </summary>
		public WithEditableNameAttribute()
			: this("URI Name", -10)
		{
		}
		/// <summary>
		/// Creates a new instance of the WithEditableAttribute class with default values.
		/// </summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public WithEditableNameAttribute(string title, int sortOrder)
			: base(title, "Name", sortOrder)
		{
		}

		/// <summary>Gets or sets the character that replaces whitespace when updating the name (default is '-').</summary>
    	public char? WhitespaceReplacement
    	{
    		get { return whitespaceReplacement; }
    		set { whitespaceReplacement = value; }
    	}

		/// <summary>Gets or sets whether names should be made lowercase.</summary>
    	public bool? ToLower
    	{
    		get { return toLower; }
    		set { toLower = value; }
    	}

		/// <summary>Gets or sets wether non-ascii characters will be removed from the name.</summary>
		[Obsolete]
		public bool? Ascii
		{
			get { return ascii; }
			set { ascii = value; }
		}

		/// <summary>Allow editor to choose wether to update name automatically.</summary>
		public bool? ShowKeepUpdated
		{
			get { return showKeepUpdated; }
			set { showKeepUpdated = value; }
		}

		/// <summary>The text on the keep updated check box.</summary>
		public string KeepUpdatedText
		{
			get { return keepUpdatedText; }
			set { keepUpdatedText = value; }
		}

		/// <summary>The tool tip on the keep updated check box.</summary>
		public string KeepUpdatedToolTip
		{
			get { return keepUpdatedToolTip; }
			set { keepUpdatedToolTip = value; }
		}

		/// <summary>Sets focus on the name editor.</summary>
		public bool Focus { get; set; }

    	public override bool UpdateItem(ContentItem item, Control editor)
		{
			NameEditor ne = (NameEditor)editor;
			if (item.Name != ne.Text)
			{
				item.Name = ne.Text;
				return true;
			}
			return false;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			NameEditor ne = (NameEditor)editor;
			ne.Text = item.Name;
			ne.Prefix = "/";
			ne.Suffix = item.Extension;
            try
            {
                if (Context.UrlParser.StartPage == item || item.Parent == null)
                {
                    ne.Prefix = "";
                    ne.Suffix = "";
                }
                else if (Context.UrlParser.StartPage != item.Parent)
                {
                    string parentUrl = item.Parent.Url;
                    if (!parentUrl.Contains("?"))
                    {
                        int extensionIndex = parentUrl.Length;
						if (!string.IsNullOrEmpty(item.Parent.Extension))
							extensionIndex = parentUrl.IndexOf(item.Extension, StringComparison.InvariantCultureIgnoreCase);
                        string prefix = parentUrl.Substring(0, extensionIndex) + "/";
                        if (prefix.Length > 60)
                            prefix = prefix.Substring(0, 50) + ".../";
                        ne.Prefix = prefix;
                    }
                }
            }
            catch (Exception)
            {
            }
		}

		protected override Control AddEditor(Control container)
		{
			NameEditor ne = new NameEditor();
			ne.ID = Name;
			ne.CssClass = "nameEditor";
			ne.WhitespaceReplacement = WhitespaceReplacement;
			ne.ToLower = ToLower;
			ne.ShowKeepUpdated = ShowKeepUpdated;
			ne.KeepUpdated.Text = KeepUpdatedText;
			ne.KeepUpdated.ToolTip = KeepUpdatedToolTip;
            ne.Placeholder(GetLocalizedText("FromDatePlaceholder") ?? Placeholder);
			container.Controls.Add(ne);
			if (Focus) ne.Focus();
			return ne;
		}

		#region IWritingDisplayable Members

		public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
		{
			writer.Write(item[propertyName]);
		}

		#endregion
	}
}
