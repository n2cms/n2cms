using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Edit.Installation;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Details;
using N2.Persistence.NH;

namespace N2.Management.Installation
{
	public partial class Rebase : System.Web.UI.Page
	{
		protected IEnumerable<RebaseInfo> RebasedLinks;
		protected InstallationManager Installer
		{
			get { return N2.Context.Current.Resolve<InstallationManager>(); }
		}

		private DatabaseStatus status;
		protected DatabaseStatus Status
		{
			get
			{
				if (status == null)
					status = Installer.GetStatus();
				return status;
			}
		}
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btnRebase_Click(object sender, EventArgs e)
		{
			var rebaser = N2.Context.Current.Resolve<AppPathRebaser>();
			RebasedLinks = rebaser.Rebase(Status.AppPath, ResolveUrl("~/"));
		}
	}
}