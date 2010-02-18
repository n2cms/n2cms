<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<TextItem>" %>
<%= Html.DisplayContent(m => m.Title) %>
<%= Html.DisplayContent(m => m.Text) %>