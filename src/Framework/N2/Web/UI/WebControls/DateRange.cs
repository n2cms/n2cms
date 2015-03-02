using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Resources;

namespace N2.Web.UI.WebControls
{
    [ValidationProperty("From")]
    public class DateRange : Control, IValidator
    {
        DatePicker fromDate = new DatePicker();
        DatePicker toDate = new DatePicker();
        Label between = new Label();

        public DateTime? From
        {
            get { return ParseDate(FromDatePicker.Text); }
            set { FromDatePicker.Text = ToString(value); }
        }

        public DateTime? To
        {
            get { return ParseDate(ToDatePicker.Text); }
            set { ToDatePicker.Text = ToString(value); }
        }

        public string BetweenText
        {
            get
            {
                this.EnsureChildControls();
                return BetweenLabel.Text;
            }
            set 
            {
                this.EnsureChildControls();
                BetweenLabel.Text = value; 
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

            FromDatePicker.ID = "from" + this.ID;
            this.Controls.Add(FromDatePicker);

            this.Controls.Add(BetweenLabel);

            ToDatePicker.ID = "to" + this.ID;
            this.Controls.Add(ToDatePicker);

            FromDatePicker.TextChanged += TextChanged;
            ToDatePicker.TextChanged += TextChanged;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            string scriptFormat = @"            
jQuery('#{0}').bind('dpClosed', function(e, selectedDates) {{
    var d = selectedDates[0];
    if (d) {{
        d = new Date(d);
        jQuery('#{1}').dpSetStartDate(d.addDays(1).asString());
    }}
}});
jQuery('#{1}').bind('dpClosed', function(e, selectedDates) {{
    var d = selectedDates[0];
    if (d) {{
        d = new Date(d);
        jQuery('#{0}').dpSetEndDate(d.addDays(-1).asString());
    }}
}});";
            string script = string.Format(scriptFormat, FromDatePicker.ClientID, ToDatePicker.ClientID);
            Register.JavaScript(Page, script, ScriptOptions.DocumentReady);
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
                return date.Value.ToString(CultureInfo.InvariantCulture);
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

        [NotifyParentProperty(true)]
        public DatePicker FromDatePicker
        {
            get { return fromDate; }
        }

        [NotifyParentProperty(true)]
        public DatePicker ToDatePicker
        {
            get { return toDate; }
        }

        [NotifyParentProperty(true)]
        public Label BetweenLabel
        {
            get { return between; }
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
