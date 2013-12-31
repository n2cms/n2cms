<%@ Page MasterPageFile="~/Templates/UI/Layouts/Top+Submenu.master" Language="C#" AutoEventWireup="true" CodeBehind="AddonCatalogPage.aspx.cs" Inherits="N2.Addons.AddonCatalog.UI.AddonCatalogPage" %>
<%@ Import Namespace="N2.Web"%>
<asp:Content ContentPlaceHolderID="Content" runat="server">
    <h1><%= CurrentPage.Title %></h1>
    <div><%= CurrentPage.Text %></div>
    
    <asp:PlaceHolder runat="server" ID="phAddons" />
    
    <a href="<%= Url.Parse(CurrentPage.Url).AppendSegment("add") %>"><img src="<%= Url.ToAbsolute("~/Addons/AddonCatalog/UI/plugin_add.png") %>" /> Submit Addon</a>
</asp:Content>
