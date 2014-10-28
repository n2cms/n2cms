using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Details;
using N2.Templates.Mvc.Models.Parts;

namespace N2.Templates.Details
{
    public class EditableOptionsAttribute : AbstractEditableAttribute
    {
        public override bool UpdateItem(ContentItem item, Control editor)
        {
            TextBox tb = (TextBox)editor;
            string[] rows = tb.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = item.Children.Count-1; i >= 0; --i)
            {
                int index = Array.FindIndex(rows, delegate(string row)
                                                      {
                                                          return row == item.Children[i].Title;
                                                      }
                    );
                if (index < 0)
                    Context.Persister.Delete(item.Children[i]);
            }
            for (int i = 0; i < rows.Length; i++)
            {
                ContentItem child = FindChild(item, rows[i]);
                if (child == null)
                {
                    child = new Option();
                    child.Title = rows[i];
                    child.AddTo(item);
                }
                child.SortOrder = i;
            }

            return true;
        }

        private static ContentItem FindChild(ContentItem item, string row)
        {
            foreach (ContentItem child in item.Children)
            {
                if (child.Title == row)
                    return child;
            }
            return null;
        }

        public override void UpdateEditor(ContentItem item, Control editor)
        {
            TextBox tb = (TextBox)editor;
            tb.Text = string.Empty;
			foreach (ContentItem child in item.Children.WhereAccessible())
            {
                tb.Text += child.Title + Environment.NewLine;
            }
        }

        protected override Control AddEditor(Control container)
        {
            TextBox tb = new TextBox();
            tb.ID = Name;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 5;
            tb.Columns = 40;
            container.Controls.Add(tb);

            return tb;
        }
    }
}
