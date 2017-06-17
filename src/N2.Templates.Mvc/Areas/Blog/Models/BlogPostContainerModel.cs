using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Templates.Mvc.Areas.Blog.Models.Pages;
using N2.Web.Mvc.Html;

namespace N2.Templates.Mvc.Areas.Blog.Models
{
	public class BlogPostContainerModel
	{
		public BlogPostContainer Container { get; set; }
		public IList<BlogPost> Posts { get; set; }

        public bool IsLast { get; set; }
        public bool IsFirst { get; set; }
        public int Page { get; set; }
        public string Tag { get; set; }
        public int PostsPerPage { get; set; }
	}
}