using System;
using System.Linq;
using N2.Engine;

namespace N2.Edit
{
	[Service]
	public class ViewOptionProvider : IProvider<ToolbarOption>
	{
		IEditUrlManager editUrlManager;

		public ViewOptionProvider(IEditUrlManager editUrlManager)
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
					Title = "Published",
					Target = Targets.Top,
					SortOrder = 0,
					Name = "Published",
					Url = "{ManagementUrl}/Content/Default.aspx?{Selection.SelectedQueryKey}={selected}"
				},
				new ToolbarOption{
					Title = "Draft",
					Target = Targets.Top,
					SortOrder = 1,
					Name = "Draft",
					Url = "{ManagementUrl}/Content/Default.aspx?{Selection.SelectedQueryKey}={selected}&view=draft"
				}
			};
		}
		#endregion
	}
}