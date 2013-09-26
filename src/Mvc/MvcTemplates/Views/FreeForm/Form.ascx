<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>


<div class="uc freeform">
	<%= Html.ValidationSummary() %>
	<% using (Html.BeginForm("Submit", null, FormMethod.Post, new { enctype = "multipart/form-data", @class = "form-horizontal" })){ %>
		<%= Html.DisplayContent("Form") %>
	<% } %>
</div>