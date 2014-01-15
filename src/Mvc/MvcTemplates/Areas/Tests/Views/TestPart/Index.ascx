<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<N2.Definitions.ItemDefinition>>" %>
<style>
    .hoverExpand .inner { display:none; }
    .hoverExpand:hover .inner { display:block; }
	.debugTable {
		font-size:.75em;
		border-top:solid 1px silver;

	}
	.debugTable th,
	.debugTable td {
		border:solid 1px silver;
		padding:0 1px;
		vertical-align:top;
	}
</style>
<table class="debugTable">
	<tr><th colspan="3" style="background-color:#eee">
		<div class="box hoverExpand">
		<h4>
			Test Part (<%= Html.CurrentItem().GetType().Name %>)
			<% using (Html.BeginForm("Remove", null)){ %><input type="submit" value="Remove this" style="font-size:.7em" /><% } %>
		</h4>
		<div class="inner">
			<h5>Route Values</h5>
			<%= Html.Partial("Dictionary", ViewContext.RouteData.Values) %>
			<h5>Route tokens</h5>
			<%= Html.Partial("Dictionary", ViewContext.RouteData.DataTokens) %>
			<h5>Urls</h5>	
			<%= Html.Partial("Urls") %>
		</div>
	</div>
</th></tr>


	<tr><th>SortOrder</th><td colspan="2"><%= Html.CurrentItem().SortOrder %></td></tr>
	<tr><th>VersionIndex</th><td colspan="2"><%= Html.CurrentItem().VersionIndex %></td></tr>
	<tr><td colspan="3"></td></tr>

<% foreach(var d in Html.CurrentItem().Details) { %>
	<tr><th><%= d.Name %></th><td colspan="2"><%= d.Value %></td></tr>
<% } %>
	<tr><td colspan="2"></td></tr>

<% foreach(var dc in Html.CurrentItem().DetailCollections) { %>
	<tr><th title="<%= dc.Details.Count %>" rowspan="<%= dc.Details.Count %>"><%= dc.Name %></th>
<% foreach(var d in dc.Details) { %>
	<th><%= d.Name %></th><td><%= d.Value %></td>
	</tr><tr>
<% } %>
	</tr>
<% } %>
	<tr><td colspan="3"></td></tr>

	<tr><th>EditableItem</th><td colspan="2"><%= Html.CurrentItem()["EditableItem"] %></td></tr>
	<tr><th>EditableLink</th><td colspan="2"><%= Html.CurrentItem()["EditableLink"] %></td></tr>
	<tr><th>EditableChildren</th><td colspan="2">
        <% foreach(var child in (IEnumerable)(Html.CurrentItem()["EditableChildren"] ?? new N2.ContentItem[0])) { %>
        <%= child %><br />
        <% } %>
    </td></tr>
	<tr><th>EditableItemSelection</th><td colspan="2">
        <% foreach(var selected in (IEnumerable)(Html.CurrentItem()["EditableItemSelection"] ?? new N2.ContentItem[0])) { %>
        <%= selected %><br />
        <% } %>
	</td></tr>
	<tr><th>EditableCheckBoxList</th><td colspan="2">
        <% foreach (var selected in (IEnumerable)(Html.CurrentItem()["EditableCheckBoxList"] ?? new string[0])){ %>
        <%= selected %><br />
        <% } %>
	</td></tr>
</table>
