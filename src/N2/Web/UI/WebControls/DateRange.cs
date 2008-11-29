using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace N2.Web.UI.WebControls
{
	[ValidationProperty("From")]
	public class DateRange : Control, IValidator
	{
		private DatePicker fromDate;
		private DatePicker toDate;
		private Label between;

		public DateTime? From
		{
			get { return ParseDate(fromDate.Text); }
			set { fromDate.Text = ToString(value); }
		}

		public DateTime? To
		{
			get { return ParseDate(toDate.Text); }
			set { toDate.Text = ToString(value); }
		}

		public string BetweenText
		{
			get
			{
				this.EnsureChildControls();
				return between.Text;
			}
			set 
			{
				this.EnsureChildControls();
				between.Text = value; 
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Page.Validators.Add(this);
			EnsureChildControls();
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			fromDate = new DatePicker();
			fromDate.ID = "from" + this.ID;
			this.Controls.Add(fromDate);

			between = new Label();
			this.Controls.Add(between);

			toDate = new DatePicker();
			toDate.ID = "to" + this.ID;
			this.Controls.Add(toDate);

			fromDate.TextChanged += TextChanged;
			toDate.TextChanged += TextChanged;
		}

		void TextChanged(object sender, EventArgs e)
		{
			EventHandler changed = Events[changedKey] as EventHandler;
			if (changed != null)
				changed.Invoke(this, new EventArgs());
		}

		private DateTime? ParseDate(string text)
		{
			if(!string.IsNullOrEmpty(text))
			{
				DateTime date;
				if(DateTime.TryParse(text, out date))
					return date;
			}
			return null;
		}

		private string ToString(DateTime? date)
		{
			if (date != null)
				return date.ToString();
			else
				return string.Empty;
		}

		#region IValidator Members

		public string ErrorMessage
		{
			get { return (string)(ViewState["ErrorMessage"] ?? "Invalid date range."); }
			set { ViewState["ErrorMessage"] = value; }
		}

		public bool IsValid
		{
			get { return (bool)(ViewState["IsValid"] ?? true); }
			set { ViewState["IsValid"] = value; }
		}

		public void Validate()
		{
			if (From != null && To != null)
				this.IsValid = From <= To;
			else
				this.IsValid = true;
		}

		#endregion
		private static readonly object changedKey = new object();

		public event EventHandler Changed
		{
			add { base.Events.AddHandler(changedKey, value); }
			remove { base.Events.RemoveHandler(changedKey, value); }
		}
	}
}
