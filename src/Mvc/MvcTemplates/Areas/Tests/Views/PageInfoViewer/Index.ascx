<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<style>
    .hoverExpand .inner { display:none; }
    .hoverExpand:hover .inner { display:block; }
	.debugTable {
		font-size:.75em;
		border-top:solid 1px silver;
		width:100%;
	}
	.debugTable th,
	.debugTable td {
		border:solid 1px silver;
		padding:0 1px;
		vertical-align:top;
	}
</style>

<table class="debugTable">
	<tr><th>SortOrder</th><td colspan="2"><%= Html.CurrentPage().SortOrder %></td></tr>
	<tr><th>VersionIndex</th><td colspan="2"><%= Html.CurrentPage().VersionIndex %></td></tr>
	<tr><td colspan="3"></td></tr>
	
<% foreach(var d in Html.CurrentPage().Details) { %>
	<tr><th><%= d.Name %></th><td title="<%= Html.AttributeEncode(d.StringValue) %>" colspan="2"><%= d.ValueType %></td></tr>
<% } %>
	<tr><td colspan="2"></td></tr>

<% foreach(var dc in Html.CurrentPage().DetailCollections) { %>
	<tr><th title="<%= dc.Details.Count %>" rowspan="<%= dc.Details.Count %>"><%= dc.Name %></th>
<% foreach(var d in dc.Details) { %>
	<th><%= d.Name %></th><td title="<%= Html.AttributeEncode(d.StringValue) %>"><%= d.ValueTypeKey %></td>
	</tr><tr>
<% } %>
	</tr>
<% } %>
</table>
