<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<N2.Definitions.ItemDefinition>>" %>
<div class="uc">
	<h4 title="<%= ViewData["RemainingItems"] %>">Content Creator</h4>
	<div class="box"><div class="inner">
	<h5 title="<%= Html.CurrentPage().Title %>/<%= Html.CurrentItem().Title %>">Random</h5>
	<% using (Html.BeginForm("Random", null)){ %>
		<%= Html.DropDownList("discriminator", Model.Select(d => new SelectListItem { Value = d.Discriminator, Text = d.Title }))%>
		<input name="name" value="Random" title="name" />
		<input name="amount" value="100" title="amount" />
		<div><label><input name="relate" type="checkbox" />incredibly relate</label></div>
		<div><label><input name="background" type="checkbox" />run in background</label></div>
		<div><label><input name="images" type="checkbox" />add images</label></div>
		<input type="submit" value="Create" />
	<% } %>
	<h5 title="<%= Html.CurrentPage().Title %>/<%= Html.CurrentItem().Title %>">Content Creator</h5>
	<% using (Html.BeginForm("Create", null)){ %>
		<%= Html.DropDownList("discriminator", Model.Select(d => new SelectListItem { Value = d.Discriminator, Text = d.Title }))%>
		<input name="name" value="Page" title="name" />
		<input name="width" value="10" title="width" />
		<input name="depth" value="3" title="depth" />
		<div><label><input name="background" type="checkbox" />run in background</label></div>
		<input type="submit" value="Create" />
	<% } %>
	<h5>Versions</h5>
	<% using (Html.BeginForm("AddVersions", null)){ %>
		<input type="submit" value="Add version (recursive)" />
	<% } %>
	</div></div>
</div>
