using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Resources;

namespace N2.Web.UI.WebControls
{
	[ValidationProperty("SelectedDate")]
	public class DatePicker : Control, IEditableTextControl, INamingContainer
	{
		readonly Label label = new Label();
		readonly TextBox datePicker = new TextBox();
		readonly TextBox timePicker = new TextBox();

		protected override void CreateChildControls()
		{
			Label.AssociatedControlID = "date";
			Controls.Add(Label);

			DatePickerBox.ID = "date";
			Controls.Add(DatePickerBox);
			DatePickerBox.CssClass = "datePicker";
			DatePickerBox.TextChanged += OnTextChanged;
			DatePickerBox.Attributes["placeholder"] = FormatInfo.ShortDatePattern;

			TimePickerBox.ID = "time";
			Controls.Add(TimePickerBox);
			TimePickerBox.CssClass = "timePicker";
			TimePickerBox.TextChanged += OnTextChanged;
			TimePickerBox.Attributes["placeholder"] = FormatInfo.ShortTimePattern;

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
				if (DateTime.TryParse(Text, out d)) return d;
				return null;
			}
			set
			{
				Text = value != null  ? value.Value.ToString(CultureInfo.InvariantCulture) : null;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EnsureChildControls();
			RegisterClientScript();
		}

		private void RegisterClientScript()
		{
			// ReSharper disable InvokeAsExtensionMethod
			Register.JQuery(Page);
			Register.JQueryUi(Page);
			Register.JQueryPlugins(Page);

			// ReSharper disable once FormatStringProblem
            var i18nScript = string.Format(I18nScriptFormat,
				/* {0} */ CurrentCultureInfo.Name,
                /* {1} */ ToJsonArray(FormatInfo.DayNames),
                /* {2} */ ToJsonArray(FormatInfo.AbbreviatedDayNames),
                /* {3} */ ToJsonArray(FormatInfo.ShortestDayNames),
                /* {4} */ ToJsonArray(FormatInfo.MonthNames),
                /* {5} */ ToJsonArray(FormatInfo.AbbreviatedMonthNames),
                /* {6} */ FirstDayOfWeek,
                /* {7} */ DateFormat,
				/* {8} */ CurrentCultureInfo.TextInfo.IsRightToLeft.ToString().ToLowerInvariant()
                );
            Register.JavaScript(Page, i18nScript, ScriptOptions.DocumentReady);

			var script = string.Format(DateScriptFormat,
				/* {0} */ CurrentCultureInfo.Name,
				/* {1} */ datePicker.ClientID
				);
			Register.JavaScript(Page, script, ScriptOptions.DocumentReady);
			// ReSharper restore InvokeAsExtensionMethod
		}

		private static string ToJsonArray(IEnumerable<string> strings)
		{
			return '[' + string.Join(",", strings.Select(s => '\'' + s + '\'')) + ']';
		}

		protected const string DateScriptFormat = @"
jQuery('#{1}').n2datepicker({{ language:'{0}' }});";

		protected const string I18nScriptFormat = @"
;(function($){{
	$.fn.datepicker.dates['{0}'] = {{
		days: {1},
		daysShort: {2},
		daysMin: {3},
		months: {4},
		monthsShort: {5},
		weekStart: {6},
		format: '{7}',
		rtl: {8}
	}};
}}(jQuery));";

		protected virtual int FirstDayOfWeek
		{
			get { return (int)FormatInfo.FirstDayOfWeek; }
		}

		private static CultureInfo CurrentCultureInfo
		{
			get { return Thread.CurrentThread.CurrentCulture; }
		}

		private static DateTimeFormatInfo FormatInfo
		{
			get { return CurrentCultureInfo.DateTimeFormat; }
		}

		protected virtual string DateFormat
		{
            get
            {
                var datePattern = FormatInfo.ShortDatePattern;

                datePattern = datePattern.Replace("dddd", "DD");
                datePattern = datePattern.Replace("ddd", "D");

                switch (datePattern.Count(p => p == 'M'))
                {
                    case 4:
                        datePattern = datePattern.Replace("MMMM", "MM");
                        break;

                    case 3:
                        datePattern = datePattern.Replace("MMM", "M");
                        break;

                    case 2:
                        datePattern = datePattern.Replace("MM", "mm");
                        break;

                    case 1:
                        datePattern = datePattern.Replace("M", "m");
                        break;

                    default:
                        datePattern = Regex.Replace(datePattern, "M+", "mm");
                        break;
                }

                datePattern = Regex.Replace(datePattern, "y+", m => m.Value.Length < 3 ? "yy" : "yyyy");
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
				if (string.IsNullOrEmpty(DatePickerBox.Text))
					return "";
				return string.Format("{0} {1}", DatePickerBox.Text, TimePickerBox.Text);
			}
			set
			{
				if(value != null)
				{
					DateTime parsed;
					if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite, out parsed)) 
						return;
					DatePickerBox.Text = parsed.ToShortDateString();
					TimePickerBox.Text = parsed.Second == 0 ? parsed.ToShortTimeString() : parsed.ToLongTimeString();
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
