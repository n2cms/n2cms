<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ModelViewUserControl<CommentInputModel, CommentInput>" %>
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
				<%--<asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" 
					CssClass="label" Text="Title" meta:resourcekey="lblTitleResource1" />
				<asp:TextBox ID="txtTitle" runat="server" CssClass="tb" MaxLength="250" 
					meta:resourcekey="txtTitleResource1" />
				<asp:RequiredFieldValidator ID="rfvTitle" runat="server" 
					ValidationGroup="CommentInput" ControlToValidate="txtTitle" Text="*" 
					Display="Dynamic" meta:resourcekey="rfvTitle" />--%>
			</div>
			<div class="row cf">
				<label for="Name" class="label">Name *</label>
				<%=Html.TextBoxFor(m => m.Name, new { maxlength = 250, @class = "tb" })%>
				<%=Html.ValidationMessage("Name")%>
				<%--<asp:Label ID="lblName" runat="server" AssociatedControlID="txtName" 
					CssClass="label" Text="Name *" meta:resourcekey="lblNameResource1" />
				<asp:TextBox ID="txtName" runat="server" CssClass="tb" MaxLength="250" 
					meta:resourcekey="txtNameResource1" />
				<asp:RequiredFieldValidator ID="rfvName" runat="server" 
					ValidationGroup="CommentInput" ControlToValidate="txtName" Text="*" 
					Display="Dynamic" meta:resourcekey="rfvNameResource1" />--%>
			</div>
			<div class="row cf">
				<label for="Email" class="label">Email (hidden) *</label>
				<%=Html.TextBoxFor(m => m.Email, new { maxlength = 250, @class = "tb" })%>
				<%=Html.ValidationMessage("Email")%>
				<%--<asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" 
					CssClass="label" Text="Email (hidden) *" meta:resourcekey="lblEmailResource1" />
				<asp:TextBox ID="txtEmail" runat="server" CssClass="tb" MaxLength="250" 
					meta:resourcekey="txtEmailResource1" />
				<asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
					ValidationGroup="CommentInput" ControlToValidate="txtEmail" Text="*" 
					Display="Dynamic" meta:resourcekey="rfvEmailResource1" />
				<asp:RegularExpressionValidator ID="revEmail" runat="server" 
					ControlToValidate="txtEmail" Text="*" ValidationGroup="CommentInput" 
					ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
					Display="Dynamic" meta:resourcekey="revEmailResource1" />--%>
			</div>
			<div class="row cf">
				<label for="Url" class="label">Url</label>
				<%=Html.TextBoxFor(m => m.Url, new { maxlength = 250, @class = "tb" })%>
				<%=Html.ValidationMessage("Url")%>
				<%--<asp:Label ID="lblUrl" runat="server" AssociatedControlID="txtUrl" 
					CssClass="label" Text="Url" meta:resourcekey="lblUrlResource1" />
				<asp:TextBox ID="txtUrl" runat="server" CssClass="tb" MaxLength="250" 
					meta:resourcekey="txtUrlResource1" />
				<asp:RegularExpressionValidator ID="revUrl" runat="server" 
					ControlToValidate="txtUrl" Text="*" ValidationGroup="CommentInput" 
					ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&amp;=]*)?" 
					Display="Dynamic" meta:resourcekey="revUrlResource1" />--%>
			</div>
			<div class="row cf">
				<label for="Text" class="label">Text *</label>
				<%=Html.TextAreaFor(m => m.Text)%>
				<%=Html.ValidationMessage("Text")%>
				<%--<asp:Label runat="server" AssociatedControlID="txtText" CssClass="label" 
					Text="Text *" meta:resourcekey="LabelResource1" />
				<asp:TextBox ID="txtText" runat="server" TextMode="MultiLine" 
					meta:resourcekey="txtTextResource1" />
				<asp:RequiredFieldValidator ID="rfvText" runat="server" 
					ValidationGroup="CommentInput" ControlToValidate="txtText" Text="*" 
					Display="Dynamic" meta:resourcekey="rfvTextResource1" />--%>
			</div>
			<div class="row cf">
				<label class="label">&nbsp;</label>
				<input type="submit" />
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
//]]></script>