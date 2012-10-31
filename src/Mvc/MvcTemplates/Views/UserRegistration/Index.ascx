<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Register.ascx.cs" Inherits="N2.Web.Mvc.ContentViewUserControl<UserRegistrationModel, UserRegistration>" %>

<div class="uc">
	<%=Html.ValidationSummary()%>

	<%using(Html.BeginForm("Submit", "UserRegistration", FormMethod.Post)){%>
		<%=Html.AntiForgeryToken() %>
		
		<div class="inputForm">
			<div class="row cf"><label class="label" for="RegisterUserName">User Name:</label><%= Html.TextBoxFor(m => m.RegisterUserName) %><%= Html.ValidationMessageFor(m => m.RegisterUserName) %></div>
			<div class="row cf"><label class="label" for="RegisterPassword">Password:</label><%= Html.PasswordFor(m => m.RegisterPassword) %><%= Html.ValidationMessageFor(m => m.RegisterPassword) %></div>
			<div class="row cf"><label class="label" for="RegisterConfirmPassword">Confirm Password:</label><%= Html.PasswordFor(m => m.RegisterConfirmPassword) %><%= Html.ValidationMessageFor(m => m.RegisterConfirmPassword) %></div>
			<div class="row cf"><label class="label" for="RegisterEmail">Email:</label><%= Html.TextBoxFor(m => m.RegisterEmail) %><%= Html.ValidationMessageFor(m => m.RegisterEmail) %></div>
			<div class="row cf"><input type="submit" /></div>
		</div>
	<%} %>
</div>