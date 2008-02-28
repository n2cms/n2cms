<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Edit.Edit" Title="Edit" meta:resourcekey="PageResource1" %>
<%@ Import namespace="N2"%>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>
<%@ Register Src="AvailableZones.ascx" TagName="AvailableZones" TagPrefix="uc1" %>
<%@ Register Src="ItemInfo.ascx" TagName="ItemInfo" TagPrefix="uc1" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/edit.css" type="text/css" />
	<script src="Js/plugins.ashx" type="text/javascript" ></script>
</asp:Content>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <div class="rightAligned">
		<edit:OptionsMenu id="om2" runat="server">
			<asp:HyperLink ID="hlEditParent" runat="server" CssClass="editParent command" AccessKey="p" meta:resourceKey="hlEditParent">Edit parent</asp:HyperLink>
			<n2:ItemDataSource id="idsChildren" runat="server" PageFilter="true" ZoneName="" />
			<asp:Repeater runat="server" DataSourceID="idsChildren">
				<ItemTemplate>
					<asp:HyperLink CssClass="command plain" 
						NavigateUrl="<%# Engine.EditManager.GetEditExistingItemUrl((ContentItem)Container.DataItem) %>" 
						runat="server"><%# Eval("Title") %></asp:HyperLink>
				</ItemTemplate>
			</asp:Repeater>
		</edit:OptionsMenu>
		<asp:HyperLink ID="hlZones" runat="server" CssClass="showZones command" AccessKey="z" meta:resourceKey="hlZones" NavigateUrl="javascript:void(0);">Zones</asp:HyperLink>
		<asp:HyperLink ID="hlInfo" runat="server" CssClass="showInfo command" AccessKey="i" meta:resourceKey="hlInfo" NavigateUrl="javascript:void(0);">Info</asp:HyperLink>
    </div>
    <edit:OptionsMenu id="om" runat="server">
		<asp:LinkButton ID="btnSave" OnCommand="OnSaveCommand" runat="server" CssClass="command" AccessKey="s" meta:resourceKey="btnSave">Save and publish</asp:LinkButton>
		<asp:LinkButton ID="btnSaveUnpublished" OnCommand="OnSaveUnpublishedCommand" runat="server" CssClass="command" AccessKey="p" meta:resourceKey="btnSaveUnpublished">Save an unpublished version</asp:LinkButton>
    </edit:OptionsMenu>
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" AccessKey="c" meta:resourceKey="hlCancel">Cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="co" ContentPlaceHolderID="Outside" runat="server">
	<div class="right">
		<uc1:ItemInfo id="ucInfo" runat="server" />
		<uc1:AvailableZones id="ucZones" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<asp:HyperLink ID="hlNewerVersion" runat="server" 
		Text="There is a newer unpublished version of this page, click here to edit it." 
		CssClass="versionInfo info" Visible="False" 
		meta:resourcekey="hlNewerVersionResource1"/>
	<asp:HyperLink ID="hlOlderVersion" runat="server" 
		Text="This is a version of another item, click here to edit the currently published version." 
		CssClass="versionInfo info" Visible="False" 
		meta:resourcekey="hlOlderVersionResource1"/>
    <asp:ValidationSummary ID="vsEdit" runat="server" CssClass="validator info" HeaderText="The item couldn't be saved. Please look at the following:" meta:resourceKey="vsEdit"/>
    
    <n2:ItemEditor ID="ie" runat="server" />
        
    <script type="text/javascript">
		$(document).ready(function(){
			$(".helpPanel").click(function(){
				var $hp = $(this);
				$hp.toggleClass("helpVisible");
			});
		});
    </script>
</asp:Content>