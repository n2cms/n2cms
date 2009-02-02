<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<NewsPage>" %>
<%@ Import Namespace="MvcTest.Models"%>
<%@ Import Namespace="MvcTest.Controllers"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<% Html.BeginForm<NewsController>(c => c.Submit(null, null)); %>
		<h1><%= string.Format("Add comment to '{0}'", Model.Title) %></h1>
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
