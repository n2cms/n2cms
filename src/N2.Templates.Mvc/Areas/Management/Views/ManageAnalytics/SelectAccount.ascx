<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Reimers.Google.Analytics.AnalyticsAccountInfo>>" %>


<div class="uc">
	<h4>Google Analytics: Account Selection</h4>
	<div class="box">
		<div class="inner">	
			<% Html.BeginForm("SaveSelectedAccount", "ManageAnalytics"); %>
			<div>
				<%= Html.DropDownList("ProfileID",
											Model.Select(a => new SelectListItem { 
												Text = a.Title, 
												Value = a.ProfileID.ToString(),
												Selected = a.ProfileID == Html.CurrentItem<ManageAnalyticsPart>().ProfileID
											}))%>
			</div>
			<input type="submit" />
			<% Html.EndForm(); %>
		</div>
	</div>
</div>