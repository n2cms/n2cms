<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Register.ascx.cs" Inherits="N2.Web.Mvc.N2ModelViewUserControl<UserRegistrationModel, UserRegistration>" %>
<%@ Import Namespace="MvcContrib.FluentHtml"%>

<%=Html.ValidationSummary()%>

<%using(Html.BeginForm<UserRegistrationController>(c => c.Submit(null), FormMethod.Post)){%>
	<%=Html.AntiForgeryToken("register") %>
	
	<div class="inputForm">
		<div class="row cf"><%=Model.TextBox(m => m.UserName).Label("User Name:", "label")%></div>
		<div class="row cf"><%=Model.Password(m => m.Password).Label("Password:", "label").Value("")%></div>
		<div class="row cf"><%=Model.Password(m => m.ConfirmPassword).Label("Confirm Password:", "label").Value("")%></div>
		<div class="row cf"><%=Model.TextBox(m => m.Email).Label("Email:", "label")%></div>
		<div class="row cf"><%=Model.SubmitButton("Create User")%></div>
	</div>
<%} %>