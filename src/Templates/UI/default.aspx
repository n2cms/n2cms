<%@ Page MasterPageFile="~/Layouts/Top+SubMenu.Master" Theme="ContentFocus" Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="N2.Templates.UI.Default" %>
<asp:Content ID="c" ContentPlaceHolderID="Content" runat="server">
<n2:EditableDisplay PropertyName="Image" runat="server" />
    <n2:EditableDisplay PropertyName="Title" runat="server" Visible="<%$ CurrentPage: ShowTitle %>" />
    <n2:EditableDisplay PropertyName="Text" runat="server" />
</asp:Content>
