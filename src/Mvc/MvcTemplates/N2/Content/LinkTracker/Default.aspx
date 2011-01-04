<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.LinkTracker._Default" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<edit:CancelLink ID="hlCancel" runat="server" meta:resourceKey="hlCancel">Cancel</edit:CancelLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
    <h1>Inbound & outbound links</h1>
    <table><tbody><tr><td>
	<edit:FieldSet class="referencingItems" runat="server" Legend="Incoming links" meta:resourceKey="referencingItems">
		<div style="margin:10px">
		<asp:Repeater runat="server" ID="rptReferencingItems">
			<ItemTemplate>
				<div><a href='<%# Eval("Url") %>'><asp:Image ImageUrl='<%# ResolveUrl(Eval("IconUrl")) %>' runat="server" /><%# Eval("Title") %></a></div>
			</ItemTemplate>
		</asp:Repeater>
		<div style="margin:10px">
	</edit:FieldSet>
	</td><td style="padding:10px;">
		<img src="../../Resources/Icons/link_go.png" alt="right" />
	</td><td style="padding:10px;">
		<a href='<%# Selection.SelectedItem.Url %>'><asp:Image ID="Image1" ImageUrl='<%# ResolveUrl(Selection.SelectedItem.IconUrl) %>' runat="server" /><%# Selection.SelectedItem.Title %></a>
	</td><td style="padding:10px;">
		<img src="../../Resources/Icons/link_go.png" alt="right" />
	</td><td>
	<edit:FieldSet class="referencedItems" runat="server" Legend="Outgoing links" meta:resourceKey="referencedItems">
		<div style="margin:10px">
		<asp:Repeater runat="server" ID="rptReferencedItems">
			<ItemTemplate>
				<div><a href='<%# Eval("Url") %>'><asp:Image ImageUrl='<%# ResolveUrl(Eval("IconUrl")) %>' runat="server" /><%# Eval("Title") %></a></div>
			</ItemTemplate>
		</asp:Repeater>	
		</div>
	</edit:FieldSet>
	</td></tr></tbody></table>
</asp:Content>