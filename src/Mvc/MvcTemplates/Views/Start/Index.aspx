<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true" %>
<asp:Content ContentPlaceHolderID="PostContent" runat="server">

[<%= User.Identity.Name %>]
[<%= User.IsInRole("Administrators") %>]
[<%= User.IsInRole("Users") %>]
[<%= User.IsInRole("Everyone") %>]
</asp:Content>