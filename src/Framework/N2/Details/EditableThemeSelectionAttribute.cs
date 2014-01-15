using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Resources;
using N2.Web;

namespace N2.Details
{
    /// <summary>
    /// Allows the selection of themes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableThemeSelectionAttribute : EditableListControlAttribute
    {
        public EditableThemeSelectionAttribute()
            : this("Theme", 14)
        {
        }

        public EditableThemeSelectionAttribute(string title, int sortOrder)
            : base(title, sortOrder)
        {
        }



        /// <summary>Show a preview theme link next to the drop down.</summary>
        public bool EnablePreview { get; set; }



        protected override Control AddEditor(Control container)
        {
            var editor = base.AddEditor(container);
            Register.JavaScript(container.Page, N2.Web.Url.ResolveTokens("{ManagementUrl}/Resources/Js/jquery.editableThemeSelection.js"));
            
            StringBuilder initializationScript = new StringBuilder();
            initializationScript.AppendFormat("jQuery('#{0}').editableThemeSelection({{ '' : null", editor.ClientID);
            foreach (string directoryPath in GetThemeDirectories())
            {
                string thumbnailPath = Path.Combine(directoryPath, "thumbnail.jpg");
                string themeName = Path.GetFileName(directoryPath);
                if (File.Exists(thumbnailPath))
                {
                    initializationScript.AppendFormat(", '{0}' : '{1}'", 
                        themeName, 
                        container.ResolveClientUrl(Url.ResolveTokens(Url.ThemesUrlToken + themeName + "/thumbnail.jpg")));
                }
                else
                {
                    initializationScript.AppendFormat(", '{0}' : null", 
                        themeName);
                }
            }
            initializationScript.AppendFormat("}});");
            Register.JavaScript(container.Page, initializationScript.ToString(), ScriptOptions.DocumentReady);

            return editor;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            base.UpdateEditor(item, editor);

            AddPreviewLink(item, editor);
        }

        private void AddPreviewLink(ContentItem item, Control editor)
        {
            if (!EnablePreview || editor.FindControl(editor.ID + "_preview") != null)
                return; // prevent double add in some situations

            var preview = new HyperLink()
            {
                ID = editor.ID + "_preview",
                Text = "Preview",
                NavigateUrl = "#",
                CssClass = "themePreview"
            };
            preview.Attributes["onclick"] = "window.open('" + N2.Web.Url.Parse(item.Url).AppendQuery("theme", "") + "' + document.getElementById('" + editor.ClientID + "').value, 'previewTheme', 'width=900,height=500'); return false;";
            editor.Parent.Controls.AddAt(editor.Parent.Controls.IndexOf(editor) + 1, preview);
        }

        protected override ListControl CreateEditor()
        {
            return new DropDownList();
        }

        protected override ListItem[] GetListItems()
        {
            List<ListItem> items = new List<ListItem>();

            foreach (string directoryPath in GetThemeDirectories())
            {
                string directoryName = Path.GetFileName(directoryPath);
                if (!directoryName.StartsWith("."))
                    items.Add(new ListItem(directoryName));
            }

            return items.ToArray();
        }

        private IEnumerable<string> GetThemeDirectories()
        {
            string path = HostingEnvironment.MapPath(Url.ResolveTokens(Url.ThemesUrlToken));
            if (Directory.Exists(path))
            {
                foreach (string directoryPath in Directory.GetDirectories(path))
                {
                    string directoryName = Path.GetFileName(directoryPath);
                    if (!directoryName.StartsWith("."))
                        yield return directoryPath;
                }
            }
        }
    }
}
