<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Edit.Edit" Title="Edit" meta:resourcekey="PageResource1" %>
<%@ Import namespace="N2"%>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>
<%@ Register Src="AvailableZones.ascx" TagName="AvailableZones" TagPrefix="uc1" %>
<%@ Register Src="ItemInfo.ascx" TagName="ItemInfo" TagPrefix="uc1" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/edit.css" type="text/css" />
</asp:Content>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <div class="rightAligned">
		<asp:PlaceHolder runat="server" ID="phPluginArea" />
		<asp:HyperLink ID="hlZones" runat="server" CssClass="showZones command" meta:resourceKey="hlZones" NavigateUrl="javascript:void(0);">Zones</asp:HyperLink>
		<asp:HyperLink ID="hlInfo" runat="server" CssClass="showInfo command" meta:resourceKey="hlInfo" NavigateUrl="javascript:void(0);">Info</asp:HyperLink>
    </div>
    <n2:OptionsMenu id="om" runat="server">
		<asp:LinkButton ID="btnSave" OnCommand="OnSaveCommand" runat="server" CssClass="command" meta:resourceKey="btnSave">Save and publish</asp:LinkButton>
		<asp:LinkButton ID="btnSaveUnpublished" OnCommand="OnSaveUnpublishedCommand" runat="server" CssClass="command plain" meta:resourceKey="btnSaveUnpublished">Save an unpublished version</asp:LinkButton>
		<asp:LinkButton ID="btnPreview" OnCommand="OnPreviewCommand" runat="server" CssClass="command plain" meta:resourceKey="btnPreview">Preview changes</asp:LinkButton>
    </n2:OptionsMenu>
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" meta:resourceKey="hlCancel">Cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="co" ContentPlaceHolderID="Outside" runat="server">
	<div class="right">
		<uc1:ItemInfo id="ucInfo" runat="server" />
		<uc1:AvailableZones id="ucZones" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<asp:HyperLink ID="hlNewerVersion" runat="server" Text="There is a newer unpublished version of this page." CssClass="versionInfo info" Visible="False" meta:resourcekey="hlNewerVersionResource1"/>
	<asp:HyperLink ID="hlOlderVersion" runat="server" Text="This is a version of another item." CssClass="versionInfo info" Visible="False" meta:resourcekey="hlOlderVersionResource1"/>
    <asp:ValidationSummary ID="vsEdit" runat="server" CssClass="validator info" HeaderText="The item couldn't be saved. Please look at the following:" meta:resourceKey="vsEdit"/>
    <asp:CustomValidator ID="cvException" runat="server" Display="None" />

    <n2:ItemEditor ID="ie" runat="server" />
        
    <script type="text/javascript">
		$(document).ready(function(){
			$(".helpPanel").click(function(){
				var $hp = $(this);
				$hp.toggleClass("helpVisible");
			});
    		toolbarSelect('<%= CreatingNew ? "new" : "edit" %>');

			$(".right fieldset").hide();

			$(".showInfo").toggle(function() {
				n2toggle.show(this, ".infoBox");
			}, function() {
				n2toggle.hide(this, ".infoBox");
			});

			$(".showZones").toggle(function() {
				n2toggle.show(this, ".zonesBox");
			}, function() {
				n2toggle.hide(this, ".zonesBox");
			});

			if (cookie.read(".infoBox"))
				$(".showInfo").click();
			if (cookie.read(".zonesBox"))
				$(".showZones").click();
		});
    </script>
</asp:Content>