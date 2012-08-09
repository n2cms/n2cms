using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Adds editing of a number.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class EditableNumberAttribute : EditableTextAttribute
	{
		string invalidRangeMessage;

		/// <summary>Initializes a new instance of the EditableTextBoxAttribute class.</summary>
		public EditableNumberAttribute()
			: this(null, 50)
		{
		}

		/// <summary>Initializes a new instance of the EditableTextBoxAttribute class.</summary>
		/// <param name="title">The label displayed to editors</param>
		/// <param name="sortOrder">The order of this editor</param>
		public EditableNumberAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
			DefaultValue = 0;
			Required = true;
			MaxLength = 11;
			ValidationType = ValidationDataType.Integer;
		}



		public string InvalidRangeText { get; set; }

		/// <summary>Gets or sets the message for the regular expression validator.</summary>
		public string InvalidRangeMessage
		{
			get { return invalidRangeMessage ?? Title + " is invalid."; }
			set { invalidRangeMessage = value; }
		}

		public string MaximumValue { get; set; }

		public string MinimumValue { get; set; }

		public ValidationDataType ValidationType { get; set; }

		protected override void AddValidation(Control container, Control editor)
		{
			base.AddValidation(container, editor);

			if (ValidationType != ValidationDataType.String)
			{
				BaseCompareValidator rv = CreateCompareValidator();
				rv.Type = ValidationType;
				rv.ID = Name + "_rv";
				rv.ControlToValidate = editor.ID;
				rv.Display = ValidatorDisplay.Dynamic;
				rv.Text = GetLocalizedText("InvalidRangeText") ?? InvalidRangeText;
				rv.ErrorMessage = GetLocalizedText("InvalidRangeMessage") ?? InvalidRangeMessage;
				container.Controls.Add(rv);
			}
		}

		private BaseCompareValidator CreateCompareValidator()
		{
			if (MaximumValue != null || MinimumValue != null)
				return new RangeValidator { MinimumValue = MinimumValue ?? ValidationTypeMinValue(), MaximumValue = MaximumValue ?? ValidationTypeMaxValue() };

			return new CompareValidator { Operator = ValidationCompareOperator.DataTypeCheck };
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			TextBox tb = editor as TextBox;
			switch (ValidationType)
			{
				case ValidationDataType.Currency:
					return Utility.SetPropertyOrDetail(item, Name, decimal.Parse(tb.Text));
				case ValidationDataType.Date:
					return Utility.SetPropertyOrDetail(item, Name, DateTime.Parse(tb.Text));
				case ValidationDataType.Double:
					return Utility.SetPropertyOrDetail(item, Name, double.Parse(tb.Text));
				case ValidationDataType.Integer:
					return Utility.SetPropertyOrDetail(item, Name, int.Parse(tb.Text));
				default:
					return Utility.SetPropertyOrDetail(item, Name, tb.Text);
			}
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			TextBox tb = editor as TextBox;
			object value = item[Name];
			if (value != null)
				tb.Text = value.ToString();
			else
				tb.Text = ValidationTypeDefault();
		}

		private string ValidationTypeDefault()
		{
			switch (ValidationType)
			{
				case ValidationDataType.Currency:
					return default(decimal).ToString();
				case ValidationDataType.Date:
					return DateTime.Now.ToString();
				case ValidationDataType.Double:
					return default(double).ToString();
				case ValidationDataType.Integer:
					return default(int).ToString();
				default:
					return "";
			}
		}

		private string ValidationTypeMinValue()
		{
			switch (ValidationType)
			{
				case ValidationDataType.Currency:
					return decimal.MinValue.ToString();
				case ValidationDataType.Date:
					return DateTime.MinValue.ToString();
				case ValidationDataType.Double:
					return double.MinValue.ToString();
				case ValidationDataType.Integer:
					return int.MinValue.ToString();
				default:
					return "";
			}
		}

		private string ValidationTypeMaxValue()
		{
			switch (ValidationType)
			{
				case ValidationDataType.Currency:
					return decimal.MaxValue.ToString();
				case ValidationDataType.Date:
					return DateTime.MaxValue.ToString();
				case ValidationDataType.Double:
					return double.MaxValue.ToString();
				case ValidationDataType.Integer:
					return int.MaxValue.ToString();
				default:
					return "";
			}
		}
	}
}