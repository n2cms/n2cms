using System;
using System.Web.UI;

namespace N2.Web.UI.WebControls
{
    /// <summary>
    /// Hides it's contents if the given property has no value or is empty 
    /// string.
    /// </summary>
    public class HasValue : Control, IItemContainer
    {
        /// <summary>Use the displayer and the values from this path.</summary>
        public string Path
        {
            get { return (string)(ViewState["Path"] ?? string.Empty); }
            set { ViewState["Path"] = value; }
        }

        /// <summary>The name of the property on the content item whose value is displayed with the Display control.</summary>
        public string PropertyName
        {
            get { return (string)ViewState["PropertyName"] ?? ""; }
            set { ViewState["PropertyName"] = value; }
        }

        /// <summary>Inverses the behaviour of the HasValue control, i.e. the contents are hidden if there is a value.</summary>
        public bool Inverse
        {
            get { return (bool)(ViewState["Inverse"] ?? false); }
            set { ViewState["Inverse"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            bool show = PropertyHasValue(CurrentItem, PropertyName);
            Visible = Inverse ? !show : show;

            base.OnInit(e);
        }

        private bool PropertyHasValue(ContentItem item, string propertyName)
        {
            if (item == null)
                return false;
            object value = item[propertyName];
            if (value == null)
                return false;
            if (string.Empty.Equals(value))
                return false;
            return true;
        }


        #region IItemContainer Members

        private ContentItem currentItem = null;

        public ContentItem CurrentItem
        {
            get
            {
                if (currentItem == null)
                {
                    currentItem = ItemUtility.FindCurrentItem(Parent);
                    if (!string.IsNullOrEmpty(Path))
                        currentItem = ItemUtility.WalkPath(currentItem, Path);
                }
                return currentItem;
            }
        }

        #endregion

    }
}
