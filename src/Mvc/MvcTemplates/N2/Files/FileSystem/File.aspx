<%@ Page Language="C#" MasterPageFile="../../Content/Framed.Master" AutoEventWireup="true" CodeBehind="File.aspx.cs" Inherits="N2.Edit.FileSystem.File1" %>

<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<asp:LinkButton ID="btnDownload" runat="server" Text="Download" CssClass="command iconed download" OnCommand="OnDownloadCommand" meta:resourceKey="btnDownload" />
    <n2:OptionsMenu id="omSizes" runat="server"/>
    <asp:HyperLink ID="hlCrop" NavigateUrl="Crop.aspx" CssClass="command crop iconed" runat="server" meta:resourceKey="hlCrop">Crop</asp:HyperLink>
	<asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="command" OnCommand="OnSaveCommand" Visible="false" meta:resourceKey="btnSave" />
	<asp:LinkButton ID="btnEdit" runat="server" Text="Edit" CssClass="command" OnCommand="OnEditCommand" Visible="false" meta:resourceKey="btnEdit" />
	<asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CssClass="command cancel" OnCommand="OnCancelCommand" Visible="false" meta:resourceKey="btnCancel" />
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" runat="server">
	<h1><% foreach (N2.ContentItem node in ancestors){ %>/<a href="<%= GetPreviewUrl(node) %>"><%= node.Title %></a><% } %></h1>

	<div class="tabPanel">
	<a href="<%= SelectedItem.Url %>">
		<img src="<%= N2.Web.Url.ToAbsolute(Selection.SelectedItem.IconUrl) %>" alt="icon" />
		<%= SelectedItem.Title %>
		(<%= GetFileSize(SelectedFile.Size) %>)
	</a>

	<div class="fileContents">
		<asp:TextBox ID="txtContent" runat="server" CssClass="fileContents" Visible="false" TextMode="MultiLine" />
		<edit:ResizedImage MaxHeight="200" MaxWidth="300" ImageUrl="<%# SelectedFile.LocalUrl %>" runat="server" Hash="<%# SelectedFile.Updated.ToString() %>" />
	</div>
	</div>
</asp:Content>
