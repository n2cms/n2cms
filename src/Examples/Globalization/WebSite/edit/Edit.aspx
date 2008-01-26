<%@ Page MasterPageFile="Framed.Master" Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="N2.Edit.Edit" Title="Edit" %>
<%@ Register Src="AvailableZones.ascx" TagName="AvailableZones" TagPrefix="uc1" %>
<%@ Register Src="ItemInfo.ascx" TagName="ItemInfo" TagPrefix="uc1" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/edit.css" type="text/css" />
	<script src="Js/plugins.js?v4" type="text/javascript" ></script>
</asp:Content>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:LinkButton ID="btnSave" OnCommand="OnSaveCommand" runat="server" CssClass="command" AccessKey="s" meta:resourceKey="btnSave">save</asp:LinkButton>
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" AccessKey="c" meta:resourceKey="hlCancel">cancel</asp:HyperLink>
    <asp:HyperLink ID="hlEditParent" runat="server" CssClass="editParent command" AccessKey="p" meta:resourceKey="hlEditParent">edit parent</asp:HyperLink>
    <asp:HyperLink ID="hlZones" runat="server" CssClass="showZones command" AccessKey="z" meta:resourceKey="hlInfo" NavigateUrl="javascript:void(0);">zones</asp:HyperLink>
    <asp:HyperLink ID="hlInfo" runat="server" CssClass="showInfo command" AccessKey="i" meta:resourceKey="hlInfo" NavigateUrl="javascript:void(0);">info</asp:HyperLink>
</asp:Content>
<asp:Content ID="co" ContentPlaceHolderID="Outside" runat="server">
	<div class="right">
		<uc1:ItemInfo id="ucInfo" runat="server" />
		<uc1:AvailableZones id="ucZones" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <asp:ValidationSummary ID="vsEdit" runat="server" CssClass="validator" HeaderText="The item couldn't be saved. Please look at the following:" meta:resourceKey="vsEdit"/>
    <n2:ItemEditor ID="ie" runat="server" />
</asp:Content>