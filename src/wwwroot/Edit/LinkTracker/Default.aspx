<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.LinkTracker._Default" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>
<asp:Content ID="ContentHead" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/linktracker.css" type="text/css" />
    <script src="../Js/plugins.ashx" type="text/javascript" ></script>
    <script type="text/javascript">
        $(document).ready(function(){
			toolbarSelect("linktracker");
		});
	</script>
</asp:Content>
<asp:Content ID="ContentContent" ContentPlaceHolderID="Content" runat="server">
    <h1>Inbound & outbound links</h1>
	<edit:FieldSet class="referencingItems" runat="server" Legend="Other items referencing this item" meta:resourceKey="referencingItems">
		<asp:Repeater runat="server" ID="rptReferencingItems">
			<ItemTemplate>
				<div><a href='<%# Eval("Url") %>'><img src='<%# Eval("IconUrl") %>' /><%# Eval("Title") %></a></div>
			</ItemTemplate>
		</asp:Repeater>
	</edit:FieldSet>
	<edit:FieldSet class="referencedItems" runat="server" Legend="Items referenced by this item" meta:resourceKey="referencedItems">
		<asp:Repeater runat="server" ID="rptReferencedItems">
			<ItemTemplate>
				<div><a href='<%# Eval("Url") %>'><img src='<%# Eval("IconUrl") %>' /><%# Eval("Title") %></a></div>
			</ItemTemplate>
		</asp:Repeater>	
	</edit:FieldSet>
</asp:Content>