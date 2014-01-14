<%@ Page MasterPageFile="~/Templates/UI/Layouts/Top+Submenu.master" Language="C#" AutoEventWireup="true" CodeBehind="MyPage.aspx.cs" Inherits="N2.Addons.MyAddon.UI.MyPage" %>
<%@ Import Namespace="N2.Addons.MyAddon.UI"%>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    <h1><%= CurrentPage.Title %></h1>
    <div><%= CurrentPage.Text %></div>
</asp:Content>
