<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.LinkTracker._Default" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<edit:CancelLink ID="hlCancel" runat="server" meta:resourceKey="hlCancel">Cancel</edit:CancelLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
    <h1>Inbound & outbound links</h1>
    <table><tbody><tr><td>
	<edit:FieldSet class="referencingItems" runat="server" Legend="Incoming links" meta:resourceKey="referencingItems">
		<asp:Repeater runat="server" ID="rptReferencingItems">
			<ItemTemplate>
				<div><a href='<%# Eval("Url") %>'><asp:Image ImageUrl='<%# Eval("IconUrl") %>' runat="server" /><%# Eval("Title") %></a></div>
			</ItemTemplate>
		</asp:Repeater>
	</edit:FieldSet>
	</td><td>
	<edit:FieldSet class="referencedItems" runat="server" Legend="Outgoing links" meta:resourceKey="referencedItems">
		<asp:Repeater runat="server" ID="rptReferencedItems">
			<ItemTemplate>
				<div><a href='<%# Eval("Url") %>'><asp:Image ImageUrl='<%# Eval("IconUrl") %>' runat="server" /><%# Eval("Title") %></a></div>
			</ItemTemplate>
		</asp:Repeater>	
	</edit:FieldSet>
	</td></tr></tbody></table>
</asp:Content>