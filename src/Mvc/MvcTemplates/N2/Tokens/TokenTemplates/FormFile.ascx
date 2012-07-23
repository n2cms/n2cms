<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<string>" %>
<%@ Import Namespace="N2.Web.Rendering" %>
<%
	string name = Model ?? Html.DisplayableToken().GenerateInputName();
%>
<span class="formfield formfile">
<input name="<%= name %>" type="file" />
</span>