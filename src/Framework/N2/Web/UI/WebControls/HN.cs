using System;
using System.Web;
using System.Web.UI;

namespace N2.Web.UI.WebControls
{
	[Obsolete]
	public class H1 : Hn { }
	[Obsolete]
	public class H2 : Hn { protected override int DefaultLevel { get { return 2; } } }
	[Obsolete]
	public class H3 : Hn { protected override int DefaultLevel { get { return 3; } } }
	[Obsolete]
	public class H4 : Hn { protected override int DefaultLevel { get { return 4; } } }
	[Obsolete]
	public class H5 : Hn { protected override int DefaultLevel { get { return 5; } } }
	[Obsolete]
	public class H6 : Hn { protected override int DefaultLevel { get { return 6; } } }

	public class Hn : Control, ITextControl
	{
		public int Level
		{
			get { return (int)(ViewState["Level"] ?? DefaultLevel); }
			set 
			{
				if (value < 1 || value > 6) 
					throw new ArgumentException("The level must be between 1 and 6."); 
				ViewState["Level"] = value; 
			}
		}

		public string Text
		{
			get { return (string)(ViewState["Text"] ?? string.Empty); }
			set { ViewState["Text"] = value; }
		}

		public string CssClass { get; set; }

		protected virtual int DefaultLevel
		{
			get { return 1; }
		}

		protected string TagKey { get { return "h" + this.Level; } }

		public bool AllowHtml { get; set; }

		protected override void Render(HtmlTextWriter writer)
		{
			if (Text.Length > 0)
			{
				string tag = TagKey;
				writer.WriteFullBeginTag(tag);
				writer.Write(AllowHtml ? Text : HttpUtility.HtmlEncode(Text));
				if (CssClass != null)
					writer.WriteAttribute("class", CssClass);
				writer.WriteEndTag(tag);
			}
			else
			{
				writer.Write("<!--empty " + TagKey + "-->");
			}
		}
	}
}
