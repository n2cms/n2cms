<%@ Page MasterPageFile="~/Templates/UI/Layouts/Top+Submenu.master" Language="C#" AutoEventWireup="true" CodeBehind="Download.aspx.cs" Inherits="N2.Addons.AddonCatalog.UI.Download" %>
<%@ Import Namespace="N2.Web"%>
<asp:Content ID="c" ContentPlaceHolderID="Content" runat="server">
    Downloading should start any second now...
    <hr />
    <a href="<%= Url.Parse(CurrentPage.Url) %>">&laquo; Back</a>

</asp:Content>
