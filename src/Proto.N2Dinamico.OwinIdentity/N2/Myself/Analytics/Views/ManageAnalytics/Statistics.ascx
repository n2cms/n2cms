<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Management.Myself.Analytics.Models.AnalyticsViewModel>" %>

<div class="inner">
	<ul>
	<% foreach (var ge in Model.Entries){ %>
		<li>
		<% foreach (var m in ge.Metrics){ %>
			<%= m.Key %>: <%= m.Value %>
		<%} %>
		-
		<em>
		<% foreach (var d in ge.Dimensions){ %>
			<span><%= d.Key %>: <%= d.Value %></span>
		<%} %>
		</em>
		</li>
	<%} %>
	</ul>
</div>