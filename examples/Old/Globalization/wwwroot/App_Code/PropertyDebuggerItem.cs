using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// An item that can be placed in the right column that swtiches languages.
/// </summary>
[N2.Definition("Property debugger", "PropertyDebuggerItem")]
[N2.Integrity.AllowedZones("Right")]
public class PropertyDebuggerItem : N2.ContentItem, N2.Definitions.IContainable
{
	public virtual Control AddTo(Control container)
	{
		Panel p = new Panel();
		p.CssClass = "uc";

		GridView gv = new GridView();
		gv.DataSource = Parent.Details;
		gv.DataBind();

		p.Controls.Add(gv);
		container.Controls.Add(p);
		return p;
	}

	public override bool IsPage
	{
		get
		{
			return false;
		}
	}
}
