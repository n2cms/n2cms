<%@ Page MasterPageFile="~/Templates/UI/Layouts/Top+Submenu.master" Language="C#" AutoEventWireup="true" CodeBehind="EditAddon.aspx.cs" Inherits="N2.Addons.AddonCatalog.UI.EditAddon" %>
<%@ Import Namespace="N2.Web"%>
<%@ Register TagPrefix="n2" TagName="AddonEditForm" Src="AddonEditForm.ascx" %>
<asp:Content ContentPlaceHolderID="Content" runat="server">
    <h1>Change Add-On</h1>
    <n2:AddonEditForm runat="server" />
</asp:Content>
