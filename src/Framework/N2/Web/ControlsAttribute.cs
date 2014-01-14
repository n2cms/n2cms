using System;
using System.Diagnostics;

namespace N2.Web
{
    /// <summary>
    /// Used to bind a controller to a certain content type.
    /// </summary>
    [DebuggerDisplay("ControlsAttribute: {AdapterType}->{ItemType}")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class ControlsAttribute : Attribute, IComparable<ControlsAttribute>, IAdapterDescriptor
    {
        private readonly Type itemType;
        private Type adapterType;

        public ControlsAttribute(Type itemType)
        {
            this.itemType = itemType;
        }

        /// <summary>The type of item beeing adapted.</summary>
        public Type ItemType
        {
            get { return itemType; }
        }

        /// <summary>The type of adapter referenced by this descriptor. This property is set by the framework as adapters are enumerated.</summary>
        public Type AdapterType
        {
            get { return adapterType; }
            set { adapterType = value; }
        }

        /// <summary>The name of the controller. Used to reference the controller in ASP.NET MVC scenarios.</summary>
        public string ControllerName
        {
            get
            {
                string name = AdapterType.Name;
                int i = name.IndexOf("Controller");
                if (i > 0)
                {
                    return name.Substring(0, i);
                }
                return name;
            }
        }

        /// <summary>Compares the path against the referenced item type to determine whether this is the correct adapter.</summary>
        /// <param name="path">The request path.</param>
        /// <param name="requiredType">The type of adapter needed.</param>
        /// <returns>True if the descriptor references the correct adapter.</returns>
        public bool IsAdapterFor(PathData path, Type requiredType)
        {
            if (path.IsEmpty())
                return false;

            return ItemType.IsAssignableFrom(path.CurrentItem.GetContentType()) && requiredType.IsAssignableFrom(adapterType);
        }

        #region IComparable<IAdapterDescriptor> Members

        public int CompareTo(IAdapterDescriptor other)
        {
            return Utility.InheritanceDepth(other.ItemType) - Utility.InheritanceDepth(ItemType);
        }

        #endregion

        #region IComparable<ControlsAttribute> Members

        int IComparable<ControlsAttribute>.CompareTo(ControlsAttribute other)
        {
            return CompareTo(other);
        }

        #endregion
    }
}
