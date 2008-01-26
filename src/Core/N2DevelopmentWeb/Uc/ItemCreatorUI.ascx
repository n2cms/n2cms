<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemCreatorUI.ascx.cs" Inherits="N2DevelopmentWeb.Uc.ItemCreatorUI" %>
<strong><%= CurrentItem.Title %></strong>
prefix: <asp:TextBox ID="txtPrefix" runat="server" >item</asp:TextBox><br />
count: <asp:TextBox ID="txtCount" runat="server" >3</asp:TextBox><br />
depth: <asp:TextBox ID="txtDepth" runat="server" >3</asp:TextBox><br />
<asp:Button ID="btnCreate" runat="server" Text="create" OnClick="btnCreate_Click" />