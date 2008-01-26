using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2;
using N2.Details;
using N2.Integrity;
using N2.Web.UI;

[Definition("Columns")]
[AllowedZones("Content")]
public class Columns : AbstractItem
{
	public const string WideNarrow = "Wide-Narrow";
	public const string NarrowWide = "Narrow-Wide";

	[Editable("Columns", typeof(DropDownList), "SelectedValue", 120, DataBind = true)]
	[EditorModifier("DataSource", new string[] { WideNarrow, NarrowWide })]
	public virtual string ColumnMode
	{
		get { return (string)(GetDetail("ColumnMode") ?? WideNarrow); }
		set { SetDetail("ColumnMode", value, WideNarrow); }
	}
}
