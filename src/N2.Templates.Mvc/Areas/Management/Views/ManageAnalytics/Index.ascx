<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Templates.Mvc.Areas.Management.Models.ManageAnalyticsPart>" %>
<% string containerID = "a" + Html.CurrentItem().ID; %>

<div id="<%= containerID %>" class="uc cf analytics">
	<h4 class="header">Google Analytics: <%= Model.AccountName %> &gt; <%= Model.Title %></h4>
	<div class="box">
		<div class="inner"><div class="chart">&nbsp;</div></div>
			
		<div class="alter inner">
			<% using (Html.BeginForm("ClearUser", "ManageAnalytics")){ %>
				<input type="submit" value="Clear login" />
			<% } %>
			<% using (Html.BeginForm("ChangeAccount", "ManageAnalytics")) { %>
				<input type="submit" value="Select account" />
			<% } %>
			<% using (Html.BeginForm("Reconfigure", "ManageAnalytics")) { %>
				<input type="submit" value="Reconfigure" />
			<% } %>
		</div>
	</div>
</div>
<style>
	.analytics .alter { height:20px; margin-top:5px; border-top:solid 1px silver; }
	.analytics .alter form { float:left; }
	.analytics .alter form input { font-size:9px; }
	.chart { height:300px; width:100%; }
	.chart canvas { position:absolute; }
</style>


	
<script src="<%= ResolveClientUrl("../../Resources/jquery.flot.js") %>" type="text/javascript"></script>
<script type="text/javascript">
	(function($) {
		$.post('<%= Url.Action("StatisticsData") %>', function(data) {
			data.options.legend = { position: "nw" }
			$.plot($("#<%= containerID %> .chart"), data.values, data.options);
		});
	})(jQuery);
</script>