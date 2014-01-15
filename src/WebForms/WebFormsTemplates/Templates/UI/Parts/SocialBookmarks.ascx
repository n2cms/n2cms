<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SocialBookmarks.ascx.cs" Inherits="N2.Templates.UI.Parts.SocialBookmarks" %>

<div style="min-height:30px;">
	<%= CurrentItem.Title %>
	
	<% if(CurrentItem.Facebook) { %>
	<div id="fb-root"></div>
	<script>
		(function (d, s, id) {
			var js, fjs = d.getElementsByTagName(s)[0];
			if (d.getElementById(id)) return;
			js = d.createElement(s); js.id = id;
			js.src = "//connect.facebook.net/en_US/all.js#xfbml=1";
			fjs.parentNode.insertBefore(js, fjs);
		} (document, 'script', 'facebook-jssdk'));

		$(document).ready(function () {
			function push(type, targetUrl) {
				if (typeof _gaq !== "undefined" && _gaq.push)
					_gaq.push(['_trackSocial', 'facebook', type, targetUrl]);
			};
			function subscribe() {
				if (typeof FB !== 'undefined' && FB.Event && FB.Event.subscribe) {
					FB.Event.subscribe('edge.create', function (targetUrl) {
						push('like', targetUrl);
					});

					FB.Event.subscribe('edge.remove', function (targetUrl) {
						push('unlike', targetUrl);
					});

					FB.Event.subscribe('message.send', function (targetUrl) {
						push('send', targetUrl);
					});
				}
			};

			if (typeof _gaq === "undefined" || typeof FB == 'undefined')
				setTimeout(subscribe, 2500);
			else
				subscribe();
		});
	</script>
	<div class="fb-like" data-href="<%= GetUrl() %>" data-send="true" data-layout="button_count" data-show-faces="false" data-font="arial" style="margin:10px 0"></div>
	<% } %>


	<% if(CurrentItem.GooglePlus1) { %>
	<div class="g-plusone" data-size="medium" data-annotation="inline" data-href="<%= GetUrl() %>" style="margin:10px 0"></div>

	<script type="text/javascript">
		(function () {
			var po = document.createElement('script');
			po.type = 'text/javascript';
			po.async = true;
			po.src = 'https://apis.google.com/js/plusone.js';
			var s = document.getElementsByTagName('script')[0];
			s.parentNode.insertBefore(po, s);
		})();
	</script>
	<% } %>
</div>