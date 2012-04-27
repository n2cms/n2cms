<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="UpdateReferences.aspx.cs" Inherits="N2.Edit.LinkTracker.UpdateReferences" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<edit:CancelLink ID="hlCancel" runat="server" meta:resourceKey="hlCancel">Cancel</edit:CancelLink>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
	<div class="tabPanel">

		<asp:Repeater runat="server" ID="rptReferencingItems">
			<ItemTemplate>
				<div><a href='<%# Eval("Url") %>'><asp:Image ImageUrl='<%# ResolveUrl(Eval("IconUrl")) %>' runat="server" /><%# Eval("Title") %></a></div>
			</ItemTemplate>
		</asp:Repeater>

		<asp:Button runat="server" Text="Update" OnCommand="OnUpdateCommand" />
	</div>
</asp:Content>