using System.Collections.Generic;

[N2.Definition("Start Page")]
[N2.Integrity.RestrictParents(typeof(RootPage))]
public class StartPage : PageItem, N2.Web.ISitesSource
{
	[N2.Details.EditableTextBox("Host", 100)]
	public virtual string Host
	{
		get { return (string)(GetDetail("Host") ?? string.Empty); }
		set { SetDetail("Host", value, string.Empty); }
	}

	public IEnumerable<N2.Web.Site> GetSites()
	{
		yield return new N2.Web.Site(Parent.ID, ID, Host);
	}

	public override string IconUrl
	{
		get { return "~/Edit/img/ico/page_world.gif"; }
	}
}
