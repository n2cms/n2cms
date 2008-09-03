<%@ Page Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.master" Theme="ContentFocus" AutoEventWireup="true" CodeBehind="SiteMap.aspx.cs" Inherits="N2.Templates.UI.Views.SiteMap" Title="Site map" %>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
    <n2:EditableDisplay ID="d1" PropertyName="Title" runat="server" />
    <n2:EditableDisplay ID="d2" PropertyName="Text" runat="server" />
    <n2:Menu ID="m" runat="server" Path="~/" BranchMode="false" />
</asp:Content>
