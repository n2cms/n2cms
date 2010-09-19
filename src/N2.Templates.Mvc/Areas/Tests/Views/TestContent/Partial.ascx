<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div class="uc">
	<h4 title="<%= Html.CurrentPage().Title %>/<%= Html.CurrentItem().Title %>">Content Creator</h4>
	<div class="box"><div class="inner">
	<% using (Html.BeginForm("Create", null)){ %>
		<input name="name" value="Page" title="name" />
		<input name="width" value="10" title="width" />
		<input name="depth" value="3" title="depth" />
		<input type="submit" value="Create" />
	<% } %>
	</div></div>
</div>
<div class="uc">
	<div class="box">
		<h4>Route Values</h4>
		<div class="inner" style="font-size:.7em">
			<%= Html.Partial("Dictionary", ViewContext.RouteData.Values) %>
		</div>
		<h4>Route tokens</h4>
		<div class="inner" style="font-size:.7em">
			<%= Html.Partial("Dictionary", ViewContext.RouteData.DataTokens) %>
		</div>
		
		<h4>Urls</h4>	
		<div class="inner" style="font-size:.7em">
			<%= Html.Partial("Urls") %>
		</div>
	</div>
	<% using (Html.BeginForm("Remove", null)){ %>
		<input type="submit" value="Remove this" style="font-size:.7em" />
	<% } %>
</div>