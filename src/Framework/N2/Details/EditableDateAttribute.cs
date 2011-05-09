using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Web.UI.WebControls;

namespace N2.Details
{
	/// <summary>
	/// Defines an editable date/time picker control for a content item.
	/// </summary>
	public class EditableDateAttribute : AbstractEditableAttribute, IDisplayable, IWritingDisplayable
	{
		bool showDate = true;
		bool showTime = true;

		public EditableDateAttribute()
			: base(null, 20)
		{
		}

		public EditableDateAttribute(string title, int sortOrder)
			: base(title, sortOrder)
		{
		}

		/// <summary>Set to false to hide time box.</summary>
		public bool ShowTime
		{
			get { return showTime; }
			set { showTime = value; }
		}

		/// <summary>Set to false to hide date box.</summary>
		public bool ShowDate
		{
			get { return showDate; }
			set { showDate = value; }
		}

		public override bool UpdateItem(ContentItem item, Control editor)
		{
			DatePicker picker = (DatePicker)editor;
			if ((DateTime?)item[Name] != picker.SelectedDate)
			{
				item[Name] = picker.SelectedDate;
				return true;
			}
			return false;
		}

		public override void UpdateEditor(ContentItem item, Control editor)
		{
			DatePicker picker = (DatePicker) editor;
			picker.SelectedDate = (DateTime?)item[Name];
		}

		protected override Control AddEditor(Control container)
		{
			DatePicker picker = new DatePicker();
			picker.TimePickerBox.Visible = ShowTime;
			picker.DatePickerBox.Visible = ShowDate;
			picker.ID = Name;
			container.Controls.Add(picker);
			return picker;
		}

		#region IDisplayable Members

		Control IDisplayable.AddTo(ContentItem item, string detailName, Control container)
		{
			DateTime? date = item[Name] as DateTime?;
			if(date.HasValue)
			{
				Literal l = new Literal();
				l.Text = date.Value.ToString();
				container.Controls.Add(l);
				return l;
			}
			return null;
		}

		#endregion

		#region IWritingDisplayable Members

		public void Write(ContentItem item, string propertyName, System.IO.TextWriter writer)
		{
			writer.Write(item[propertyName]);
		}

		#endregion
	}
}
