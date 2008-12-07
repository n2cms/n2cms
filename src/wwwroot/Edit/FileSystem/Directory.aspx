<%@ Page Language="C#" MasterPageFile="~/Edit/Framed.Master" AutoEventWireup="true" CodeBehind="Directory.aspx.cs" Inherits="N2.Edit.FileSystem.Directory1" Title="Untitled Page" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>
<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
	<asp:HyperLink ID="hlNewFile" runat="server" Text="Upload file" CssClass="command" />
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" runat="server">	
	<edit:InfoLabel ID="lblDirectories" Label="Directories: " runat="server" />
	<edit:InfoLabel ID="lblFiles" Label="Files: " runat="server" />
</asp:Content>
