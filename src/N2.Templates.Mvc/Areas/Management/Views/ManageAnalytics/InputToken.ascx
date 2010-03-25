<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ManageAnalyticsPart>" %>

<div class="uc">
	<h4>Google Analytics username & password</h4>
	<div class="box">
		<div class="inner">	
			<% Html.BeginForm("InputToken", "ManageAnalytics"); %>
			<%= Html.ValidationSummary() %>
			<div>
				<%= Html.LabelFor(m => m.Username) %>
				<%= Html.EditorFor(m => m.Username)%>
			</div>
			<div>
				<%= Html.LabelFor(m => m.Password) %>
				<%= Html.EditorFor(m => m.Password)%>
			</div>
			<input type="submit" />
			<% Html.EndForm(); %>
		</div>
	</div>
</div>