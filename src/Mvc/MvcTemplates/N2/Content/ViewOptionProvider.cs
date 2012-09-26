using System;
using System.Linq;
using N2.Engine;
using N2.Web;
using N2.Configuration;

namespace N2.Edit
{
	[Service]
	public class ViewOptionProvider : IProvider<ToolbarOption>
	{
		IEditUrlManager editUrlManager;
		private IWebContext webContext;
		private ViewPreference defaultViewPreference;

		public ViewOptionProvider(IEditUrlManager editUrlManager, IWebContext webContext, EditSection config)
		{
			this.editUrlManager = editUrlManager;
			this.webContext = webContext;
			this.defaultViewPreference = config.Versions.DefaultViewMode;
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
					Url = editUrlManager.GetEditInterfaceUrl(ViewPreference.Published),
					Selected = webContext.HttpContext.GetViewPreference(defaultViewPreference) == ViewPreference.Published
				},
				new ToolbarOption{
					Title = "Draft",
					Target = Targets.Top,
					SortOrder = 1,
					Name = "Draft",
					Url = editUrlManager.GetEditInterfaceUrl(ViewPreference.Draft),
					Selected = webContext.HttpContext.GetViewPreference(defaultViewPreference) == ViewPreference.Draft
				}
			};
		}
		#endregion
	}
}