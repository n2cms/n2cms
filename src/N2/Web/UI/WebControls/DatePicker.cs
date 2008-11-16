using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web.UI.HtmlControls;

[assembly: WebResource("N2.Resources.calendar.png", "img/png")]
[assembly: WebResource("N2.Resources.DateTimePicker.js", "text/javascript")]
[assembly: WebResource("N2.Resources.DateTimePicker.css", "text/css")]

namespace N2.Web.UI.WebControls
{
	[ValidationProperty("SelectedDate")]
	public class DatePicker : Control, IEditableTextControl
	{
		TextBox datePicker;
		TextBox timePicker;

		public DatePicker()
		{
		}

		protected override void CreateChildControls()
		{
			datePicker = new TextBox();
			datePicker.ID = ID + "_date";
			Controls.Add(datePicker);
			datePicker.CssClass = "datePicker";
			datePicker.TextChanged += new EventHandler(OnTextChanged);

			timePicker = new TextBox();
			datePicker.ID = ID + "_time";
			Controls.Add(timePicker);
			timePicker.CssClass = "timePicker";
			timePicker.TextChanged += new EventHandler(OnTextChanged);

			base.CreateChildControls();
		}

		void OnTextChanged(object sender, EventArgs e)
		{
			if (TextChanged != null)
				TextChanged.Invoke(this, e);
		}

		public DateTime? SelectedDate
		{
			get
			{
				DateTime d;
				if (DateTime.TryParse(this.Text, out d))
					return d;
				else
					return null;
			}
			set
			{
				Text = value != null 
					? value.Value.ToString() 
					: null;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			RegiserClientScript();
			RegisterStyleSheet();
			EnsureChildControls();
		}

		private void RegiserClientScript()
		{
			this.Page.ClientScript.RegisterClientScriptResource(typeof(DatePicker), "N2.Resources.DateTimePicker.js");
		}

		private void RegisterStyleSheet()
		{
			HtmlLink cssLink = new HtmlLink();
			cssLink.Href = Page.ClientScript.GetWebResourceUrl(typeof(DatePicker), "N2.Resources.DateTimePicker.css");
			cssLink.Attributes["type"] = "text/css";
			cssLink.Attributes["rel"] = "stylesheet";
			Page.Header.Controls.Add(cssLink);
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			base.Render(writer);
			RenderCalendarIcon(writer);
		}

		private void RenderCalendarIcon(HtmlTextWriter writer)
		{
			writer.Write("<img class='calendarOpener' src='{0}' onclick=\"displayDatePicker('{1}');\"/>",
				Page.ClientScript.GetWebResourceUrl(typeof(DatePicker), "N2.Resources.calendar.png"),
				datePicker.UniqueID,
				datePicker.ClientID
				);
		}

		#region IEditableTextControl Members

		public event EventHandler TextChanged;

		#endregion

		#region ITextControl Members

		public string Text
		{
			get
			{
				return string.Format("{0} {1}", this.datePicker.Text, this.timePicker.Text);
			}
			set
			{
				if(value != null)
				{
					string[] dateTime = value.Split(' ');
					datePicker.Text = dateTime[0];
					if (dateTime.Length > 1)
						timePicker.Text = dateTime[1];
					else
						timePicker.Text = string.Empty;
				}
				else
				{
					datePicker.Text = string.Empty;
					timePicker.Text = string.Empty;
				}
			}
		}

		#endregion
	}
}
