<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div id="content">
	
	<div id="main">
		<h1 title="<%= Html.CurrentPage().Title %> #<%= Html.CurrentPage().ID %> "><%= Html.CurrentItem().Title %> #<%= Html.CurrentItem().ID %></h1>
		<p>Type: <%= Html.CurrentItem().GetType().Name %> (<%= Html.CurrentItem().GetContentType().Name %>)</p>
		
		<table>
			<thead><tr><td colspan="2"><h2>Details</h2></td></tr><tr><td>Name</td><td>Value</td></tr></thead>
			<tbody>
		<% foreach(var d in Html.CurrentItem().Details) { %>
		<tr><td><%= d.Name %></td><td><%= d.Value %></td></tr>
		<% } %>
		</tbody></table>

		<table>
			<thead><tr><td colspan="3"><h2>Detail Collections</h2></td></tr><tr><td>Collection Name</td><td>Name</td><td>Value</td></tr></thead>
			<tbody>
		<% foreach(var dc in Html.CurrentItem().DetailCollections) { %>
			<tr><td title="<%= dc.Details.Count %>" rowspan="<%= dc.Details.Count %>"><%= dc.Name %></td>
		<% foreach(var d in dc.Details) { %>
			<td><%= d.Name %></td><td><%= d.Value %></td>
			</tr><tr>
		<% } %>
		</tr>
		<% } %>
		</tbody></table>

	<table>
		<thead><tr><td colspan="4"><h2>Properties metadata</h2></td></tr><tr><td>Property</td><td>Editable</td><td>Displayable</td><td>Attributes</td></tr></thead>
			<tbody>
		<% foreach(var p in N2.Context.Current.Definitions.GetDefinition(Html.CurrentItem()).Properties.OrderBy(p => p.Key)) { %>
		<tr><td rowspan="<%=p.Value.Attributes.Length %>" title="<%= p.Value.PropertyType.Name %>"><%= p.Key %><%= p.Value.Info == null ? "*" : "" %></td>
		<td rowspan="<%=p.Value.Attributes.Length %>"><%= p.Value.Editable != null ? p.Value.Editable.GetType().Name : "" %></td>
		<td rowspan="<%=p.Value.Attributes.Length %>"><%= p.Value.Displayable != null ? p.Value.Displayable.GetType().Name : "" %></td>
		<% foreach(var a in p.Value.Attributes) { %>
			<td title="<%= a.GetHashCode() %>"><%= a.GetType().Name %></td>
			</tr><tr>
		<% } %>
		</tr>
		<% } %>
	</tbody></table>
	
	<table>
		<thead><tr><td colspan="3"><h2>Editables metadata</h2></td></tr><tr><td>Name</td><td>Attribute</td><td>HashCode</td></tr></thead>
			<tbody>
		<% foreach(var e in N2.Context.Current.Definitions.GetDefinition(Html.CurrentItem()).Editables) { %>
			<tr>
			<td><%= e.Name %></td>
			<td><%= e.GetType().Name %></td>
			<td><%= e.GetHashCode() %></td>
			</tr>
		<% } %>
	</tbody></table>
	
	<table>
		<thead><tr><td colspan="4"><h2>Displayables metadata</h2></td></tr><tr><td>Name</td><td>Attribute</td><td>HashCode</td><td>Display</td></tr></thead>
			<tbody>
		<% foreach(var d in N2.Context.Current.Definitions.GetDefinition(Html.CurrentItem()).Displayables) { %>
			<tr>
			<td><%= d.Name %></td>
			<td><%= d.GetType().Name %></td>
			<td><%= d.GetHashCode() %></td>
			<td><%= Html.DisplayContent(d.Name) %></td>
			</tr>
		<% } %>
	</tbody></table>

		<style>#main td { border:solid 1px #ddd; padding:1px 2px; font-size:.7em; max-width:140px; overflow:hidden; } #main thead { background-color:#eee; } #main table { width:100%; }</style>
	</div>
	
	<div id="extras">
	<fieldset><legend>Route values</legend>
		<%= Html.Partial("Dictionary", ViewContext.RouteData.Values) %>
	</fieldset>
		
	<fieldset><legend>Route tokens</legend>
		<%= Html.Partial("Dictionary", ViewContext.RouteData.DataTokens) %>
	</fieldset>
    
	<fieldset><legend>Urls</legend>
		<%= Html.Partial("Urls") %>		
	</fieldset>
		
	<fieldset><legend>TestParts</legend>
		<% using (Html.BeginForm("Add", null)){ %>
			<input name="name" />
			<input type="submit" value="Add" />
		<% } %>
		<%= Html.DroppableZone("TestParts") %>
	</fieldset>
	</div>
	</div>

	<button id="refresh">refresh</button>
	<script>
		$("#refresh").click(function () {
			
		});

	</script>
</asp:Content>
