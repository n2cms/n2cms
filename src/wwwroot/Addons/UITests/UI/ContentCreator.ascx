<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContentCreator.ascx.cs" Inherits="N2.Addons.UITests.UI.ContentCreator" %>
<n2:Box runat="server">
<table><tr><td>
Width: <asp:TextBox runat="server" ID="txtWidth" Text="3" Width="50" />
</td><td>
Depth: <asp:TextBox runat="server" ID="txtDepth" Text="2" Width="50" />
</td></tr></table>
Type: <asp:DropDownList ID="ddlType" runat="server" DataTextField="Title" DataValueField="Discriminator" Width="150" /><br />
Name prefix: <asp:TextBox runat="server" ID="txtName" Text="Page" Width="150" /><br />
<asp:Button Text="Create" runat="server" OnCommand="OnCreateCommand" />
</n2:Box>