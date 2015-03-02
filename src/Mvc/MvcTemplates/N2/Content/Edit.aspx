<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Edit.Edit" Title="Edit" meta:resourcekey="PageResource1" EnableEventValidation="false" %>
<%@ Import namespace="N2"%>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Register Src="AvailableZones.ascx" TagName="AvailableZones" TagPrefix="uc1" %>
<%@ Register Src="ItemInfo.ascx" TagName="ItemInfo" TagPrefix="uc1" %>
<%@ Register Src="Versions/RecentVersions.ascx" TagName="RecentVersions" TagPrefix="uc1" %>
<%@ Register Src="Activity/RecentActivity.ascx" TagName="RecentActivity" TagPrefix="uc1" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
	<edit:ButtonGroup CssClass="btn-primary" runat="server">
		<asp:LinkButton ID="btnSavePublish" data-icon-class="fa fa-play-circle" 
			OnCommand="OnPublishCommand" runat="server" 
			CssClass="command iconed publish" 
			meta:resourceKey="btnSave">Save and publish</asp:LinkButton>

		<asp:LinkButton ID="btnPreview" data-icon-class="fa fa-eye" OnCommand="OnPreviewCommand" runat="server" CssClass="command plain iconed preview"
			meta:resourceKey="btnPreview">Save and preview</asp:LinkButton>
		<asp:LinkButton ID="btnSaveUnpublished" data-icon-class="fa fa-save" OnCommand="OnSaveUnpublishedCommand" runat="server" CssClass="command plain iconed save" meta:resourceKey="btnSaveUnpublished">Save an unpublished version</asp:LinkButton>
		<asp:HyperLink ID="hlFuturePublish" data-icon-class="fa fa-clock-o" NavigateUrl="#futurePanel" CssClass="command plain iconed future hidden-action" runat="server" meta:resourceKey="hlSavePublishInFuture">Save and publish version in future</asp:HyperLink>
		<asp:LinkButton ID="btnUnpublish" data-icon-class="fa fa-stop" OnCommand="OnUnpublishCommand" runat="server" CssClass="command plain iconed unpublish hidden-action" meta:resourceKey="btnUnpublish">Unpublish</asp:LinkButton>
	</edit:ButtonGroup>
		<edit:CancelLink ID="hlCancel" runat="server" CssClass="btn" meta:resourceKey="hlCancel">Close</edit:CancelLink>
	<ul class="nav pull-right">
		<li>
			<asp:PlaceHolder runat="server" ID="phPluginArea" />
		</li>
	</ul>

</asp:Content>
<asp:Content ID="co" ContentPlaceHolderID="Outside" runat="server">
	<uc1:ItemInfo id="ucInfo" runat="server" />
	<uc1:RecentVersions id="ucVersions" runat="server" />
	<uc1:RecentActivity id="ucActivity" runat="server" />
	<asp:PlaceHolder runat="server" ID="phSidebar" />
	<uc1:AvailableZones id="ucZones" runat="server" />
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<a href="javascript:void(0);" class="rightOpener"><span class="rightOpener-open">&laquo;</span><span class="rightOpener-close">&raquo;</span></a>

<%--  	
<table>
	<tr><th>Selected</th><td><%= Selection.SelectedItem %> (<%= Selection.SelectedItem.State %>)</td><th>VersionOf</th><td><%= Selection.SelectedItem.VersionOf.ID %></td><th>Parent</th><td><%= Selection.SelectedItem.Parent %></td></tr>
	<tr><th>Edited</th><td><%= ie.CurrentItem %> (<%= ie.CurrentItem.State %>)</td><th>VersionOf</th><td><%= ie.CurrentItem.VersionOf.ID %></td><th>Parent</th><td><%= ie.CurrentItem.Parent %></td></tr>
	<tr><th colspan="5"><hr /></th></tr>
	<% foreach(var key in Request.QueryString.AllKeys) { %>
	<tr><th colspan="2"><%= key %></th><td colspan="3"><%= Request[key] %></td></tr>
	<%} %>
</table>
--%>
	<edit:PermissionPanel id="ppPermitted" RequiredPermission="Write" runat="server" meta:resourceKey="ppPermitted">
	<asp:HyperLink ID="hlNewerVersion" runat="server" Text="There is a newer unpublished version of this page." CssClass="alert alert-margin" Visible="False" meta:resourcekey="hlNewerVersionResource1"/>
	<asp:HyperLink ID="hlOlderVersion" runat="server" Text="This is a version of another item." CssClass="alert alert-info alert-margin" Visible="False" meta:resourcekey="hlOlderVersionResource1"/>
		<asp:ValidationSummary ID="vsEdit" runat="server" CssClass="alert alert-block alert-margin" HeaderText="The item couldn't be saved. Please look at the following:" meta:resourceKey="vsEdit"/>
		<asp:CustomValidator ID="cvException" runat="server" Display="None" />

		<div id="futurePanel" class="modal" tabindex="-1" role="dialog">
			<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
				<h3 meta:resourceKey="h3futureHeading">Schedule publishing</h3>
			</div>
			<div class="modal-body" style="min-height: 100px;">
				<n2:DatePicker Label-Text="When should this content be published" ID="dpFuturePublishDate" runat="server" meta:resourceKey="dpFuturePublishDate" />
			</div>
			<div class="modal-footer">
				<asp:Button ID="btnSavePublishInFuture" Text="OK" OnCommand="OnSaveFuturePublishCommand" CssClass="btn btn-primary" runat="server" meta:resourceKey="btnSavePublishInFuture" />
				<asp:HyperLink ID="hlCancelSavePublishInFuture" NavigateUrl="javascript:void(0);" runat="server" CssClass="btn" meta:resourceKey="hlCancelSavePublishInFuture">Close</asp:HyperLink>
			</div>
		</div>
		<div class="future-panel-backdrop modal-backdrop fade in" style="display:none"></div>

		<n2:ItemEditor ID="ie" runat="server" />
	</edit:PermissionPanel>

		<script type="text/javascript">
			$(document).ready(function () {
				// future publish
				$("#futurePanel").hide().click(function (e) {
					e.stopPropagation();
				});
				$(".future").click(function (e) {
					$("#futurePanel").show().addClass("modal");
					$("#futurePanel input:first").focus();
					$("#future-panel-backdrop").show();
					e.stopPropagation();
					e.preventDefault();
					$(this).closest(".open").removeClass("open");
				});

				$("#futurePanel .cancel").click(function () {
					$("#futurePanel").hide();
				});
				$(document.body).click(function (e) {
					if ($(e.target).closest(".datepicker,.day,.week,.month,.year").length == 0)
						$("#futurePanel").hide();
				});

			// zones
				$(".showZones").toggle(function () {
					n2toggle.show(this, ".zonesBox");
				}, function () {
					n2toggle.hide(this, ".zonesBox");
				});

			// info
				if ($.cookie(".infoBox"))
					$(".showInfo").click();
				if ($.cookie(".zonesBox"))
					$(".showZones").click();

				$(".help-tooltip").tooltip({ });
				$(".help-popover").each(function () {
					var title = $(this).attr("title");
					var content = $(this).attr("data-content");
					$(this).attr("title", "");
					$(this).tooltip({ html: true, title: "<h6>" + title + "</h6><p>" + content + "</p>" });
				});
			});

		</script>
</asp:Content>
