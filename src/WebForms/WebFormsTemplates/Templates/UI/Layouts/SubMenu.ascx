<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubMenu.ascx.cs" Inherits="N2.Templates.UI.Layouts.SubMenu" %>
<div class="uc">
	<n2:Hn Level="4" ID="hsm" runat="server" HtmlEncode="false" />
	<n2:Box ID="bsm" runat="server">
		<n2:Menu ID="m" BranchMode="true" runat="server" CssClass="subMenu menu" SkinID="subMenu" StartLevel="2" />
	</n2:Box>
</div>