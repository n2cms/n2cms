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
using N2.Details;
using N2;

namespace MvcTest.Models
{
	[WithEditableTitle, WithEditableName]
	public abstract class AbstractPage : ContentItem, INode
	{
		public string PreviewUrl
		{
			get { return Url; }
		}
	}
}
