<%@ Import Namespace="N2.Web" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Templates.Mvc.Areas.Management.Models.ManageAnalyticsPart>" %>
<% string containerID = "a" + Html.CurrentItem().ID; %>

<div id="<%= containerID %>" class="uc cf analytics">
	<% using (Html.BeginForm("ClearUser", "ManageAnalytics", FormMethod.Post, new { onsubmit = "return confirm('Logout from google analytics?');" })) { %>
	<h4 class="header">Google Analytics: <%= Model.AccountName %> &gt; <%= Model.Title %>
			<input type="submit" value="Logout" />
	</h4>
	<% } %>
	<div class="box">
		<div class="inner"><div class="chart">&nbsp;</div></div>
			
		<div class="alter inner">
			<a href="<%= Html.CurrentPage().Url.ToUrl().AppendQuery("changeaccount", Model.ID) %>">Switch profile</a>
			|
			<a href="<%= Html.CurrentPage().Url.ToUrl().AppendQuery("reconfigure", Model.ID) %>">Configure report</a>
		</div>
	</div>
</div>

<style>
	.analytics .alter { margin-top:5px; border-top:solid 1px silver; }
	.analytics .alter a { padding:0 10px; }
	.analytics form .header input { float:right; font-size:9px; margin-top:-2px; }
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