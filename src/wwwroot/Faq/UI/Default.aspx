<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.Faq.UI.Default" Title="Untitled Page" %>
<asp:Content ID="cpc" ContentPlaceHolderID="PostContent" runat="server">
    <div class="list">
        <n2:DroppableZone runat="server" ZoneName="Questions" />
    </div>
</asp:Content>
