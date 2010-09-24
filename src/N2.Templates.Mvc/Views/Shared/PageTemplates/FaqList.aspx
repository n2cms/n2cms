<%@ Page Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<FaqList>" Title="" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<link href="<%=ResolveUrl("~/Content/Css/Faq.css") %>" type="text/css" rel="stylesheet" />
</asp:Content>

<asp:Content ContentPlaceHolderID="PostContent" runat="server">
	<div class="list">
	<% Html.DroppableZone(Zones.Questions).WrapIn("div", new { @class = "uc faq" }).Render(); %>
	</div>
</asp:Content>