<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<NewsViewData>" %>
<%@ Import Namespace="MvcTest.Views.News"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<h1><%= Model.News.Title %></h1>
	<%= Model.News.Text%>
	<div class="comments">
	<% foreach (MvcTest.Models.CommentItem comment in Model.Comments){ %>
		<div>
			<h3><%= comment.Title %></h3>
			<%= comment.Text %>
		</div>
	<%} %>
	</div>
	<hr />
	<%= N2.Web.Link.To(Model.Back).Text("Back").Class("back") %>
	<%= Html.ActionLink("Comment", "Comment", new { @class = "comment" }) %>
</asp:Content>	
