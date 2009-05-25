using System;
using System.Web.UI.WebControls;
using N2;
using N2.Web.UI;

/// <summary>
/// Part templates defines two generic arguments. The first is for the 
/// page, the second is for the part itself. The content data is 
/// available through the CurrentPage and CurrentItem respectively.
/// </summary>
public partial class UI_Calculator : ContentUserControl<ContentItem, CalculatorItem>
{
	protected void ButtonNumber_Click(object sender, EventArgs e)
	{
		TextBox1.Text += ((Button) sender).Text;
	}
	protected void ButtonDivide_Click(object sender, EventArgs e)
	{
		Memory = double.Parse(TextBox1.Text);
	}
	protected void ButtonMultiply_Click(object sender, EventArgs e)
	{
		Op = ((Button) sender).Text;
		Memory = Value;
		Value = 0;
	}
	protected void ButtonSubtract_Click(object sender, EventArgs e)
	{
		Op = ((Button)sender).Text;
		Memory = Value;
		Value = 0;
	}
	protected void ButtonAdd_Click(object sender, EventArgs e)
	{
		Op = ((Button)sender).Text;
		Memory = Value;
		Value = 0;
	}
	protected void ButtonEquals_Click(object sender, EventArgs e)
	{
		switch (Op)
		{
			case "+":
				Value = Memory + Value;
				break;
			case "-":
				Value = Memory - Value;
				break;
			case "*":
				Value = Memory * Value;
				break;
			case "/":
				Value = Memory / Value;
				break;
		}
	}

	protected void ButtonClear_Click(object sender, EventArgs e)
	{
		Memory = 0;
		TextBox1.Text = "";
	}



	public double Memory
	{
		get { return (double)(ViewState["Memory"] ?? 0); }
		set { ViewState["Memory"] = value; }
	}
	public double Value
	{
		get { return double.Parse(TextBox1.Text.Length > 0 ? TextBox1.Text : "0"); }
		set { TextBox1.Text = value != 0 ? value.ToString() : ""; }
	}

	public string Op
	{
		get { return (string)(ViewState["Op"] ?? ""); }
		set { ViewState["Op"] = value; }
	}
}
