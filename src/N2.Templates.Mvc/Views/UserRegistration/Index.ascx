﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Register.ascx.cs" Inherits="N2.Web.Mvc.ContentViewUserControl<UserRegistrationModel, UserRegistration>" %>

<div class="uc">
	<%=Html.ValidationSummary()%>

	<%using(Html.BeginForm("Submit", "UserRegistration", FormMethod.Post)){%>
		<%=Html.AntiForgeryToken("register") %>
		
		<div class="inputForm">
			<div class="row cf"><label class="label" for="RegisterUserName">User Name:</label><%= Html.TextBox("RegisterUserName") %></div>
			<div class="row cf"><label class="label" for="RegisterPassword">Password:</label><%= Html.Password("RegisterPassword", "")%></div>
			<div class="row cf"><label class="label" for="RegisterConfirmPassword">Confirm Password:</label><%= Html.Password("RegisterConfirmPassword", "")%></div>
			<div class="row cf"><label class="label" for="RegisterEmail">Email:</label><%= Html.TextBox("RegisterEmail")%></div>
			<div class="row cf"><input type="submit" /></div>
		</div>
	<%} %>
</div>