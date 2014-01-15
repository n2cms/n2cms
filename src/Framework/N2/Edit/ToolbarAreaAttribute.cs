using System;
using System.Security.Principal;
using System.Web.UI;
using N2.Definitions;

namespace N2.Edit
{
    public abstract class ToolbarAreaAttribute : Attribute, IContainable
    {
        private string containerName;
        private int sortOrder;
        private string name;

        public int SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }

        public string ContainerName
        {
            get { return containerName; }
            set { containerName = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        public Control AddTo(Control container)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorized(IPrincipal user)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IContainable other)
        {
            throw new NotImplementedException();
        }
    }
}
