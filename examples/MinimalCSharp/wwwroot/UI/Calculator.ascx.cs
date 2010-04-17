using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI;
using N2;
using App.Models;

namespace App.UI
{
	public partial class Calculator1 : ContentUserControl<ContentItem, CalculatorItem>
	{
		protected void ButtonNumber_Click(object sender, EventArgs e)
		{
			TextBox1.Text += ((Button)sender).Text;
		}
		protected void ButtonDivide_Click(object sender, EventArgs e)
		{
			Memory = double.Parse(TextBox1.Text);
		}
		protected void ButtonMultiply_Click(object sender, EventArgs e)
		{
			Op = ((Button)sender).Text;
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
}