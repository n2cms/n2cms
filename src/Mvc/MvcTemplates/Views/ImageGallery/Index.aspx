<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true" 
	Inherits="N2.Web.Mvc.ContentViewPage<ImageGalleryModel, ImageGallery>" Title="" %>
<%@ Import Namespace="N2.Collections"%>
<%@ Import Namespace="N2.Web.Drawing" %>
<asp:Content ContentPlaceHolderID="Head" runat="server">
	<script src="<%= ResolveClientUrl("~/Content/Galleria/galleria-1.2.9.min.js") %>" type="text/javascript"></script>
	<link rel="Stylesheet" type="text/css" href="<%= ResolveClientUrl("~/Content/Galleria/themes/classic/galleria.classic.css") %>" />
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentAndSidebar" runat="server">

	<% var fs = Html.ResolveService<N2.Edit.FileSystem.IFileSystem>(); %>
	<%=ContentHtml.DisplayContent(m => m.Text)%>
	<div id="galleria" class="bgDark" style="height:542px">
		<%foreach(var item in Model.GalleryItems){ %>
			<a id="t<%= item.ID %>" href="<%= ResolveUrl(ImagesUtility.GetExistingImagePath(fs, item.ImageUrl, "wide")) %>" class="thumbnail">
				<img alt="<%= item.Title %>" src="<%= ResolveUrl(ImagesUtility.GetExistingImagePath(fs, item.ImageUrl, "thumb")) %>" 
					data-big="<%= ResolveUrl(ImagesUtility.GetExistingImagePath(fs, item.ImageUrl, "original")) %>" 
					data-title="<%= HttpUtility.HtmlAttributeEncode(item.Title) %>" 
					data-description="<%= HttpUtility.HtmlAttributeEncode(item.Text) %>"/>
			</a>
		<%}%>
	</div>

	<script type="text/javascript">
		Galleria.loadTheme('<%= ResolveClientUrl("~/Content/Galleria/themes/classic/galleria.classic.js") %>');

		$(document).ready(function () {

			Galleria.run('#galleria', {
				thumb_crop: true, // crop all thumbnails to fit
//				data_config: function(img) {
//					// will extract and return image captions from the source:
//					var $texts = $(img).parent().next('.text');
//					return {
//						title: $texts.children("h2").html(),
//						description: $texts.children("div").html()
//					};
//				},
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
