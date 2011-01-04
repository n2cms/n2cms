using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Web.UI;
using N2.Edit.Web;
using N2.Resources;
using System.Threading;
using System.Text.RegularExpressions;

namespace N2.Web.UI.WebControls
{
	[ValidationProperty("SelectedDate")]
	public class DatePicker : Control, IEditableTextControl, INamingContainer
	{
        Label label = new Label();
		TextBox datePicker = new TextBox();
		TextBox timePicker = new TextBox();

		protected override void CreateChildControls()
		{
            Label.AssociatedControlID = "date";
            Controls.Add(Label);

			DatePickerBox.ID = "date";
			Controls.Add(DatePickerBox);
			DatePickerBox.CssClass = "datePicker";
			DatePickerBox.TextChanged += OnTextChanged;

			TimePickerBox.ID = "time";
			Controls.Add(TimePickerBox);
			TimePickerBox.CssClass = "timePicker";
			TimePickerBox.TextChanged += OnTextChanged;

			base.CreateChildControls();
		}

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            label.Visible = label.Text.Length > 0;
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
			Register.JQueryPlugins(Page);
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
                    DateTime parsed;
                    if (DateTime.TryParse(value, out parsed))
                    {
                        DatePickerBox.Text = parsed.ToShortDateString();
                        if (parsed.Second == 0)
                            TimePickerBox.Text = parsed.ToShortTimeString();
                        else
                            TimePickerBox.Text = parsed.ToLongTimeString();
                    }
				}
				else
				{
					DatePickerBox.Text = string.Empty;
					TimePickerBox.Text = string.Empty;
				}
			}
        }

        [NotifyParentProperty(true)]
        public Label Label
        {
            get { return label; }
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
