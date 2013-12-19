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
using System.Web.UI.WebControls;
using N2.Persistence.Search;
using System.IO;
using N2.Web.UI.WebControls;
using N2.Definitions;
using N2.Edit;

namespace N2.Details
{
    /// <summary>
    /// Attribute used to mark the DirectUrl property of an <see cref="Definition.IUrlSource"/> 
    /// as editable. This editor is predefined to set constraints relating to url sources.
    /// <example>
    /// public class TextPage : ContentItem, N2.Definitions.IUrlSource
    /// {
    ///     [N2.Details.EditableDirectUrl("Direct URL", 50)]
    ///     public virtual string DirectUrl { get; set; }
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableDirectUrlAttribute : EditableTextAttribute
    {
        public EditableDirectUrlAttribute()
            : this(null, 100)
        {
        }


        public string UniqueUrlText { get; set; }

        public string UniqueUrlMessage { get; set; }

        /// <summary>Initializes a new instance of the EditableTextBoxAttribute class.</summary>
        /// <param name="title">The label displayed to editors</param>
        /// <param name="sortOrder">The order of this editor</param>
        public EditableDirectUrlAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
            UniqueUrlText = "*";
            UniqueUrlMessage = "{0} must be unique, but is already used by {1} (#{2})";

            Validate = true;
            ValidationExpression = "^/[^?]*$";
            ValidationMessage = "The direct url must start with slash (/), e.g. /direct";
        }

        public override bool UpdateItem(ContentItem item, Control editor)
        {
            if (item is IUrlSource)
            {
                TextBox tb = editor as TextBox;
                string value = tb.Text;
                if (DefaultValue is string && tb.Text == (string)DefaultValue)
                    value = null;
                if (!AreEqual(value, item[Name]))
                {
                    item[Name] = value.TrimEnd('/');
                    return true;
                }
            }
            return false;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            TextBox tb = editor as TextBox;
            if (item is IUrlSource)
            {
                tb.Text = Utility.Convert<string>(item[Name]) ?? DefaultValue as string;
            }
            else
            {
                tb.Text = item.GetContentType() + " doesn't implement IUrlSource and cannot be used in combination with [EditableDirectUrl].";
                tb.ReadOnly = true;
            }
        }

        protected override void AddValidation(Control container, Control editor)
        {
            base.AddValidation(container, editor);

            var cv = new CustomValidator();
            cv.ID = Name + "_cv";
            cv.ControlToValidate = editor.ID;
            cv.Display = ValidatorDisplay.Dynamic;
            cv.Text = GetLocalizedText("UniqueUrlText") ?? UniqueUrlText;
            cv.ServerValidate += (object source, ServerValidateEventArgs args) =>
            {
                var url = ((TextBox)editor).Text.Trim('/');
                url = Engine.RequestContext.Url.HostUrl.Append(url);
                var existing = Engine.UrlParser.FindPath(url);
                if (!existing.IsEmpty() && existing.CurrentItem != new SelectionUtility(container, Engine).SelectedItem)
                {
                    args.IsValid = false;
                    cv.ErrorMessage = string.Format(GetLocalizedText("UniqueUrlMessage") ?? UniqueUrlMessage, GetLocalizedText("Title") ?? Title, existing.CurrentItem.Title, existing.CurrentItem.ID);
                }
            };
            container.Controls.Add(cv);
        }

        
    }
}
