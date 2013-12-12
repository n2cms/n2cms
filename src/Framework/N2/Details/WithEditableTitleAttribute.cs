using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;

namespace N2.Details
{
    /// <summary>Class applicable attribute used to add a title editor.</summary>
    /// <example>
    /// [N2.Details.WithEditableTitle("Menu text", 10)]
    /// public abstract class AbstractBaseItem : N2.ContentItem 
    /// {
    ///	}
    /// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class WithEditableTitleAttribute : AbstractEditableAttribute, IWritingDisplayable, IDisplayable
    {
		private bool focus = true;

		/// <summary>
		/// Creates a new instance of the WithEditableAttribute class with default values.
		/// </summary>
		public WithEditableTitleAttribute()
			: this("Title", -20) 
		{
		}

		/// <summary>
		/// Creates a new instance of the WithEditableAttribute class with default values.
		/// </summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public WithEditableTitleAttribute(string title, int sortOrder)
			: base(title, "Title", sortOrder)
		{
			_allowHtmlLoaded = false;
			Required = true;
		}

		/// <summary>
		/// Creates a new instance of the WithEditableAttribute class with default values.
		/// </summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		/// <param name="allowHtml">Determines whether the title tag may contain HTML (otherwise, any HTML text content will be escaped)</param>
		public WithEditableTitleAttribute(string title, int sortOrder, bool allowHtml)
			: base(title, "Title", sortOrder)
		{
			AllowHtml = allowHtml;
			Required = true;
		}

		private bool _allowHtmlLoaded;
		private bool _allowHtmlValue;

		public bool AllowHtml
		{
			get
			{
				if (!_allowHtmlLoaded)
				{
					_allowHtmlValue = N2.Context.Current.Resolve<EngineSection>().DefaultHtmlEscape;
					_allowHtmlLoaded = true;
				}
				return _allowHtmlValue;
			}
			set
			{
				_allowHtmlLoaded = true;
				_allowHtmlValue = value;
			}
		}
			

		/// <summary>Gets or sets whether the title editor should receive focus.</summary>
		public bool Focus
		{
			get { return focus; }
			set { focus = value; }
		}

		public string CssClass { get; set; }

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			var tb = (TextBox)editor;
			if (item.Title == tb.Text) return false;
			item.Title = tb.Text;
			return true;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			TextBox titleTextBox = (TextBox)editor;
			titleTextBox.Text = item.Title;

			if (Focus)
                titleTextBox.PreRender += delegate { titleTextBox.Focus(); };
		}

		protected override Control AddEditor(Control container)
		{
			TextBox tb = new TextBox();
			tb.ID = Name;
			tb.MaxLength = 250;
			tb.CssClass = "titleEditor input-xxlarge";
            tb.Placeholder(GetLocalizedText("FromDatePlaceholder") ?? Placeholder);
			container.Controls.Add(tb);
			return tb;
		}

		#region IWritingDisplayable Members

		public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
		{
			object value = item[propertyName];
			if (value != null)
			{
				int headingLevel = item.IsPage ? 1 : 2;
			writer.Write("<h");
			writer.Write(headingLevel);
			if(CssClass != null)
				writer.Write(" class='" + CssClass + "'");
			writer.Write(">");
				writer.Write(value);
			writer.Write("</h");
			writer.Write(headingLevel);
			writer.Write(">");
		}
		}

		#endregion

		#region IDisplayable Members

		public override Control AddTo(ContentItem item, string detailName, Control container)
		{
			var value = item[detailName];
			if (value != null)
			{
				var heading = new Hn
				{
					Level = item.IsPage ? 1 : 2,
					Text = value.ToString(),
					CssClass = CssClass
				};
				container.Controls.Add(heading);
				return heading;
			}
			return null;
		}

		#endregion
	}
}
