<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<N2.Definitions.ItemDefinition>>" %>
<div class="uc">
	<div class="box">
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
