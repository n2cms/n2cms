<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.master" AutoEventWireup="true" 
	Inherits="N2.Web.Mvc.ContentViewPage<ImageGalleryModel, ImageGallery>" Title="" %>
<%@ Import Namespace="N2.Collections"%>

<asp:Content ContentPlaceHolderID="ContentAndSidebar" runat="server">
	<% Html.RenderAction<NavigationController>(c => c.Breadcrumb()); %>
	
	<%=ContentHtml.DisplayContent(m => m.Title)%>
	<%=ContentHtml.DisplayContent(m => m.Text)%>
	
	<div id="thumbnails">
		<%foreach(var item in Model.GalleryItems){ %>
			<a id="t<%=item.ID%>" href="<%=ResolveUrl(item.ResizedImageUrl) %>" class="thumbnail">
				<img alt="<%=item.Title%>" src="<%=ResolveUrl(item.ThumbnailImageUrl)%>" />
			</a>
			<div class="text">
				<h2><%=item.Title%></h2>
				<%=item.Text%>
			</div>
		<%}%>
	</div>
	<div id="preview">
	</div>

	<script type="text/javascript">
		$(document).ready(function(){
			var show = function(){
				$(".selected", "#thumbnails").removeClass("selected");
				$("#preview").html($(this).addClass("selected").next().children().clone()).prepend("<img src='" + this.href + "'/>");
				
				return false;
			};
			$(".thumbnail").click(show).filter(location.hash || "*").slice(0,1).each(show);
		});
	</script>
</asp:Content>