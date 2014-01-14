using N2.Definitions;
using N2.Web.UI.WebControls;
using System;

namespace N2.Web.UI
{
    /// <summary>
    /// Places a container in the right-hand side of the editing UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class SidebarContainerAttribute : EditorContainerAttribute
    {
        public SidebarContainerAttribute(string name)
            : base(name, 0)
        {
        }

        public SidebarContainerAttribute(string name, int sortOrder)
            : base(name, sortOrder)
        {
        }

        public string HeadingText { get; set; }

        public override System.Web.UI.Control AddTo(System.Web.UI.Control container)
        {
            var accessor = ItemUtility.FindInParents<Edit.IPlaceHolderAccessor>(container);
            if (accessor == null)
                return null;
            
            Box box = new Box();
            box.ID = Name;
            box.HeadingText = HeadingText;
            var placeholder = accessor.GetPlaceHolder("Sidebar");
            if (placeholder == null)
                return null;

            placeholder.Controls.Add(box);
            return box;
        }
    }
}
