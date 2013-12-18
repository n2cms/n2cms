using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Definitions;
using System;

namespace N2.Web.UI
{
    /// <summary>
    /// Organizes editors in a field set that can be expanded to show all details.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ExpandableContainerAttribute : EditorContainerAttribute
    {
        private string legend;

        public ExpandableContainerAttribute(string name, int sortOrder)
            : base(name, sortOrder)
        {
        }

        public ExpandableContainerAttribute(string name, string legend, int sortOrder)
            : this(name, sortOrder)
        {
            Legend = legend;
        }

        /// <summary>Gets or sets the fieldset legend (text/title).</summary>
        public string Legend
        {
            get { return legend; }
            set { legend = value; }
        }

        /// <summary>Adds the fieldset to a parent container and returns it.</summary>
        /// <param name="container">The parent container onto which to add the container defined by this interface.</param>
        /// <returns>The newly added fieldset.</returns>
        public override Control AddTo(Control container)
        {
            Panel fieldSet = new Panel();
            fieldSet.ID = Name;
            fieldSet.ToolTip = GetLocalizedText("Legend") ?? Legend;
            fieldSet.CssClass = "expandable";
            container.Controls.Add(fieldSet);
            return fieldSet;
        }
    }
}
