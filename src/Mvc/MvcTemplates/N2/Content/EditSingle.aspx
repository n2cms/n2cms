<%@ Page Title="" Language="C#" MasterPageFile="~/N2/Content/Framed.master" AutoEventWireup="true" CodeBehind="EditSingle.aspx.cs" Inherits="N2.Management.Content.EditSingle" meta:resourcekey="PageResource1" %>
<%@ Import namespace="N2"%>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="<%= MapCssUrl("edit.css") %>" type="text/css" />
</asp:Content>

<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
	<asp:LinkButton ID="btnSave" OnCommand="OnPublishCommand" runat="server" CssClass="command iconed publish" meta:resourceKey="btnSave">Save</asp:LinkButton>
	<edit:CancelLink ID="hlCancel" runat="server" Text="Cancel" meta:resourceKey="hlCancel" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
	<edit:PermissionPanel id="ppPermitted" RequiredPermission="Write" runat="server" meta:resourceKey="ppPermitted">
		<asp:ValidationSummary ID="vsEdit" runat="server" CssClass="validator info" HeaderText="The item couldn't be saved. Please look at the following:" meta:resourceKey="vsEdit"/>
		<asp:CustomValidator ID="cvException" runat="server" Display="None" />

		<n2:ItemEditor ID="ie" runat="server" />
		

    <script type="text/javascript">
    	$(document).ready(function () {
    		
    		$(".helpPanel").click(function () {
    			var $hp = $(this);
    			$hp.toggleClass("helpVisible");
    		});

    		// hide mce toolbar to prevent it getting skewed
    		$(".tabs a").click(function () {
    			$(".mceExternalToolbar").hide();
    		});
    		$("input").focus(function () {
    			$(".mceExternalToolbar").hide();
    		});

    		$(".dimmable").n2dimmable();

    		$(".uploader > label").n2revealer();

    		$(".expandable").n2expandable({ visible: ".uncontractable" });

    		$("form").n2expandableBox({ opener: ".rightOpener", opened: "#outside" });
    		$("#outside .box").n2expandableBox({ opener: "h4", opened: ".box-inner" });
    	});
    </script>
	</edit:PermissionPanel>
</asp:Content>
