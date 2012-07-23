﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace="N2.Web.Rendering" %>
<%
	DisplayableToken token = Html.DisplayableToken();
	string[] components = token.GetComponents();
	string[] options = (components.Length > 1 ? components[1] : components[0]).Split(',');
	string name = token.GetOptionalInputName(0, 1);
%>

<span class="formfield">
<% foreach(var opt in options) { %>
<% var id = @Html.UniqueID(null); %>
<span class="formcheckbox">
<input type="checkbox" name="<%= name %>" value="<%= opt %>" id="<%= id %>" />
<label for="<%= id %>"><%= opt %></label>
</span>
<% } %>
</span>