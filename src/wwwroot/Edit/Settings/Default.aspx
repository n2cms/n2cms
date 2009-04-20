<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Settings.Default" meta:resourceKey="settingsPage" %>
<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
		<asp:LinkButton ID="btnSave" OnClick="btnSave_Click" runat="server" CssClass="command" meta:resourceKey="btnSave">Save</asp:LinkButton>
	<asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" meta:resourceKey="hlCancel">Cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <asp:PlaceHolder ID="phSettings" runat="server" />
</asp:Content>