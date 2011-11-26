using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using N2.Edit.Web;
using N2.Persistence;
using System.IO;
using N2.Definitions;
using System.Web.Mvc;

namespace N2.Management.Content.Export
{
    public partial class Bulk : EditPage
    {
		private CsvParser Parser { get; set; }
		
		private bool isDirty = false;
		protected IEnumerable<CsvRow> Rows { get; set; }
		protected CsvRow FirstRow { get; set; }
		protected List<EditableReference> Editables { get; set; }

		protected override void OnInit(EventArgs e)
		{
			Parser = Engine.Resolve<CsvParser>();
			base.OnInit(e);
		}
		
        public void UploadCommand(object sender, CommandEventArgs args)
        {
            var tfs = Engine.Resolve<TemporaryFileHelper>();
            if(string.IsNullOrEmpty(fuImportFile.PostedFile.FileName))
            {
				rfvImportFile.IsValid = false;
                return;
            }

			var tempFile = Engine.Resolve<TemporaryFileHelper>()
				.GetTemporaryFileName(System.IO.Path.GetExtension(fuImportFile.PostedFile.FileName));
			fuImportFile.PostedFile.SaveAs(tempFile);
			ViewState["file"] = tempFile;

			rfvImportFile.Enabled = false;

			lblLocation.Text = Selection.SelectedItem.Title;

			ddlTypes.DataSource = Engine.Definitions.GetAllowedChildren(Selection.SelectedItem)
				.OrderByDescending(d => d.NumberOfItems / 10)
				.ThenBy(d => d.SortOrder)
				.ThenBy(d => d.Title);
			ddlTypes.DataBind();

			isDirty = true;
        }

		public void ImportCommand(object sender, CommandEventArgs args)
		{
			string tempFile = (string)ViewState["file"];
			char separator;
			var rows = ParseRows(tempFile, out separator);
			if (chkFirstRow.Checked)
				rows = rows.Skip(1);

			var firstRow = rows.FirstOrDefault();
			if (firstRow == null)
				return;

			Dictionary<int, string> mapping = new Dictionary<int, string>();
			for (int i = 0; i < firstRow.Columns.Count; i++)
			{
				var column = firstRow.Columns[i];
				RepeaterItem item = rptPreview.Items[i];
				var ddl = (DropDownList)item.FindControl("ddlColumnMap");
				if (string.IsNullOrEmpty(ddl.SelectedValue))
					continue;

				mapping[i] = ddl.SelectedValue;
			}

			var definition = GetSelectedDefinition();
			var activator = Engine.Resolve<ContentActivator>();
			foreach (var row in rows)
			{
				var importedItem = activator.CreateInstance(definition.ItemType, Selection.SelectedItem, definition.TemplateKey);
				foreach (var kvp in mapping)
				{
					importedItem[kvp.Value] = row.Columns[kvp.Key];
				}
				Engine.Persister.Save(importedItem);
			}
		}

		private IEnumerable<CsvRow> ParseRows(string tempFile, out char separator)
		{
			separator = Parser.GuessBestSeparator(() => File.OpenText(tempFile), ',', '\t', ';');
			return Parser.Parse(separator, File.OpenText(tempFile)).ToList();
		}

		protected void ddlTypes_OnSelectedIndexChanged(object sender, EventArgs args)
		{
			isDirty = true;
		}
		
		protected void chkFirstRow_OnCheckedChanged(object sender, EventArgs args)
		{
			isDirty = true;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (ViewState["file"] != null)
			{
				string tempFile = (string)ViewState["file"];
				char separator;
				var rows = ParseRows(tempFile, out separator);
				if (!rows.Any())
					return;

				BindViewModelTo(separator, rows);

				if (isDirty)
				{
					rptPreview.DataBind();

					if (chkFirstRow.Checked)
					{
						for (int i = 0; i < FirstRow.Columns.Count; i++)
						{
							var column = FirstRow.Columns[i];
							RepeaterItem item = rptPreview.Items[i];
							string bestMatch = GetBestSelection(column);
							var ddl = (DropDownList)item.FindControl("ddlColumnMap");
							ddl.SelectedValue = bestMatch;
						}
					}
				}
			}
		}

		protected class EditableReference
		{
			public string Name { get; set; }
			public string Title { get; set; }
		}

		private void BindViewModelTo(char separator, IEnumerable<CsvRow> rows)
		{
			FirstRow = rows.First();

			if (chkFirstRow.Checked)
			{
				Rows = rows.Skip(1);
			}
			else
			{
				FirstRow = new CsvRow(FirstRow.Columns.Select((col, index) => "Column " + (1 + index)).ToList(), separator);
				Rows = rows;
			}

			Editables = GetEditablesForSelectedDefinition();
		}

		protected List<EditableReference> GetEditablesForSelectedDefinition()
		{
			var editables = GetSelectedDefinition().Editables.OrderBy(ed => ed.SortOrder)
						 .Select(ed => new EditableReference { Name = ed.Name, Title = string.IsNullOrEmpty(ed.Title) ? ed.Name : ed.Title })
						 .ToList();
			editables.Insert(0, new EditableReference { Title = "Ignored", Name = "" });
			return editables;
		}

		protected ItemDefinition GetSelectedDefinition()
		{
			return Engine.Definitions.GetDefinition(ddlTypes.SelectedValue);
		}

		protected string GetBestSelection(string column)
		{
			var bestSelection = Editables.Select(editable => new { editable, similarity = ComputeLevenshteinDistance(editable.Name, column) })
				.OrderBy(x => x.similarity).FirstOrDefault();

			if (bestSelection.similarity <= 2)
				return bestSelection.editable.Name;

			return "";
		}

		public static int ComputeLevenshteinDistance(string s, string t)
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];

			// Step 1
			if (n == 0)
			{
				return m;
			}

			if (m == 0)
			{
				return n;
			}

			// Step 2
			for (int i = 0; i <= n; d[i, 0] = i++)
			{
			}

			for (int j = 0; j <= m; d[0, j] = j++)
			{
			}

			// Step 3
			for (int i = 1; i <= n; i++)
			{
				//Step 4
				for (int j = 1; j <= m; j++)
				{
					// Step 5
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

					// Step 6
					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}
			// Step 7
			return d[n, m];
		}
    }
}