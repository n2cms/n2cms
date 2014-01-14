<%@ Page Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<FaqList>" Title="" %>

<asp:Content ContentPlaceHolderID="PostContent" runat="server">
	<div class="list">
	<% Html.DroppableZone(Zones.Questions).WrapIn("div", new { @class = "uc faq" }).Render(); %>
	</div>
</asp:Content>
