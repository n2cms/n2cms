<%@ Page Language="C#" MasterPageFile="~/Edit/Framed.Master" AutoEventWireup="true" CodeBehind="File.aspx.cs" Inherits="N2.Edit.FileSystem.File1" Title="Untitled Page" %>
<asp:Content ContentPlaceHolderID="Toolbar" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="Content" runat="server">
    <a href="<%= SelectedItem.Url %>">
        <img src="<%= N2.Web.Url.ToAbsolute(SelectedItem.IconUrl) %>" alt="icon" />
        <%= SelectedItem.Title %>
        (<%= SelectedFile.Size / 1024 %> kB)
    </a>
</asp:Content>
