<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Reimers.Google.Analytics.AnalyticsAccountInfo>>" %>


<div class="uc">
	<h4>Google Analytics Account</h4>
	<div class="box">
		<div class="inner">	
			<% Html.BeginForm("SaveSelectedAccount", "ManageAnalytics"); %>
			<div>
				<%= Html.DropDownList("AccountID", 
						Model.Select(a => new SelectListItem { Text = a.Title, Value = a.AccountID.ToString() })) %>
			</div>
			<input type="submit" />
			<% Html.EndForm(); %>
		</div>
	</div>
</div>