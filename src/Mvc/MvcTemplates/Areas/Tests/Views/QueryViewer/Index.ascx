<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Templates.Mvc.Areas.Tests.Models.QueryViewData>" %>
<div class="uc"><div class="box"><div class="inner">
	<style>div pre { display:none; }</style>

	<div onclick="document.body.innerHTML=this.innerHTML">NH Queries: <%= Model.Queries().Count%> <a href="#show">(+)</a><br />
	 <style>.stacktrace { height:1.2em; overflow:hidden; font-family:Monospace; } .stacktrace:hover { height:auto; } .sql { border-bottom:solid 1px silver;</style>
<% DateTime? last = null; %>
		<pre style="background-color:#fff;padding:5px;"><% foreach (var q in Model.Queries()) { %>
<% string fq = q.Sql.Replace("SELECT", "\r\nSELECT").Replace("FROM", "\r\n\tFROM").Replace("WHERE", "\r\n\tWHERE").Replace("ORDER BY", "\r\n\tORDER BY")
		.Replace("select", "\r\nSELECT").Replace("from", "\r\n\tFROM").Replace("where", "\r\n\tWHERE").Replace("order by", "\r\n\tORDER BY")
		.Replace(";", ";\r\n\t\t"); %>
<% string[] colors = new string[] { "#f00", "#0f0", "#00f", "#c11", "#1c1", "#11c", "#a22", "#2a2", "#22a", "#733" }; %>
<% fq = Regex.Replace(fq, "(FROM )(\\w+)", delegate(Match m) { return m.Groups[1] + "<b style='background-color:#eee'>" + m.Groups[2] + "</b>"; }); %>
<% fq = Regex.Replace(fq, "(join )(\\w+)", delegate(Match m) { return m.Groups[1] + "<b style='background-color:#eee'>" + m.Groups[2] + "</b>"; }); %>
<% fq = Regex.Replace(fq, "(\\w+)=@", delegate(Match m) { return "<span style='background-color:#fee'>" + m.Groups[1] + "</span>=@"; }); %>
<% fq = Regex.Replace(fq, "@p([0-9]+)", delegate(Match m) { return "<b style='color:" + colors[int.Parse(m.Groups[1].Value) % colors.Length] + "'><em>" + m.Value + "</em></b>"; }); %>
<% fq = fq.Replace("left outer join", "\r\n\t\t<em>left outer join</em>").Replace("right outer join", "\r\n\t\t<em>right outer join</em>").Replace("inner join", "\r\n\t\t<em>inner join</em>"); %>
		<div class="stacktrace"><em><% if (!last.HasValue) { Response.Write(q.Time); } else { Response.Write("+" + q.Time.Subtract(last.Value).TotalSeconds); } last = q.Time; %>:</em> <b><%= q.Caller %></b></div><div class="sql"><%= fq %></div>
<% } %></pre>
	</div>
	<div onclick="document.body.innerHTML=this.innerHTML">NH Events: <%= Model.All().Count%> <a href="#show">(+)</a><br />
		<pre style="background-color:#fff;padding:5px;"><% foreach (var q in Model.All()) { %>
<%= q.Sql %>
<% } %></pre>
	</div>
	<div>Collections: <%= Html.ResolveService<N2.Persistence.NH.ISessionProvider>().OpenSession.Session.Statistics.CollectionCount %></div>
	<div>Entities: <%= Html.ResolveService<N2.Persistence.NH.ISessionProvider>().OpenSession.Session.Statistics.EntityCount%></div>
	<div>Elapsed: <%= (int)(N2.Utility.CurrentTime() - Context.Timestamp).TotalMilliseconds %> ms</div>
	<div>Time: <%= N2.Utility.CurrentTime() %></div>
</div></div></div>