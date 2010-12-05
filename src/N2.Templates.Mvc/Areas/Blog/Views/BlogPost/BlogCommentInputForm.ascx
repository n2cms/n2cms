<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ModelViewUserControl<BlogCommentInputModel, BlogPost>" %>
<%@ Import Namespace="N2.Web"%>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Controllers" %>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Models.Pages" %>
<%@ Import Namespace="N2.Templates.Mvc.Areas.Blog.Models" %>


<script type="text/javascript" src="<%=ResolveUrl("~/N2/Resources/tiny_mce/tiny_mce.js") %>"></script>

<div id="addComment">
    <a href="javascript:showComment();">
	    <img src="<%= ResolveUrl("~/Content/Img/bullet_toggle_plus.png") %>" alt="" />
	    Post comment
    </a>
</div>
<div class="uc blog-comment-input" id="comments">
    <div class="blog-comment-title">Post Comment</div>
	<div class="box" id="commentInput">
	    <div class="inner">
		    <div class="inputForm">
		    <% 
		        using (Html.BeginForm("Submit", "BlogPost")) 
                {%>
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
				    <label for="Email" class="label">Email <span class="small-text">(hidden)</span> *</label>
				    <%=Html.TextBoxFor(m => m.Email, new { maxlength = 250, @class = "tb" })%>
				    <%=Html.ValidationMessage("Email")%>
			    </div>
			    <div class="row cf">
				    <label for="Url" class="label">Url <span class="small-text">(start with http://)</span></label>
				    <%=Html.TextBoxFor(m => m.Url, new { maxlength = 250, @class = "tb" })%>
				    <%=Html.ValidationMessage("Url")%>
			    </div>
			    <div class="row cf">
				    <label for="Text" class="label">Text *<%="<br>" + Html.ValidationMessage("Text")%></label>
				    <%=Html.TextAreaFor(m => m.Text, new { style="width: 445px;" })%>
			    </div>
			    <div class="row cf">
				    <label class="label">&nbsp;</label>
					<input type="submit" value="Submit" />
			    </div>
		    <%} %>
		    </div>
	    </div>
	</div>
</div>
<script type="text/javascript">	//<![CDATA[
    function showComment(quick) {
        if (document.forms[0].action.indexOf("#") < 0)
            document.forms[0].action += "#comments";
        jQuery("#addComment").hide();
        $c = jQuery(".blog-comment-input");
        if (quick) $c.show();
        else $c.slideDown();
    }
    $(document).ready(function () {
        tinyMCE.init({
            // General options
            mode: "textareas",
            theme: "advanced",
            plugins: "safari",

            // Theme options
            theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,outdent,indent,blockquote,",
            theme_advanced_buttons2: "bullist,numlist,|,undo,redo,|,sub,sup,|,link,unlink,image,cleanup",
            theme_advanced_buttons3: "",
            theme_advanced_toolbar_location: "top",
            theme_advanced_toolbar_align: "left",
            theme_advanced_statusbar_location: "bottom",
            theme_advanced_resizing: true

        });
    });

//]]></script>
<% if (!ViewData.ModelState.IsValid) { %>
     <script type="text/javascript">	//<![CDATA[
         jQuery(document).ready(function () {
             showComment();

             if ($('#Text').val().trim() == '') {
                 // Text cannot be blank.  wait 1sec for MCE to load then style it
                 setTimeout("errors()", 1000);
             }
         });
         function errors() {
            // style mce editor with error
            var error = $('.input-validation-error:first');
            var color = $(error).css('border-top-color');
            var width = $(error).css('border-top-width');
            var style = $(error).css('border-top-style');
            $('#Text_tbl').css('border-width', width);
            $('#Text_tbl').css('border-color', color);
            $('#Text_tbl').css('border-style', style);
         }
//]]></script>   
<% }%>