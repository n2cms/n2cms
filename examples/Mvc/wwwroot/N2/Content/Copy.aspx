<%@ Page MasterPageFile="Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Copy.aspx.cs" Inherits="N2.Edit.Copy" Title="Copy" %>
<%@ Register Src="AffectedItems.ascx" TagName="AffectedItems" TagPrefix="uc1" %>

<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:LinkButton ID="btnCopy" meta:resourceKey="btnCopy" runat="server" OnClick="OnCopyClick" CssClass="command"><img src='../Resources/Icons/page_copy.png' /> try again</asp:LinkButton>
    <asp:HyperLink ID="btnCancel" meta:resourceKey="btnCancel" runat="server" CssClass="command cancel">cancel</asp:HyperLink>
</asp:Content>

<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <asp:CustomValidator id="cvCopy" meta:resourceKey="cvCopy" runat="server" CssClass="validator info" />
    <asp:CustomValidator ID="cvException" runat="server" CssClass="validator info" Display="Dynamic" />

	<div class="cf">
		<asp:Panel ID="pnlNewName" runat="server" CssClass="formField" Visible="false">
			<asp:Label ID="lblNewName" runat="server" AssociatedControlID="txtNewName" meta:resourceKey="lblNewName" Text="New name" CssClass="label" />
			<asp:TextBox ID="txtNewName" runat="server" />
		</asp:Panel>
		<div class="formField">
			<asp:Label ID="lblFrom" runat="server" AssociatedControlID="from" meta:resourceKey="lblFrom" Text="From" CssClass="label" />
			<asp:Label ID="from" runat="server"/>
		</div>
		<div class="formField">
			<asp:Label ID="lblTo" runat="server" AssociatedControlID="to" meta:resourceKey="lblTo" Text="To" CssClass="label" />        
			<asp:Label ID="to" runat="server"/>
		</div>
	</div>
    <hr />
    <h3 id="h3" runat="server" meta:resourceKey="h3">Copied items:</h3>
    <uc1:AffectedItems id="itemsToCopy" runat="server" />
</asp:Content>