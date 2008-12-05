<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="MvcTest.Views.News.Index" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<h1><%= ViewData.Model.News.Title %></h1>
	<%= ViewData.Model.News.Text%>
	<div class="comments">
	<% foreach (MvcTest.Models.CommentItem comment in ViewData.Model.Comments){ %>
		<div>
			<h3><%= comment.Title %></h3>
			<%= comment.Text %>
		</div>
	<%} %>
	</div>
	<hr />
	<%= N2.Web.Link.To(ViewData.Model.Back).Text("Back").Class("back") %>
	<%= Html.ActionLink<MvcTest.Controllers.NewsController>(c => c.Comment(), "Comment", new {Class = "comment"}) %>
</asp:Content>	
