<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Templates.Mvc.Areas.Tests.Models.QueryViewData>" %>

	<style>div pre { display:none; }</style>
	<div onclick="document.body.innerHTML=this.innerHTML">NH Queries: <%= Model.Queries().Count%> <a href="#show">(+)</a><br />
		<pre style="background-color:#fff;padding:5px;"><% foreach (string q in Model.Queries()) { %>
<% string fq = q.Replace("SELECT", "\r\n<b>SELECT</b>").Replace("FROM", "\r\n\t<b>FROM</b>").Replace("WHERE", "\r\n\t<b>WHERE</b>").Replace("ORDER BY", "\r\n\t<b>ORDER BY</b>")
		.Replace("select", "\r\n<b>SELECT</b>").Replace("from", "\r\n\t<b>FROM</b>").Replace("where", "\r\n\t<b>WHERE</b>").Replace("order by", "\r\n\t<b>ORDER BY</b>")
		.Replace("left outer join", "\r\n\t\t<b>left outer join</b>").Replace("right outer join", "\r\n\t\t<b>right outer join</b>").Replace("inner join", "\r\n\t\t<b>inner join</b>")
		.Replace(";", ";\r\n\t\t"); %>
<% string[] colors = new string[] { "#f00", "#0f0", "#00f", "#c11", "#1c1", "#11c", "#a22", "#2a2", "#22a", "#733" }; %>
<% fq = Regex.Replace(fq, "@p([0-9]+)", delegate(Match m) { return "<b style='color:" + colors[int.Parse(m.Groups[1].Value) % colors.Length] + "'><em>" + m.Value + "</em></b>"; }); %>
<%= fq %>
<% } %></pre>
	</div>
	<div onclick="document.body.innerHTML=this.innerHTML">NH Events: <%= Model.All().Count%> <a href="#show">(+)</a><br />
		<pre style="background-color:#fff;padding:5px;"><% foreach (string q in Model.All()) { %>
<%= q %>
<% } %></pre>
	</div>
	<div>Collections: <%= Html.ResolveService<N2.Persistence.NH.ISessionProvider>().OpenSession.Session.Statistics.CollectionCount %></div>
	<div>Entities: <%= Html.ResolveService<N2.Persistence.NH.ISessionProvider>().OpenSession.Session.Statistics.EntityCount%></div>
	<div>Time: <%= (int)(DateTime.Now - Context.Timestamp).TotalMilliseconds %> ms</div>
