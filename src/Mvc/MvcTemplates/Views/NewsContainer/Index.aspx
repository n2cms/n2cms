<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.master" Inherits="System.Web.Mvc.ViewPage<NewsContainerModel>" %>

<asp:Content ID="mc" ContentPlaceHolderID="PostContent" runat="server">
    <div class="list">
		<% Html.RenderPartial("Range", Model); %>
    </div>
    <% if (!Model.IsLast)
	   { %>
	<script type="text/javascript">
		jQuery(document).ready(function() {

			var loading = false;

			var loadNext = function(e) {
				var a = this;
				loading = true;
				$(a).text("...");
				jQuery.get(a.href, { fragment: true }, function(html) {
					$(a).replaceWith(html);
					loading = false;
				});
				if (e && e.preventDefault) e.preventDefault();
			};

			$(".scroller").click(loadNext).each(function(i) {
				$(window).scroll(function() {
					if (loading) return;
					$(".scroller").filter(function() {
						var h = Math.min($(this).position().top, $(document).height());
						return $(window).scrollTop() >= h - $(window).height();
					}).each(loadNext);
				}).scroll();
			});
		});
	</script>
    <% } %>
</asp:Content>
