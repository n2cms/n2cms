using System.Web.UI;
using System;

namespace N2.Definitions
{
    /// <summary>
    /// Used as root container when retrieving editable attributes and editor
    /// containers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]  
    internal class RootContainer : EditorContainerAttribute
    {
        public RootContainer()
            : base("Root", 0)
        {
        }

        public override Control AddTo(Control container)
        {
            return container;
        }
    }
}
