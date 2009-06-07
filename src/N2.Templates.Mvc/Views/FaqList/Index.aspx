<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.master" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewPage<FaqList>" Title="" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<link href="<%=ResolveUrl("~/Content/Css/Faq.css") %>" type="text/css" rel="stylesheet" />
</asp:Content>

<asp:Content ContentPlaceHolderID="PostContent" runat="server">
	<div class="list">
	<%=this.DroppableZone(Zones.Questions).WrapIn("div", new{@class="uc faq"}) %>
	</div>
</asp:Content>