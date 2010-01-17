<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.master" AutoEventWireup="true" 
	Inherits="System.Web.Mvc.ViewPage<GalleryItem>" Title="" %>
<%@ Import Namespace="N2.Collections"%>
<%@ Import Namespace="N2.Templates.Mvc.Models.Pages"%>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<script type="text/javascript" src="<%=ResolveUrl("~/Content/Scripts/jquery-1.3.2.min.js")%>"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentAndSidebar" runat="server">
	<% Html.RenderAction<NavigationController>(c => c.Breadcrumb()); %>
	
	<%=Html.Display(m => m.Title)%>
	
	<img alt="<%=Model.Title%>" src="<%=ResolveUrl(Model.ResizedImageUrl)%>" />
	
	<%=Html.Display(m => m.Text)%>
</asp:Content>