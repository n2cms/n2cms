using System;
using System.Reflection;
using System.Web.UI;
using N2.Definitions;

namespace N2.Web.UI
{
    /// <summary>Attribute used to modify a detail's editor.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]    
    public class EditorModifierAttribute : Attribute, IUniquelyNamed
    {
        #region Constructor
        public EditorModifierAttribute(string editorPropertyName, object value)
        {
            this.editorPropertyName = editorPropertyName;
            this.value = value;
        } 
        #endregion

        #region Private Members
        private string name;
        private string editorPropertyName;
        private object value; 
        #endregion

        #region Public Properties
        /// <summary>Gets or sets the name of the detail whose editor should have this modifier applied. When the attribute is specified on a property in the content item class that property is used.</summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>Gets or sets the name of the property on the editor to set.</summary>
        public string EditorPropertyName
        {
            get { return editorPropertyName; }
            set { editorPropertyName = value; }
        }

        /// <summary>Gets or sets the value to apply to property with the name <see cref="EditorPropertyName"/> on the editor.</summary>
        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        } 
        #endregion

        #region Methods
        /// <summary>Applies the modifications specified by this attribute. Updates the property specified by <see cref="EditorPropertyName"/> with the value specified by <see cref="Value"/>.</summary>
        /// <param name="editor">The editor control to modify.</param>
        public virtual void Modify(Control editor)
        {
            PropertyInfo pi = editor.GetType().GetProperty(EditorPropertyName);
            System.Diagnostics.Debug.Assert(pi != null, "No property with the given name found on the editor, property name: " + EditorPropertyName);
            if (pi != null)
            {
                object o = Value;
                if (o.GetType() != pi.PropertyType)
                    o = Utility.Convert(o, pi.PropertyType);
                if (o != null)
                    pi.SetValue(editor, o, new object[0]);
            }
        }
        #endregion
    }
}
