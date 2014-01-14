using System;
using N2.Templates.Mvc.Models.Parts;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Templates.Mvc.Models
{
    public class SocialBookmarksModel : IItemContainer<SocialBookmarks>
    {
        public SocialBookmarksModel(SocialBookmarks item)
        {
            CurrentItem = item;
        }

        public SocialBookmarks CurrentItem { get; private set; }

        /// <summary>Gets the item associated with the item container.</summary>
        ContentItem IItemContainer.CurrentItem
        {
            get{ return CurrentItem; }
        }

        public Bookmark[] Bookmarks
        {
            get
            {
                return new[]
                        {
                            new Bookmark("http://delicious.com/save?jump=yes&v=4&noui&url={0}&title={1}",
                                         "delicious.png",
                                         "del.icio.us"),
                            new Bookmark("http://digg.com/submit?phase=2&amp;url={0}&amp;title={1}",
                                         "digg.png", "Digg it"),
                            new Bookmark("http://www.dotnetkicks.com/kick/?url={0}",
                                         "dotnetkicks.png", "Dotnetkicks"),
                            new Bookmark("http://www.dzone.com/links/add.html?url={0}&amp;title={1}",
                                         "dzone.png", "DZone"),
                            new Bookmark("http://www.google.com/bookmarks/mark?op=edit&amp;output=popup&amp;bkmk={0}&amp;title={1}",
                                         "google.png", "Google"),
                            new Bookmark("https://favorites.live.com/quickadd.aspx?url={0}&amp;title={1}",
                                         "live.png", "Live"),
                            new Bookmark("http://www.netscape.com/submit/?U={0}&amp;T={1}",
                                         "netscape.gif", "Netscape"),
                            new Bookmark("http://reddit.com/submit?url={0}&amp;title={1}",
                                         "reddit.png", "Reddit"),
                            new Bookmark("http://slashdot.org/bookmark.pl?url={0}&amp;title={1}",
                                         "slashdot.png", "Slashdot"),
                            new Bookmark("http://technorati.com/faves?sub=addfavbtn&amp;add={0}",
                                         "technorati.png", "Technorati"),
                            new Bookmark("http://myweb2.search.yahoo.com/myresults/bookmarklet?t={1}&amp;u={0}",
                                         "yahoomyweb.png", "Yahoo MyWeb")
                        };
            }
        }

        public class Bookmark
        {
            public Bookmark(string urlFormat, string imageName, string text)
            {
                UrlFormat = urlFormat;
                ImageName = imageName;
                Text = text;
            }

            public string UrlFormat { get; set; }

            public string ImageName { get; set; }

            public string Text { get; set; }
        }
    }
}
