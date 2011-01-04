<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkFromParameter.ascx.cs" Inherits="N2.Addons.Wiki.UI.WikiTemplates.LinkFromParameter" %>
<a class="<%= CurrentPage.GetChild(CurrentArguments) == CurrentPage ? "new" : string.Empty %>" href="<%= CurrentPage.AppendUrl(CurrentArguments) %>"><%= CurrentArguments%></a>
