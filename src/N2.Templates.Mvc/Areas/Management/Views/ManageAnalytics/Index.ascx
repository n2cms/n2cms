<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Templates.Mvc.Areas.Management.Models.ManageAnalyticsPart>" %>
<div class="uc">
	<h4 class="header">Google Analytics</h4>
	<div class="box">
		<% using(Html.BeginAsyncAction("Statistics")) { %>
			...
		<% } %>
	</div>
</div>
