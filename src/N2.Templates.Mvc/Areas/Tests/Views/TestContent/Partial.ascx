<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div style="border:solid 2px green; margin:5px; padding:5px;">
	<h2 title="<%= Html.CurrentPage().Title %>"><%= Html.CurrentItem().Title %></h2>
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