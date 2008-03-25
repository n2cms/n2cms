<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubMenu.ascx.cs" Inherits="N2.Templates.UI.Layouts.Parts.SubMenu" %>
<n2:H4 ID="hsm" runat="server" />
<n2:Box ID="bsm" runat="server">
	<n2:Menu ID="sm" BranchMode="false" runat="server" StartLevel='2' CssClass="subMenu menu" />
</n2:Box>
