<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentInput.ascx.cs" Inherits="N2.Templates.UI.Parts.CommentInput" %>
<%@ Import Namespace="N2.Web"%>
<a href="javascript:showComment();" id="addComment"><img src="<%= Url.ToAbsolute("~/Templates/UI/Img/bullet_toggle_plus.png") %>" alt="" /> <%= GetLocalResourceObject("addComment") %></a>
<n2:Box ID="commentInput" runat="server" CssClass="box" meta:resourcekey="BoxResource1">
    <div class="inputForm">
        <div class="row cf">
            <asp:Label ID="lblTitle" runat="server" AssociatedControlID="txtTitle" 
                CssClass="label" Text="Title" meta:resourcekey="lblTitleResource1" />
            <asp:TextBox ID="txtTitle" runat="server" CssClass="tb" MaxLength="250" 
                meta:resourcekey="txtTitleResource1" />
        </div>
        <div class="row cf">
            <asp:Label ID="lblName" runat="server" AssociatedControlID="txtName" 
                CssClass="label" Text="Name *" meta:resourcekey="lblNameResource1" />
            <asp:TextBox ID="txtName" runat="server" CssClass="tb" MaxLength="250" 
                meta:resourcekey="txtNameResource1" />
            <asp:RequiredFieldValidator ID="rfvName" runat="server" 
                ValidationGroup="CommentInput" ControlToValidate="txtName" Text="*" 
                Display="Dynamic" meta:resourcekey="rfvNameResource1" />
        </div>
        <div class="row cf">
            <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" 
                CssClass="label" Text="Email (hidden) *" meta:resourcekey="lblEmailResource1" />
            <asp:TextBox ID="txtEmail" runat="server" CssClass="tb" MaxLength="250" 
                meta:resourcekey="txtEmailResource1" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                ValidationGroup="CommentInput" ControlToValidate="txtEmail" Text="*" 
                Display="Dynamic" meta:resourcekey="rfvEmailResource1" />
            <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                ControlToValidate="txtEmail" Text="*" ValidationGroup="CommentInput" 
                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                Display="Dynamic" meta:resourcekey="revEmailResource1" />
        </div>
        <div class="row cf">
            <asp:Label ID="lblUrl" runat="server" AssociatedControlID="txtUrl" 
                CssClass="label" Text="Url" meta:resourcekey="lblUrlResource1" />
            <asp:TextBox ID="txtUrl" runat="server" CssClass="tb" MaxLength="250" 
                meta:resourcekey="txtUrlResource1" />
            <asp:RegularExpressionValidator ID="revUrl" runat="server" 
                ControlToValidate="txtUrl" Text="*" ValidationGroup="CommentInput" 
                ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&amp;=]*)?" 
                Display="Dynamic" meta:resourcekey="revUrlResource1" />
        </div>
        <div class="row cf">
            <asp:Label runat="server" AssociatedControlID="txtText" CssClass="label" 
                Text="Text *" meta:resourcekey="LabelResource1" />
            <asp:TextBox ID="txtText" runat="server" TextMode="MultiLine" 
                meta:resourcekey="txtTextResource1" />
            <asp:RequiredFieldValidator ID="rfvText" runat="server" 
                ValidationGroup="CommentInput" ControlToValidate="txtText" Text="*" 
                Display="Dynamic" meta:resourcekey="rfvTextResource1" />
        </div>
        <div class="row cf">
            <label class="label">&nbsp;</label>
            <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" 
                Text="Submit" meta:resourcekey="btnSubmitResource1" />
        </div>
    </div>
</n2:Box>
<script type="text/javascript">
    function showComment(quick) {
        if (document.forms[0].action.indexOf("#") < 0) 
            document.forms[0].action += "#commentInput";
        jQuery("#addComment").hide();
        $c = jQuery("#<%= commentInput.ClientID %>");
        if (quick) $c.show();
        else $c.slideDown();
    }
    if (location.hash == "#commentInput")
        showComment(true);
</script>
