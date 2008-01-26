using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace N2.Web.UI.WebControls
{
	public class H1 : HN { }
	public class H2 : HN { protected override int DefaultLevel { get { return 2; } } }
	public class H3 : HN { protected override int DefaultLevel { get { return 3; } } }
	public class H4 : HN { protected override int DefaultLevel { get { return 4; } } }
	public class H5 : HN { protected override int DefaultLevel { get { return 5; } } }
	public class H6 : HN { protected override int DefaultLevel { get { return 6; } } }

	public class HN : Control, ITextControl
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

		protected virtual int DefaultLevel
		{
			get { return 1; }
		}

		protected string TagKey { get { return "h" + this.Level; } }

		protected override void Render(HtmlTextWriter writer)
		{
			if (Text.Length > 0)
			{
				string tag = TagKey;
				writer.WriteFullBeginTag(tag);
				writer.Write(Text);
				writer.WriteEndTag(tag);
			}
			else
			{
				writer.Write("<!--empty " + TagKey + "-->");
			}
		}
	}
}
