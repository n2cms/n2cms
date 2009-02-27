using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI;
using N2.Resources;
using System.Threading;
using System.Text.RegularExpressions;

namespace N2.Web.UI.WebControls
{
	[ValidationProperty("SelectedDate")]
	public class DatePicker : Control, IEditableTextControl
	{
		TextBox datePicker = new TextBox();
		TextBox timePicker = new TextBox();

		protected override void CreateChildControls()
		{
			DatePickerBox.ID = ID + "_date";
			Controls.Add(DatePickerBox);
			DatePickerBox.CssClass = "datePicker";
			DatePickerBox.TextChanged += OnTextChanged;

			DatePickerBox.ID = ID + "_time";
			Controls.Add(TimePickerBox);
			TimePickerBox.CssClass = "timePicker";
			TimePickerBox.TextChanged += OnTextChanged;

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
				if (DateTime.TryParse(Text, out d))
					return d;
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
			EnsureChildControls();
		}

		private void RegiserClientScript()
		{
			Register.JQuery(Page);
			Register.JavaScript(Page, "~/Edit/Js/plugins.ashx");
			string script = string.Format(DateScriptFormat, FirstDayOfWeek, DateFormat, FirstDate);
			Register.JavaScript(Page, script, ScriptOptions.DocumentReady);
		}

		protected const string DateScriptFormat = @"
Date.firstDayOfWeek = {0};
Date.format = '{1}';
jQuery.dpText.TEXT_CHOOSE_DATE = '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
jQuery('.datePicker').datePicker({{ startDate: '{2}' }});";

		protected virtual int FirstDayOfWeek
		{
			get { return (int)Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek; }
		}
		protected virtual string DateFormat
		{
			get
			{
				CultureInfo culture = Thread.CurrentThread.CurrentCulture;
				string datePattern = culture.DateTimeFormat.ShortDatePattern;
				datePattern = Regex.Replace(datePattern, "M+", "mm");
				datePattern = Regex.Replace(datePattern, "d+", "dd");
				datePattern = Regex.Replace(datePattern, "y+", delegate(Match m) { return m.Value.Length < 3 ? "yy" : "yyyy"; });
				return datePattern;
			}
		}

		protected virtual string FirstDate
		{
			get { return new DateTime(2000, 1, 1).ToString(DateFormat); }
		}

		#region IEditableTextControl Members

		public event EventHandler TextChanged;

		#endregion

		#region ITextControl Members

		public string Text
		{
			get
			{
				return string.Format("{0} {1}", DatePickerBox.Text, TimePickerBox.Text);
			}
			set
			{
				if(value != null)
				{
					string[] dateTime = value.Split(' ');
					DatePickerBox.Text = dateTime[0];
					if (dateTime.Length > 1)
						TimePickerBox.Text = dateTime[1].EndsWith(":00") ? dateTime[1].Substring(0, dateTime[1].Length - 3) : dateTime[1];
					else
						TimePickerBox.Text = string.Empty;

					if (dateTime.Length > 2)
						TimePickerBox.Text += " " + dateTime[2]; // This could be AM/PM
				}
				else
				{
					DatePickerBox.Text = string.Empty;
					TimePickerBox.Text = string.Empty;
				}
			}
		}

		[NotifyParentProperty(true)]
		public TextBox DatePickerBox
		{
			get { return datePicker; }
		}

		[NotifyParentProperty(true)]
		public TextBox TimePickerBox
		{
			get { return timePicker; }
		}

		#endregion
	}
}
