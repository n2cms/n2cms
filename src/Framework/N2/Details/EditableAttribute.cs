#region License
/* Copyright (C) 2007 Cristian Libardo
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion

using System;
using System.Web.UI;

namespace N2.Details
{
    /// <summary>Attribute used to mark properties as editable. This is used to associate the control used for the editing with the property/detail on the content item whose value we are editing.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableAttribute : AbstractEditableAttribute
    {
        private Type controlType;
        private string controlPropertyName;
        private bool dataBind = false;
        private bool focus = false;

        #region Constructors
        /// <summary>Default/empty constructor.</summary>
        public EditableAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EditableAttribute class set to use a server control.
        /// </summary>
        /// <param name="title">The label displayed to editors</param>
        /// <param name="editorType">The type of webcontrol used for editing the unit's property</param>
        /// <param name="editorPropertyName">The property on the edit control that will update the unit's property</param>
        /// <param name="sortOrder">The order of this editor</param>
        public EditableAttribute(string title, Type editorType, string editorPropertyName, int sortOrder)
            : base(title, sortOrder)
        {
            this.ControlType = editorType;
            this.ControlPropertyName = editorPropertyName;
        } 
        #endregion

        #region Methods
        /// <summary>Is invoked when adding the editor control.</summary>
        /// <param name="container">The container control in which to add the editor.</param>
        /// <returns>The editor control.</returns>
        protected override Control AddEditor(Control container)
        {
            Control editor = CreateEditor(container);
            editor.ID = Name;
            container.Controls.Add(editor);
            if (DataBind || Focus)
                editor.PreRender += editor_PreRender;

            return editor;
        }

        #region AddTo Helper Methods

        void editor_PreRender(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if(!c.Page.IsPostBack)
                Modify(c);
        }

        /// <summary>Creates an editor of the type defined by the ControlType property.</summary>
        /// <param name="container">The container in which the editor will be added.</param>
        /// <returns>The created control.</returns>
        protected virtual Control CreateEditor(Control container)
        {
            return (Control)System.Activator.CreateInstance(this.ControlType);
        }
        #endregion

        /// <summary>Updates the item with the values from the editor.</summary>
        /// <param name="item">The item to update.</param>
        /// <param name="editor">The editor contorl whose values to update the item with.</param>
        public override bool UpdateItem(ContentItem item, Control editor)
        {
            object editorValue = this.GetEditorValue(editor);
            object itemValue = item[this.Name];
            if (!AreEqual(editorValue, itemValue))
            {
                item[Name] = editorValue;
                return true;
            }
            return false;
        }

        #region UpdateItem Helper Methods
        /// <summary>Gets the value from the editor control. The ControlPropertyName property on the attribute defines which property's value is retrieved.</summary>
        /// <param name="editor">The control whose value to get.</param>
        /// <returns>The value of the editor property with the name defined by ControlPropertyName.</returns>
        protected virtual object GetEditorValue(Control editor)
        {
            return DataBinder.Eval(editor, this.ControlPropertyName);
        } 
        #endregion

        /// <summary>Updates the editor with the values from the item.</summary>
        /// <param name="item">The item that contains values to assign to the editor.</param>
        /// <param name="editor">The editor to load with a value.</param>
        public override void UpdateEditor(ContentItem item, Control editor)
        {
            this.SetEditorValue(editor, item[this.Name]);
        }

        #region UpdateEditor Helper Methods
        /// <summary>Loads an editor with a value.</summary>
        /// <param name="editor">The editor to load with a value.</param>
        /// <param name="value">The value to load the editor with.</param>
        protected virtual void SetEditorValue(Control editor, object value)
        {
            if (editor == null) throw new ArgumentNullException("editor");

            System.Reflection.PropertyInfo pi = editor.GetType().GetProperty(this.ControlPropertyName);
            if (pi == null) throw new N2Exception("No property '{0}' found on the editor control '{1}'.", this.ControlPropertyName, editor.GetType());
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = Utility.Convert(value, pi.PropertyType);
            pi.SetValue(editor, value, null);
        } 
        #endregion

        /// <summary>Applies editor modifications after creating the control.</summary>
        /// <param name="editor">The editor to modify.</param>
        public virtual void Modify(Control editor)
        {
            if (this.DataBind)
                editor.DataBind();
            if (this.Focus)
                editor.Focus();
        }
        #endregion

        #region Properties
        /// <summary>Gets or sets whether the control should be databound when it's added to a page.</summary>
        public bool DataBind
        {
            get { return dataBind; }
            set { dataBind = value; }
        }

        /// <summary>Gets or sets whether the control should be focused when it's added to a page.</summary>
        public bool Focus
        {
            get { return focus; }
            set { focus = value; }
        }

        /// <summary>Gets or sets the type of the control that is used in combination with an item's property/detail.</summary>
        public Type ControlType
        {
            get { return controlType; }
            set { controlType = value; }
        }

        /// <summary>Gets or sets the property on the control that is used to get or set content data.</summary>
        public string ControlPropertyName
        {
            get { return controlPropertyName; }
            set { controlPropertyName = value; }
        }
        #endregion
    }
}
