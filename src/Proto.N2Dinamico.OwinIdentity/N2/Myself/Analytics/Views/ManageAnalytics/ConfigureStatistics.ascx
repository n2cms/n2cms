<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="N2.Management.Myself.Analytics.Models" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Management.Myself.Analytics.Models.ConfigureAnalyticsViewModel>" %>

<style>
	.analytics label { white-space: nowrap; float:left; }
</style>

<div class="uc analytics">
	<h4 class="header">Configure Report Data</h4>
	<div class="box">
		<div class="inner">	
			<% Html.BeginForm("SaveStatisticsConfiguration", "ManageAnalytics"); %>
			<%= Html.ValidationSummary() %>
			<div>
				<label>
					Show values from last:
					<%= Html.DropDownListFor(m => m.Period, Model.Periods) %>			
				</label>
				<label>
					Dimension: 
					<%= Html.DropDownList(
							"Dimension",
							Model.AllDimensions.Select(d => 
								new SelectListItem { Value = d.ToString(), Text = d.ToString().SplitWords(), Selected = Model.SelectedDimensions.Contains(d)}))%>
						
				</label>
			</div>
			<div style="clear:both">
				<fieldset><legend>Metrics</legend>
				<% foreach(var metric in Model.AllMetrics) { %>
				<label>
					<input type="checkbox" name="Metric" <%= Model.SelectedMetrics.Contains(metric) ? "checked='checked'" : "" %> value="<%= metric %>" />
					<%= metric.ToString().SplitWords() %>
				</label>
				<% } %>
				</fieldset>
			</div>
			<div class="buttons">
				<input type="submit" value="Save" />
				No data? Choosing less metrics and shorter time periods might help
			</div>
			<% Html.EndForm(); %>
		</div>
	</div>
</div>