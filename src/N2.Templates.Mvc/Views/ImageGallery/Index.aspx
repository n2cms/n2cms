<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true" 
	Inherits="N2.Web.Mvc.ContentViewPage<ImageGalleryModel, ImageGallery>" Title="" %>
<%@ Import Namespace="N2.Collections"%>
<asp:Content ContentPlaceHolderID="Head" runat="server">
	<script src="<%= ResolveClientUrl("~/Content/Galleria/jquery.galleria.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentAndSidebar" runat="server">

	<%=ContentHtml.DisplayContent(m => m.Text)%>

	<div id="galleria" class="bgDark">
		<%foreach(var item in Model.GalleryItems){ %>
			<a id="t<%= item.ID %>" href="<%= ResolveUrl(item.GetResizedImageUrl(Html.ResolveService<N2.Edit.FileSystem.IFileSystem>())) %>" class="thumbnail">
				<img alt="<%= item.Title %>" src="<%= ResolveUrl(item.GetThumbnailImageUrl(Html.ResolveService<N2.Edit.FileSystem.IFileSystem>())) %>" />
			</a>
			<div class="text">
				<h2><%=item.Title%></h2>
				<div><%=item.Text%></div>
			</div>
		<%}%>
	</div>
	<div id="preview">
	</div>

	<script type="text/javascript">
		Galleria.loadTheme('<%= ResolveClientUrl("~/Content/Galleria/themes/classic/galleria.classic.js") %>');

		$(document).ready(function () {
			// run galleria and add some options
			$('#galleria').galleria({
				image_crop: true, // crop all images to fit
				thumb_crop: true, // crop all thumbnails to fit
				transition: 'fade', // crossfade photos
				transition_speed: 250, // slow down the crossfade
				data_config: function(img) {
					// will extract and return image captions from the source:
					var $texts = $(img).parent().next('.text');
					return {
						title: $texts.children("h2").html(),
						description: $texts.children("div").html()
					};
				},
				extend: function() {
					this.bind(Galleria.IMAGE, function(e) {
						// bind a click event to the active image
						$(e.imageTarget).css('cursor', 'pointer').click(this.proxy(function() {
							// open the image in a lightbox
							this.openLightbox();
						}));
					});
				}
			});
		});
	</script>
</asp:Content>