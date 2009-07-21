<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.Master" Inherits="N2.Web.Mvc.N2ViewPage<ContentItem>" %>
<%@ Import Namespace="N2"%>

<asp:Content ID="errorContent" ContentPlaceHolderID="ContentAndSidebar" runat="server">
	<h1>Server error</h1>
	<p>
		Sorry, an error occurred while processing your request.
	</p>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Menu" />