<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" Theme="ContentFocus" AutoEventWireup="true" CodeBehind="Text.aspx.cs" Inherits="N2.Templates.UI.Text" Title="Untitled Page" %>
<asp:Content ID="c" ContentPlaceHolderID="Content" runat="server">
    <n2:EditableDisplay ID="dim" PropertyName="Image" runat="server" />
    <n2:Path ID="p" runat="server" />
    <n2:EditableDisplay ID="dti" PropertyName="Title" runat="server" Visible="<%$ CurrentPage: ShowTitle %>" />
    <n2:EditableDisplay ID="dte" PropertyName="Text" runat="server" />
</asp:Content>
