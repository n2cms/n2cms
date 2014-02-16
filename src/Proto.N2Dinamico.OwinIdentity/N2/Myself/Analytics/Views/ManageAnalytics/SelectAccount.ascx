<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="N2.Web.Mvc.Html" %>
<%@ Import Namespace="N2.Management.Myself.Analytics.Models" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Reimers.Google.Analytics.AnalyticsAccountInfo>>" %>


<div class="uc">
	<h4 class="header">Report profile selection</h4>
	<div class="box">
		<div class="inner">	
			<% Html.BeginForm("SaveSelectedAccount", "ManageAnalytics"); %>
			<div>
				<label>Profile: 
				<%= Html.DropDownList("ProfileID",
											Model.Select(a => new SelectListItem {
												Text = a.Title + " (" + a.AccountName + ")", 
												Value = a.ProfileID.ToString(),
												Selected = a.ProfileID == Html.CurrentItem<ManageAnalyticsPart>().ProfileID
											}))%>
				</label>
			</div>
			<div class="buttons">
				<input type="submit" value="Save" />
			</div>
			<% Html.EndForm(); %>
		</div>
	</div>
</div>