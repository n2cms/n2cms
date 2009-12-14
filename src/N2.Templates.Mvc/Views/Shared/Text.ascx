<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<TextItem>" %>
<%= Html.Display(m => m.Title) %>
<%= Html.Display(m => m.Text) %>