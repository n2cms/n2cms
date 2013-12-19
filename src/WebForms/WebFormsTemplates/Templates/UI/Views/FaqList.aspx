<%@ Page Language="C#" MasterPageFile="../Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="FaqList.aspx.cs" Inherits="N2.Templates.UI.Views.FaqList" Title="Untitled Page" %>
<asp:Content ID="cpc" ContentPlaceHolderID="PostContent" runat="server">
    <div class="list">
        <n2:DroppableZone runat="server" ZoneName="Questions" />
    </div>
</asp:Content>
