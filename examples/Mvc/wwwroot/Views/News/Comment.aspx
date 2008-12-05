<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Comment.aspx.cs" Inherits="MvcTest.Views.News.Comment" %>
<%@ Import Namespace="MvcTest.Controllers"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<% Html.BeginForm<NewsController>(c => c.Submit(null, null)); %>
		<h1><%= string.Format("Add comment to '{0}'", ViewData.Model.Title) %></h1>
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
	<% Html.EndForm(); %>
</asp:Content>
