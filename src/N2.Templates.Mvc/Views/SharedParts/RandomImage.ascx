<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<RandomImage>" %>
<%= Html.DisplayContent(ri => ri.RandomImageUrl)%>