<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="MvcTest.Views.News.Index" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<h1><%= ViewData.News.Title %></h1>
	<%= ViewData.News.Text%>
	<div class="comments">
	<% foreach (MvcTest.Models.CommentItem comment in ViewData.Comments){ %>
		<div>
			<h3><%= comment.Title %></h3>
			<%= comment.Text %>
		</div>
	<%} %>
	</div>
	<hr />
	<%= N2.Web.Link.To(ViewData.Back).Text("Back").Class("back") %>
	<span class="comment"><%= Html.ActionLink("Comment", "comment", new { })%></span>
	<%= Html.ActionLink<MvcTest.Controllers.NewsController>(x => x.Comment(), "Comment", new {Class = "comment"}) %>
</asp:Content>	
