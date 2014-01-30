using System.Web.UI.WebControls;

namespace N2.Web.UI.WebControls
{
    /// <summary>A parameter used to retrieve content data for data sources.</summary>
    public class CurrentItemParameter : Parameter
    {
        public CurrentItemParameter()
            : base()
        {
        }
        public CurrentItemParameter(CurrentItemParameter original)
            : base(original)
        {
        }

        public string DetailName
        {
            get { return (string)ViewState["DetailName"] ?? ""; }
            set { ViewState["DetailName"] = value; }
        }

        protected override System.Web.UI.WebControls.Parameter Clone()
        {
            return new CurrentItemParameter(this);
        }

        protected override object Evaluate(System.Web.HttpContext context, System.Web.UI.Control control)
        {
            ContentItem item = ItemUtility.FindCurrentItem(control.Parent);
            if (item != null)
                return item[this.DetailName] ?? this.DefaultValue;
            else
                return DefaultValue;
        }
    }
}
