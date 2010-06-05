<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<N2.Templates.Mvc.Areas.Tests.Models.DemoViewModel>" %>
<div class="uc">
	<style>.field-validation-error { color:red; display:block; }</style>
	<h4>Sign up for the demo</h4>
	<div class="box"><div class="inner">
		<% using(Html.BeginForm("Login", null)){ %>
			<p>
				<%= Html.LabelFor(m => m.Email) %>
				<%= Html.EditorFor(m => m.Email) %>
				<%= Html.ValidationMessageFor(m => m.Email) %>
			</p>
			<%= Html.SubmitButton("Start", "Start demo") %>
		<%} %>
	</div></div>
</div>