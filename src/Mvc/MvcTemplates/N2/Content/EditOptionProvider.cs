using System;
using System.Linq;
using N2.Engine;

namespace N2.Edit
{
    [Service]
    public class EditOptionProvider : IProvider<ToolbarOption>
    {
        IEditUrlManager editUrlManager;

        public EditOptionProvider(IEditUrlManager editUrlManager)
        {
            this.editUrlManager = editUrlManager;
        }

        #region IProvider<ToolbarOption> Members
        public ToolbarOption Get()
        {
            return GetAll().FirstOrDefault();
        }

        public System.Collections.Generic.IEnumerable<ToolbarOption> GetAll()
        {
            return new ToolbarOption[]{
                new ToolbarOption{
                    Title = "Form",
                    Target = Targets.Preview,
                    SortOrder = 0,
                    Name = "Form",
                    Url = "{ManagementUrl}/Content/Edit.aspx?{Selection.SelectedQueryKey}={selected}"
                },
                new ToolbarOption{
                    Title = "Organize",
                    Target = Targets.Preview,
                    SortOrder = 1,
                    Name = "Organize",
                    Url = "{url}{query}edit=drag"
                }
            };
        }
        #endregion
    }
}
