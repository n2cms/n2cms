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
			var script = string.Format(DateScriptFormat, 
				/* {0} */ FirstDayOfWeek,
				/* {1} */ DateFormat,
				/* {2} */ FirstDate,
				/* {3} */ ToJsonArray(FormatInfo.ShortestDayNames),
				/* {4} */ ToJsonArray(FormatInfo.DayNames),
				/* {5} */ ToJsonArray(FormatInfo.AbbreviatedMonthNames),
				/* {6} */ ToJsonArray(FormatInfo.MonthNames),
				/* {7} */ Url.ResolveTokens("{ManagementUrl}/Resources/icons/calendar.png"),
				/* {8} */ datePicker.ClientID
				);
			Register.JavaScript(Page, script, ScriptOptions.DocumentReady);
			// ReSharper restore InvokeAsExtensionMethod
		}

		private static string ToJsonArray(IEnumerable<string> strings)
		{
			return '[' + string.Join(",", strings.Select(s => '\'' + s + '\'').ToArray()) + ']';
		}

		protected const string DateScriptFormat = @"
jQuery('#{8}').n2datepicker({{ firstDay:{0}, dateFormat:'{1}', dayNamesMin:{3}, dayNames:{4}, monthNamesShort:{5}, monthNames:{6}, showOn:'button', buttonImage:'{7}' }});";

		protected virtual int FirstDayOfWeek
		{
			get { return (int)FormatInfo.FirstDayOfWeek; }
		}

		private static DateTimeFormatInfo FormatInfo
		{
			get { return Thread.CurrentThread.CurrentCulture.DateTimeFormat; }
		}

		protected virtual string DateFormat
		{
			get
			{
				var culture = Thread.CurrentThread.CurrentCulture;
				var datePattern = culture.DateTimeFormat.ShortDatePattern;
				datePattern = Regex.Replace(datePattern, "M+", "mm");
				datePattern = Regex.Replace(datePattern, "d+", "dd");
				datePattern = Regex.Replace(datePattern, "y+", m => m.Value.Length < 3 ? "y" : "yy");
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
					if (!DateTime.TryParse(value, out parsed)) 
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
