<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="N2.Edit.Delete" Title="Delete" %>
<%@ Register Src="AffectedItems.ascx" TagName="AffectedItems" TagPrefix="uc1" %>
<%@ Register Src="ReferencingItems.ascx" TagName="ReferencingItems" TagPrefix="uc1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>
<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/delete.css" type="text/css" />
</asp:Content>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:LinkButton ID="btnDelete" runat="server" OnClick="OnDeleteClick" CssClass="command" meta:resourceKey="btnDelete">delete</asp:LinkButton>
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" meta:resourceKey="hlCancel">cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
    <asp:CustomValidator ID="cvDelete" runat="server" CssClass="validator info" meta:resourceKey="cvDelete" Display="Dynamic" />
    <table><tr><td>
		<edit:FieldSet class="affectedItems" runat="server" Legend="Affected items" meta:resourceKey="affectedItems">
			<uc1:AffectedItems id="itemsToDelete" runat="server" />
		</edit:FieldSet>
    </td><td>
		<edit:FieldSet class="referencingItems" runat="server" Legend="Items referencing the items you're deleting" meta:resourceKey="referencingItems">
			<uc1:ReferencingItems id="referencingItems" runat="server" />
		</edit:FieldSet>
    </td></tr></table>
</asp:Content>