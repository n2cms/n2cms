<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.Faq.UI.Default" Title="Untitled Page" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <n2:Path ID="p1" runat="server" />
    <n2:EditableDisplay PropertyName="Title" runat="server" />
    <n2:EditableDisplay PropertyName="Text" runat="server" />
    <div class="list">
        <n2:DroppableZone runat="server" ZoneName="Questions" />
    </div>
</asp:Content>
