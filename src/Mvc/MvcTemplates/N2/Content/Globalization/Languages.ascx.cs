using N2.Edit.Web;

namespace N2.Edit.Globalization
{
    public partial class Languages : EditPageUserControl
    {
        private object dataSource;

        public object DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        protected string GetClass()
        {
            string className = (bool)Eval("IsNew") ? "new" : "existing";

            if (Selection.SelectedItem == (ContentItem)Eval("ExistingItem"))
                className += " current";

            return className;
        }
    }
}
