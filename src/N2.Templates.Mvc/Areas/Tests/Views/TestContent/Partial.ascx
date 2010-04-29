<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div style="border:solid 1px green;padding:2px;">
	<h2 title="<%= Html.CurrentPage().Title %>"><%= Html.CurrentItem().Title %></h2>

	<fieldset><legend>Content Creator</legend>
	<% using (Html.BeginForm("Create", null)){ %>
		<input name="name" value="Page" title="name" />
		<input name="width" value="10" title="width" />
		<input name="depth" value="3" title="depth" />
		<input type="submit" value="Create" />
	<% } %>
	</fieldset>
	
	<div style="height:200px; overflow:auto">
	<fieldset><legend>Route values</legend>
		<%= Html.Partial("Dictionary", ViewContext.RouteData.Values) %>
	</fieldset>
	
	<fieldset><legend>Route tokens</legend>
		<%= Html.Partial("Dictionary", ViewContext.RouteData.DataTokens) %>
	</fieldset>
		
	<fieldset><legend>Urls</legend>
		<%= Html.Partial("Urls") %>		
	</fieldset>

	<% using (Html.BeginForm("Remove", null)){ %>
		<input type="submit" value="Remove" />
	<% } %>
	</div>
</div>