<%@ Import Namespace="N2.Management.Myself.Analytics.Models" %>
<%@ Import Namespace="N2.Web.Mvc" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ManageAnalyticsPart>" %>

<div class="uc">
	<h4 class="header">Google Username &amp; Password</h4>
	<div class="box">
		<div class="inner">	
			<% Html.BeginForm("InputToken", "ManageAnalytics"); %>
			<%= Html.ValidationSummary() %>
			<div>
				<label>Username: <input name="username" /></label>
			</div>
			<div>
				<label>Password: <input name="password" type="password" /></label>
			</div>
			<div class="buttons">
			<input type="submit" value="Save" />
			Your password is not saved but all editors will see the statistics.
			</div>
			<% Html.EndForm(); %>
		</div>
	</div>
</div>