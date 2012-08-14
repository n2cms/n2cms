<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<N2.Definitions.ItemDefinition>>" %>
<div class="uc">
	<h4>Content Creator</h4>
	<div class="box"><div class="inner">
	<h5 title="<%= Html.CurrentPage().Title %>/<%= Html.CurrentItem().Title %>">Random</h5>
	<% using (Html.BeginForm("Random", null)){ %>
		<%= Html.DropDownList("discriminator", Model.Select(d => new SelectListItem { Value = d.Discriminator, Text = d.Title }))%>
		<input name="name" value="Random" title="name" />
		<input name="amount" value="100" title="amount" />
		<input type="submit" value="Create" />
	<% } %>
	<h5 title="<%= Html.CurrentPage().Title %>/<%= Html.CurrentItem().Title %>">Content Creator</h5>
	<% using (Html.BeginForm("Create", null)){ %>
		<%= Html.DropDownList("discriminator", Model.Select(d => new SelectListItem { Value = d.Discriminator, Text = d.Title }))%>
		<input name="name" value="Page" title="name" />
		<input name="width" value="10" title="width" />
		<input name="depth" value="3" title="depth" />
		<input type="submit" value="Create" />
	<% } %>
	</div></div>
</div>
