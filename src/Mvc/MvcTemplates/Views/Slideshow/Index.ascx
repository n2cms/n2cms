<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<N2.Web.Slideshow>" %>
<%@ Import Namespace="N2.Collections"%>
<%@ Import Namespace="N2.Web.Drawing" %>

<% var uniqueID = Html.UniqueID(); %>

<%--TODO: Write this into the <HEAD> somehow.--%>

<script src="<%= ResolveClientUrl("~/Content/Galleria/galleria-1.2.9.min.js") %>" type="text/javascript"></script>
<link rel="Stylesheet" type="text/css" href="<%= ResolveClientUrl("~/Content/Galleria/themes/classic/galleria.classic.css") %>" />

<div id="<%= uniqueID %>" style="height: 400px;">
<% foreach (var x in Model.GetSlideshowImages()) { %>
	<img src="<%= x.ImageHref %>" alt="<%= x.Title %>" data-title="<%= x.Title %>" />
<% } %>
</div>


<script type="text/javascript">
	Galleria.loadTheme('<%= ResolveClientUrl("~/Content/Galleria/themes/classic/galleria.classic.js") %>');

	$(document).ready(function () {

	    Galleria.run('#<%= uniqueID %>', {
	        thumb_crop: true, // crop all thumbnails to fit
	        //				data_config: function(img) {
	        //					// will extract and return image captions from the source:
	        //					var $texts = $(img).parent().next('.text');
	        //					return {
	        //						title: $texts.children("h2").html(),
	        //						description: $texts.children("div").html()
	        //					};
	        //				},
	        extend: function () {
	            this.bind(Galleria.IMAGE, function (e) {
	                // bind a click event to the active image
	                $(e.imageTarget).css('cursor', 'pointer').click(this.proxy(function () {
	                    // open the image in a lightbox
	                    this.openLightbox();
	                }));
	            });
	        }
	    });
	});
</script>