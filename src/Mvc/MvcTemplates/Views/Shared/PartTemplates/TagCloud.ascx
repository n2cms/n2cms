<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<TagCloud>" %>
<div class="uc tagcloud">
	<h4><%=Model.Title%></h4>
	<div class="box"><div class="inner">
		<% var page = Find.ClosestPage(Model); %>
		<% var tags = Html.ContentEngine().Resolve<N2.Persistence.TagsRepository>().FindTagSizes(Model.Container ?? page, "Tags").ToList(); %>
		<% var max = tags.Max(t => t.Value); %>
		<% foreach (var tag in tags) { %>
		<%= Html.ActionLink(tag.Key, page, new { tag = tag.Key }, new { @class = "freq" + (9 * tag.Value / max) }) %>
		<% } %>
	</div></div>
</div>