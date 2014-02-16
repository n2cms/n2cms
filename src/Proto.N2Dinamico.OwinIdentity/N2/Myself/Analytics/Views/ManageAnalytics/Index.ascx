<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="N2.Web" %>
<%@ Import Namespace="N2.Web.Mvc.Html" %>
<%@ Import Namespace="N2.Management.Myself.Analytics.Models" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Management.Myself.Analytics.Models.ManageAnalyticsPart>" %>

<% string containerID = "a" + Html.CurrentItem().ID; %>

<div id="<%= containerID %>" class="uc cf analytics">
	<% using (Html.BeginForm("ClearUser", "ManageAnalytics", FormMethod.Post, new { title = "Log out from google analytics. This affects all users." })) { %>
	<h4 class="header">
		<span title="<%= Model.AccountName %>"><%= Model.Title %></span>
		<span class="right">
			<a title="Change profile" href="<%= Html.CurrentPage().Url.ToUrl().AppendQuery("changeaccount", Model.ID) %>">Profile</a>
			| <a title="Reconfigure report" href="<%= Html.CurrentPage().Url.ToUrl().AppendQuery("reconfigure", Model.ID) %>">Report</a>
			<input type="submit" value="Logout" />
		</span>
	</h4>
	<% } %>
	<div class="box">
		<div class="inner"><div class="chart">&nbsp;</div></div>
	</div>
</div>

<style>
	.analytics .alter { margin-top:5px; border-top:solid 1px silver; }
	.analytics .right { float:right; margin-top:-1px }
	.analytics .right a { padding:0 5px; font-size:10px; color:#fff; }
	.analytics .right input { font-size:10px; margin-top:-2px; }
	.chart { height:300px; width:100%; }
	.chart canvas { position:absolute; }
	.chart .tickLabel { background-color:#fff; padding-right:2px; }
</style>

<!--[if IE]><script language="javascript" type="text/javascript" src="<%= ResolveClientUrl("../../Resources/excanvas.min.js") %>"></script><![endif]-->
<script src="<%= ResolveClientUrl("../../Resources/jquery.flot.min.js") %>" type="text/javascript"></script>
<script type="text/javascript">
	(function($) {
		$.post('<%= Url.Action("StatisticsData") %>', function(data) {
			data.options.legend = { position: "nw" }
			$.plot($("#<%= containerID %> .chart"), data.values, data.options);
		});
	})(jQuery);
</script>