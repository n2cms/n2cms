<%@ Page MasterPageFile="Layouts/Default.Master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.Calendar.UI.Default" %>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <n2:Path ID="p1" runat="server" />
    <n2:EditableDisplay PropertyName="Title" runat="server" />
    <span class="date"><%= CurrentItem.EventDate %></span>
    <n2:EditableDisplay PropertyName="Introduction" runat="server" />
    <n2:EditableDisplay PropertyName="Text" runat="server" />
</asp:Content>