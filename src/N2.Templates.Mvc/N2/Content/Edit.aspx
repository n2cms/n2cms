<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Edit.Edit" Title="Edit" meta:resourcekey="PageResource1" EnableEventValidation="false" %>
<%@ Import namespace="N2"%>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Register Src="AvailableZones.ascx" TagName="AvailableZones" TagPrefix="uc1" %>
<%@ Register Src="ItemInfo.ascx" TagName="ItemInfo" TagPrefix="uc1" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="<%= MapCssUrl("edit.css") %>" type="text/css" />
</asp:Content>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <div class="rightAligned">
		<asp:PlaceHolder runat="server" ID="phPluginArea" />
		<asp:HyperLink ID="hlZones" runat="server" CssClass="showZones command" meta:resourceKey="hlZones" NavigateUrl="javascript:void(0);">Zones</asp:HyperLink>
		<asp:HyperLink ID="hlInfo" runat="server" CssClass="showInfo command" meta:resourceKey="hlInfo" NavigateUrl="javascript:void(0);">Info</asp:HyperLink>
    </div>
    <n2:OptionsMenu id="om" runat="server">
		<asp:LinkButton ID="btnSavePublish" OnCommand="OnPublishCommand" runat="server" CssClass="command iconed publish" meta:resourceKey="btnSave">Save and publish</asp:LinkButton>
		<asp:LinkButton ID="btnPreview" OnCommand="OnPreviewCommand" runat="server" CssClass="command plain iconed preview" meta:resourceKey="btnPreview">Save and preview</asp:LinkButton>
		<asp:LinkButton ID="btnSaveUnpublished" OnCommand="OnSaveUnpublishedCommand" runat="server" CssClass="command plain iconed save" meta:resourceKey="btnSaveUnpublished">Save an unpublished version</asp:LinkButton>
        <asp:HyperLink ID="hlFuturePublish" NavigateUrl="#futurePanel" CssClass="command plain iconed future" runat="server" meta:resourceKey="hlSavePublishInFuture">Save and publish version in future</asp:HyperLink>
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

    <div id="futurePanel" class="popup">
        <n2:DatePicker Label-Text="When" ID="dpFuturePublishDate" runat="server" meta:resourceKey="dpFuturePublishDate" />
        <asp:Button ID="btnSavePublishInFuture" Text="OK" OnCommand="OnSaveFuturePublishCommand" CssClass="ok" runat="server" meta:resourceKey="btnSavePublishInFuture" />
        <asp:HyperLink ID="hlCancelSavePublishInFuture" NavigateUrl="javascript:void(0);" runat="server" CssClass="cancel" meta:resourceKey="hlCancelSavePublishInFuture">Cancel</asp:HyperLink>
    </div>

    <n2:ItemEditor ID="ie" runat="server" />
        
    <script type="text/javascript">
    	$(document).ready(function() {
    		// future publish
    		$("#futurePanel").hide().click(function(e) { e.stopPropagation(); });
    		$(".future").click(function(e) {
    			$("#futurePanel").css({ left: e.clientX + "px", top: e.clientY + "px" }).show();
    			$("#futurePanel input:first").focus();
    			e.preventDefault();
    			e.stopPropagation();
    		});

    		$("#futurePanel .cancel").click(function() {
    			$("#futurePanel").hide();
    		});
    		$(document.body).click(function(e) {
    			if ($(e.target).closest(".jCalendar").length == 0)
    				$("#futurePanel").hide();
    		});

    		$(".helpPanel").click(function() {
    			var $hp = $(this);
    			$hp.toggleClass("helpVisible");
    		});

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

    		if ($.cookie(".infoBox"))
    			$(".showInfo").click();
    		if ($.cookie(".zonesBox"))
    			$(".showZones").click();

    		// hide mce toolbar to prevent it getting skewed
    		$(".tabs a").click(function() {
    			$(".mceExternalToolbar").hide();
    		});
    		$("input").focus(function() {
    			$(".mceExternalToolbar").hide();
    		});

    		$(".dimmable").n2dimmable();

    		$(".uploader > label").each(function() {
    			$("<a href='#' class='revealer'/>").html(this.innerHTML)
    			.insertBefore(this)
    			.click(function() {
    				$(this).hide()
    				.siblings().show()
    				.end().closest(".editDetail").addClass("crowded");
    			}).siblings().hide();
    		});

    		$(".expandable").n2expandable({ visible: ".uncontractable" });
    	});

    </script>
</asp:Content>
