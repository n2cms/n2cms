<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JQuerySocialBookmarks.ascx.cs" Inherits="N2.Templates.UI.Parts.JQuerySocialBookmarks" %>
<%
	// Don't show the bookmarks on an error page
	if (Request.Url.ToString().IndexOf("aspxerrorpath") == -1)
	{
%>
<!-- This is + CSS are temporary until it uses an embedded resources -->
<script type="text/javascript">
	/*-
	* Based on shareit by Fredrik Lindberg - http://www.shapeshifter.se
	* Modified by C Small.
	*/
	if (typeof $ != 'undefined')
	{
		(function($)
		{
			$.fn.socialBookmarks = function(settings)
			{
				// Default settings
				if (settings == null)
				{
					var settings =
					{
						show: ['delicious', 'digg', 'stumbleupon', 'twitter', 'technorati', 'google', 'facebook', 'reddit', 'diigo', 'blogmarks', 'blinklist', 'magnolia'],
						icon_path: '/Upload/SocialBookmarks/',
						size: "16",
						extra_sites: [] // format: [{ "name": "website","image" : "xyx24x24.png","title":"alt tag","url":"url.aspx?url=%url%&title=%title"}]
					};
				}
				else if (settings.show == "all")
				{
					settings.show = ['delicious', 'digg', 'stumbleupon', 'twitter', 'technorati', 'google', 'facebook', 'reddit', 'diigo', 'blogmarks', 'blinklist', 'magnolia'];
				}

				//http: //www.dotnetkicks.com/kick/?url=http://devlicio.us/blogs/tuna_toksoz/archive/2009/03/14/an-improvement-on-sessionfactory-initialization.aspx

				// The list of submission urls and icons
				var imgSize = settings.size + "x" + settings.size + ".png";
				var submissionInfo = [{
							"name": "delicious",
							"image": "delicious_24x24.png",
							"title": "Bookmark at Del.icio.us",
							"url": "http://delicious.com/save?jump=yes&v=4&noui&url=%url%&title=%title%"
						},
						{
							"name": "digg",
							"image": "Digg_24x24.png",
							"title": "Digg This!",
							"url": "http://digg.com/submit?url=%url%&title=%title%"
						},
						{
							"name": "stumbleupon",
							"image": "Stumbleupon_24x24.png",
							"title": "Stumble upon",
							"url": "http://www.stumbleupon.com/submit?url=%url%&title=%title%"
						},
						{
							"name": "google",
							"image": "Google_24x24.png",
							"title": "Google Bookmarks",
							"url": "http://www.google.com/bookmarks/mark?op=edit&output=popup&bkmk=%url%&title=%title%"
						},
						{
							"name": "twitter",
							"image": "Twitter_24x24.png",
							"title": "Twitter",
							"url": "http://twitthis.com/twit?url=%url%"
						},
						{
							"name": "reddit",
							"image": "Reddit_24x24.png",
							"title": "Reddit",
							"url": "http://reddit.com/submit?url=%url%&title=%title%"
						},
						{
							"name": "diigo",
							"image": "Diigo_24x24.png",
							"title": "Diigo",
							"url": "http://www.diigo.com/post?url=%url%&title=%title%"
						},
						{
							"name": "facebook",
							"image": "FaceBook_24x24.png",
							"title": "Share on Facebook",
							"url": "http://www.facebook.com/sharer.php?u=%url%&t=%title%"
						},
						{
							"name": "blogmarks",
							"image": "BlogMarks_24x24.png",
							"title": "BlogMarks",
							"url": "http://blogmarks.net/my/new.php?mini=1&simple=1&url=%url%&title=%title%"
						},
						{
							"name": "blinklist",
							"image": "Blinklist_24x24.png",
							"title": "Blinklist",
							"url": "http://www.blinklist.com/index.php?Action=Blink/addblink.php&Url=%url%&Title=%title%"
						},
						{
							"name": "magnolia",
							"image": "Magnolia_24x24.png",
							"title": "ma.gnolia",
							"url": "http://ma.gnolia.com/bookmarklet/add?url=%url%&title=%title%"
						},
						{
							"name": "technorati",
							"image": "Technorati_24x24.png",
							"title": "Technorati",
							"url": "http://technorati.com/faves?add=%url%"
						}];

				if (settings.extra_sites.length > 0)
				{
					// Push the extra items in
					submissionInfo.push(settings.extra_sites);
				}

				// Go through all sites setup to show
				for (i = 0; i < settings.show.length; i++)
				{
					// Find inside the submission object
					for (n = 0; n < submissionInfo.length; n++)
					{
						if (settings.show[i] == submissionInfo[n].name)
						{
							var url = submissionInfo[n].url;
							var img = submissionInfo[n].image;

							// Set a different image size
							if (settings.size != "24")
								img = img.replace(/24x24\.png/g, imgSize);

							url = url.replace("%url%", encodeURI(location.href));
							url = url.replace("%title%", escape(document.title));

							var link = '<a href="' + url + '" title="' + submissionInfo[n].title + '" target="_blank">';
							link += '<img src="' + settings.icon_path + img + '" border="0"></a>';

							$(this).append(link);
						}
					}
				}
			}
		})(jQuery);
	};
</script>

<script type="text/javascript">
	$(document).ready(function()
	{
		$("#socialbookmarks").socialBookmarks({
		show: "all", 
		icon_path: '/Templates/UI/Img/SocialBookmarks/',
		size: "16",
		extra_sites: []
		});
	});
</script>
<style type="text/css">
.rounded
{
	-moz-border-radius-topleft : 3px;
	-moz-border-radius-topright : 3px;
	-moz-border-radius-bottomleft : 3px;
	-moz-border-radius-bottomright : 3px;
	
	-webkit-border-top-left-radius : 3px;
	-webkit-border-top-right-radius : 3px;
	-webkit-border-bottom-left-radius : 3px;
	-webkit-border-bottom-right-radius : 3px;
}
</style>
<span id="socialbookmarks" class="rounded"></span>
<%
	}
%>