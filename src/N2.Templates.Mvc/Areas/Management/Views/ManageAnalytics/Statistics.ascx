<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Templates.Mvc.Areas.Management.Models.AnalyticsViewModel>" %>

<div class="inner">
	<ul>
	<% foreach (var ge in Model.Entries){ %>
		<li>
		<% foreach (var m in ge.Metrics){ %>
			<%= m.Key %>: <%= m.Value %>
		<%} %>
		-
		<% foreach (var d in ge.Dimensions){ %>
			<%= d.Key %>: <%= d.Value %>
		<%} %>
		</li>
	<%} %>
	</ul>
	
	<fieldset><legend>Clear</legend>
	<% using (Html.BeginForm("ClearUser", "ManageAnalytics")){ %>
		<input type="submit" value="Login" />
	<% } %>
	<% using (Html.BeginForm("ClearAccount", "ManageAnalytics")){ %>
		<input type="submit" value="Account" />
	<% } %>
	<% using (Html.BeginForm("ClearConfiguration", "ManageAnalytics")){ %>
		<input type="submit" value="Dimensions & Metrics" />
	<% } %>
	</fieldset>
</div>