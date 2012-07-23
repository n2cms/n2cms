<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<div id="content">
	
	<div id="main">
		<h1 title="<%= Html.CurrentPage().Title %> #<%= Html.CurrentPage().ID %> "><%= Html.CurrentItem().Title %> #<%= Html.CurrentItem().ID %></h1>
		<table>
			<thead><tr><td colspan="2"><h2>Details</h2></td></tr><tr><td>Name</td><td>Value</td></tr></thead>
			<tbody>
		<% foreach(var d in Html.CurrentItem().Details) { %>
		<tr><td><%= d.Name %></td><td><%= d.Value %></li></td></tr>
		<% } %>
		</tbody></table>

		<table>
			<thead><tr><td colspan="3"><h2>Detail Collections</h2></td></tr><tr><td>Collection Name</td><td>Name</td><td>Value</td></tr></thead>
			<tbody>
		<% foreach(var dc in Html.CurrentItem().DetailCollections) { %>
			<tr><td title="<%= dc.Details.Count %>" rowspan="<%= dc.Details.Count %>"><%= dc.Name %></td>
		<% foreach(var d in dc.Details) { %>
			<td><%= d.Name %></td><td><%= d.Value %></li></td>
			</tr><tr>
		<% } %>
		</tr>
		<% } %>
		</tbody></table>

		<style>td { border:solid 1px #ddd; padding:1px 2px; }</style>
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
</asp:Content>