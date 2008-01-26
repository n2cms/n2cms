using System;
using System.Collections.Specialized;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using N2.Templates.Scrum.Items;

namespace N2.Templates.Scrum.Commands
{
	public class MakeCurrentSprint : N2.Web.IAjaxService
	{
		public string Name
		{
			get { return "makeCurrentSprint"; }
		}

		public bool RequiresEditAccess
		{
			get { return false; }
		}

		public NameValueCollection Handle(NameValueCollection request)
		{
			ScrumSprint sprint = Context.Persister.Get<ScrumSprint>(int.Parse(request["item"]));
			ScrumProject project = sprint.Project;
			project.CurrentSprint = sprint;
			Context.Persister.Save(project);
			return new NameValueCollection();
		}
	}
}
