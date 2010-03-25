<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Templates.Mvc.Areas.Management.Models.ConfigureAnalyticsViewModel>" %>

<style>
	.analytics label { white-space: nowrap; float:left; }
</style>

<div class="uc analytics">
	<h4>Google Analytics Dimensions & Metrics</h4>
	<div class="box">
		<div class="inner">	
			<% Html.BeginForm("SaveStatisticsConfiguration", "ManageAnalytics"); %>
			<%= Html.ValidationSummary() %>
			<div>
				<fieldset><legend>Dimensions</legend>
				<% foreach(var dimension in Model.AllDimensions) { %>
				<label>
					<input type="checkbox" name="Dimension" <%= Model.SelectedDimensions.Contains(dimension) ? "checked='checked'" : "" %> value="<%= dimension %>" />
					<%= dimension.ToString().SplitWords()%>
				</label>
				<% } %>
				</fieldset>
				<fieldset><legend>Metrics</legend>
				<% foreach(var metric in Model.AllMetrics) { %>
				<label>
					<input type="checkbox" name="Metric" <%= Model.SelectedMetrics.Contains(metric) ? "checked='checked'" : "" %> value="<%= metric %>" />
					<%= metric.ToString().SplitWords() %>
				</label>
				<% } %>
				</fieldset>
			</div>
			<input type="submit" />
			<% Html.EndForm(); %>
		</div>
	</div>
</div>