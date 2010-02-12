<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<RandomImage>" %>
<%= Html.Display(ri => ri.RandomImageUrl) %>