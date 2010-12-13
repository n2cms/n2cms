<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="N2.Edit.Delete" Title="Delete" %>
<%@ Register Src="AffectedItems.ascx" TagName="AffectedItems" TagPrefix="uc1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="ContentToolbar" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:LinkButton ID="btnDelete" runat="server" OnClick="OnDeleteClick" CssClass="command" meta:resourceKey="btnDelete">delete</asp:LinkButton>
    <asp:HyperLink runat="server" ID="hlReferencingItems" CssClass="plain command" Text="List Referencing Items" meta:resourceKey="hlReferencingItems" />
    <edit:CancelLink ID="hlCancel" runat="server" meta:resourceKey="hlCancel">cancel</edit:CancelLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
    <asp:CustomValidator ID="cvDelete" runat="server" CssClass="validator info" meta:resourceKey="cvDelete" Display="Dynamic" />
    <asp:CustomValidator ID="cvException" runat="server" CssClass="validator info" Display="Dynamic" />
	<fieldset id="referencingItems" runat="server" style="padding:8px; margin-bottom:10px">
		<legend><asp:CheckBox ID="chkAllow" Checked="true" AutoPostBack="true" OnCheckedChanged="chkAllow_OnCheckedChanged" runat="server" Text="Delete and break references" meta:resourceKey="chkAllow" /></legend>
		<asp:Repeater ID="rptReferencing" runat="server">
			<ItemTemplate><div><edit:ContentLink runat="server" DataSource='<%# Container.DataItem %>' /></div></ItemTemplate>
		</asp:Repeater>
	</fieldset>
	<edit:FieldSet class="affectedItems" runat="server" Legend="Affected items" meta:resourceKey="affectedItems">
		<uc1:AffectedItems id="itemsToDelete" runat="server" />
	</edit:FieldSet>
</asp:Content>
