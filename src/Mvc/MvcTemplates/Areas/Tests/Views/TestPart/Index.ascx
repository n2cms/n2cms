<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<N2.Definitions.ItemDefinition>>" %>
<style>
    .hoverExpand .inner { display:none; }
    .hoverExpand:hover .inner { display:block; }
</style>
<div class="uc">
	<div class="box hoverExpand">
		<h4>Test Part</h4>
		<div class="inner">
			<h5>Route Values</h5>
			<div style="font-size:.7em">
			<%= Html.Partial("Dictionary", ViewContext.RouteData.Values) %>
			</div>
			<h5>Route tokens</h5>
			<div style="font-size:.7em">
			<%= Html.Partial("Dictionary", ViewContext.RouteData.DataTokens) %>
			</div>
			<h5>Urls</h5>	
			<div style="font-size:.7em">
			<%= Html.Partial("Urls") %>
			</div>
		</div>
	</div>
	<% using (Html.BeginForm("Remove", null)){ %>
		<input type="submit" value="Remove this" style="font-size:.7em" />
	<% } %>
</div>
<table>
    <tr>
        <th>current</th><td><%= Html.CurrentItem() %></td>
    </tr>
    <tr>
        <th>editable item</th><td><%= Html.CurrentItem()["EditableItem"] %></td>
    </tr>
    <tr>
        <th>editable children</th>
        <td>
            <% foreach(var child in (IEnumerable)(Html.CurrentItem()["EditableChildren"] ?? new N2.ContentItem[0])) { %>
            <%= child %><br />
            <% } %>
        </td>
    </tr>
</table>