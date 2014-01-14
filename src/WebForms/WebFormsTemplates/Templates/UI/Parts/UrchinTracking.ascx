<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UrchinTracking.ascx.cs" Inherits="N2.Templates.UI.Parts.UrchinTracking" %>
<asp:PlaceHolder Visible="<%# Track %>" runat="server">
<script type="text/javascript">
	var _gaq = _gaq || [];
	_gaq.push(['_setAccount', '<%# CurrentItem.UACCT %>']);
	_gaq.push(['_setCustomVar', 1, "page", <%# CurrentPage.ID %>]);
	_gaq.push(['_setCustomVar', 1, "published", "<%# CurrentPage.Published.HasValue ? CurrentPage.Published.Value.ToString("yyyy-MM") : "unpublished" %>"]);
	_gaq.push(['_setCustomVar', 1, "authenticated", "<%# this.Page.User.Identity.IsAuthenticated ? true : false %>"]);
	_gaq.push(['_trackPageview']);
	(function () {
		var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
		ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
		var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);

		$(document).ready(function(){
			$("a").not("[href^=#]").not("[href^=mailto]")
			.hover(function(){
				_gaq.push(['_trackEvent', 'Link', 'Hover', this.innerText]);
			}).click(function(e){
				var href = this.href.toString();
				if (href.match(/[.](pdf|doc|txt|docx|rtm)$/)) {
					_gaq.push(['_trackEvent', 'Link', 'Document', this.innerText]);
				} else if(href.indexOf(location.protocol + "//" + location.host) != 0) {
					_gaq.push(['_trackEvent', 'Link', 'External', this.innerText]);
				} else
					return;

				e.preventDefault();
				setTimeout(function(){
					window.location = href;
				}, 10);
			});
		});
	})();
</script>
</asp:PlaceHolder>