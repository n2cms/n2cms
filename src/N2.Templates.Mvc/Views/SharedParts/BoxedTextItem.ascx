<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<BoxedTextItem>" %>

<h4><%=Model.Title%></h4>
<%--<n2:H4 Text="<%$ CurrentItem: Title %>" Visible="<%$ HasValue: Title %>" runat="server" />--%>
<div class="box"><div class="inner">
    <%= Html.Display(m => m.Text) %>
</div></div>