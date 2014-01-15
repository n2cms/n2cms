using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Engine;
using N2.Edit;

namespace N2.Management.Content.Export
{
    [Service]
    public class BulkOptionProvider : IProvider<ToolbarOption>
    {
        public ToolbarOption Get()
        {
            return GetAll().FirstOrDefault();
        }

        public IEnumerable<ToolbarOption> GetAll()
        {
            yield return new ToolbarOption { Title = Utility.GetResourceString("Bulk", "Import.Title") ?? "Import from file", Name = "BulkImort", Target = Targets.Preview, Url = "{ManagementUrl}/Content/Export/Default.aspx?{Selection.SelectedQueryKey}={selected}", SortOrder = 0 };
            yield return new ToolbarOption { Title = Utility.GetResourceString("Bulk", "Export.Title") ?? "Export to file", Name = "BulkExport", Target = Targets.Preview, Url = "{ManagementUrl}/Content/Export/Export.aspx?{Selection.SelectedQueryKey}={selected}", SortOrder = 1 };
            yield return new ToolbarOption { Title = Utility.GetResourceString("Bulk", "Editing.Title") ?? "Bulk editing", Name = "BulkEditing", Target = Targets.Preview, Url = "{ManagementUrl}/Content/Export/BulkEditing.aspx?{Selection.SelectedQueryKey}={selected}", SortOrder = 2 };
        }
    }
}
