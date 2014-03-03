<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.ContentViewUserControl<CommentInputModel, CommentInput>" %>
<%@ Import Namespace="N2.Web"%>
<a href="javascript:showComment();" id="addComment">
	<img src="<%= ResolveUrl("~/Content/Img/bullet_toggle_plus.png") %>" alt="" />
	Add comment
</a>
<div class="uc commentInput">
	<div class="box" id="commentInput">
	<div class="inner">
		<div class="inputForm">
		<%using (Html.BeginForm("Submit", "CommentInput")){%>
			<div class="row cf">
				<label for="Email" class="label">Title *</label>
				<%=Html.TextBoxFor(m => m.Title, new { maxlength = 250, @class = "tb" })%>
				<%=Html.ValidationMessage("Title")%>
			</div>
			<div class="row cf">
				<label for="Name" class="label">Name *</label>
				<%=Html.TextBoxFor(m => m.Name, new { maxlength = 250, @class = "tb" })%>
				<%=Html.ValidationMessage("Name")%>
			</div>
			<div class="row cf">
				<label for="Email" class="label">Email (hidden) *</label>
				<%=Html.TextBoxFor(m => m.Email, new { maxlength = 250, @class = "tb" })%>
				<%=Html.ValidationMessage("Email")%>
			</div>
			<div class="row cf">
				<label for="Url" class="label">Url</label>
				<%=Html.TextBoxFor(m => m.Url, new { maxlength = 250, @class = "tb" })%>
				<%=Html.ValidationMessage("Url")%>
			</div>
			<div class="row cf">
				<label for="Text" class="label">Text *</label>
				<%=Html.TextAreaFor(m => m.Text)%>
				<%=Html.ValidationMessage("Text")%>
			</div>
			<div class="row cf">
				<label for="Trap" class="label">Clear this</label>
				<%=Html.TextBoxFor(m => m.Trap, new { @class = "trap" })%>
			</div>
			<div class="row cf">
				<label class="label">&nbsp;</label>
				<input type="submit" />
				<%=Html.ValidationMessage("Trap")%>
			</div>
		<%} %>
		</div>
	</div>
	</div>
</div>
<script type="text/javascript">	//<![CDATA[
	function showComment(quick) {
		if (document.forms[0].action.indexOf("#") < 0) 
			document.forms[0].action += "#commentInput";
		jQuery("#addComment").hide();
		$c = jQuery("#commentInput");
		if (quick) $c.show();
		else $c.slideDown();
	}
	if (location.hash == "#commentInput")
		showComment(true);

	jQuery(function () {
		$(".trap").val("").closest("div").hide();
	});
//]]></script>