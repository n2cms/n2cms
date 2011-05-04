<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
	[<%= Html.CurrentItem().ID %>]
	[<%= Html.CurrentItem().Name %>]
</asp:Content>
