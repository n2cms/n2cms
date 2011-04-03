using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;

namespace N2.Details
{
	public class EditableMetaTagAttribute : EditableTextAttribute
	{
		public override System.Web.UI.Control AddTo(ContentItem item, string detailName, System.Web.UI.Control container)
		{
			using (var tw = new StringWriter())
			{
				Write(item, detailName, tw);
				var lc = new LiteralControl(tw.ToString());
				return lc;
			}
		}

		public override void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
		{
			string content = item[propertyName] as string;
			if (string.IsNullOrEmpty(content))
				return;

			writer.Write("<meta name=\"");
			writer.Write(propertyName.ToLower());
			writer.Write("\" content=\"");
			writer.Write(content);
			writer.Write("\" />");			
		}
	}
}
