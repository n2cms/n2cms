<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Comment.aspx.cs" Inherits="MvcTest.Views.News.Comment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<% using(Html.Form("NewsContainer", "submit")) { %>
		<h1><%= string.Format("Add comment to '{0}'", ViewData.Title) %></h1>
		<p>
			<label>Subject</label>
			<%= Html.TextBox("title") %>
		</p>
		<p>
			<label>Comment</label>
			<%= Html.TextArea("text", "") %>
		</p>
		<p>
			<%= Html.SubmitButton() %>
			<%= Html.ActionLink("Back", "index") %>
		</p>
	<% } %>
</asp:Content>
