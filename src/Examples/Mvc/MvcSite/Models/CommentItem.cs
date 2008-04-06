using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using N2;
using N2.Details;
using N2.Integrity;

namespace MvcTest.Models
{
	[Definition("Comment")]
	[RestrictParents(typeof(NewsPage))]
	public class CommentItem : AbstractPage
	{
		public override bool IsPage
		{
			get { return false; }
		}

		[EditableFreeTextArea("Text", 100)]
		public virtual string Text
		{
			get { return (string)(GetDetail("Text") ?? string.Empty); }
			set { SetDetail("Text", value, string.Empty); }
		}
	}
}
