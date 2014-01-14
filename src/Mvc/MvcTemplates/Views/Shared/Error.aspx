<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" Inherits="System.Web.Mvc.ViewPage<ContentItem>" %>
<%@ Import Namespace="N2"%>

<asp:Content ID="errorContent" ContentPlaceHolderID="ContentAndSidebar" runat="server">
	<h1>Server error</h1>
	<p>
		Sorry, an error occurred while processing your request.
	</p>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Menu" />
