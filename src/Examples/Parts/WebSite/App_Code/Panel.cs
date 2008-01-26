using N2;
using N2.Details;
using N2.Integrity;
using N2.Web.UI;
using System.Web.UI.WebControls;

[Definition("Panel")]
[AllowedZones("Content")]
public class Panel : AbstractItem
{

	[Editable("Color", typeof(DropDownList), "SelectedValue", 120, DataBind = true)]
	[EditorModifier("DataSource", new string[] { "", "LightSalmon", "Pink", "LightYellow", "Lavender", "YellowGreen", "LightCyan", "LightBlue", "Cornsilk", "Gainsboro" })]
	public virtual string BackColor
	{
		get { return (string)(GetDetail("BackColor") ?? "Gainsboro"); }
		set { SetDetail("BackColor", value); }
	}

}
