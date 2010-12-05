using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using N2.Collections;
using N2.Persistence.Finder;
using N2.Templates.Mvc.Areas.Blog.Models;
using N2.Templates.Mvc.Areas.Blog.Models.Pages;
using N2.Templates.Mvc.Controllers;
using N2.Web;
using N2.Web.Mvc;

namespace N2.Templates.Mvc.Areas.Blog.Controllers
{
    /// <summary>
    /// This controller returns a view that displays the item created via the management interface
    /// </summary>
    [Controls(typeof(Models.Pages.BlogPostContainer))]
    public class BlogPostContainerController : ContentController<Models.Pages.BlogPostContainer>
    {
        IItemFinder finder;
        public BlogPostContainerController(IItemFinder finder)
        {
            this.finder = finder;
        }

        public override ActionResult Index()
        {
            var model = GetPosts(string.Empty, 1, CurrentItem.PostsPerPage);
            return View(model);
        }

        public ActionResult Page(int? p)
        {
            // Default page to 1 if null or <= 0
            int page = p == null || p <= 0 ? 1 : p.Value;

            var model = GetPosts(string.Empty, page, CurrentItem.PostsPerPage);
            return View("Index", model);
        }

        public ActionResult Tag(string t, int? p)
        {
            // If tag is empty redirect to main
            if (string.IsNullOrEmpty(t))
            {
                return RedirectToAction("Index");
            }

            // Default page to 1 if null or <= 0
            p = p == null || p <= 0 ? 1 : p.Value;

            var model = GetPosts(t, p.Value, CurrentItem.PostsPerPage);

            ViewData["Tag"] = t;
            return View("Index", model);
        }

        private BlogPostContainerModel GetPosts(string tag, int page, int postCount)
        {
            int skip = (page - 1) * postCount;
            int take = postCount;

            IList<BlogPost> posts;

            if (!string.IsNullOrEmpty(tag))
            {
                posts = finder.Where.Type.Eq(typeof(BlogPost))
                        .And.Parent.Eq(CurrentPage)
                        .And.Detail("Tags").Like("%" + tag + "%")
                        .FirstResult(skip)
                        .MaxResults(take + 1)
                        .OrderBy.Published.Desc
                        .Select<BlogPost>();
            }
            else
            {
                posts = finder.Where.Type.Eq(typeof(BlogPost))
                        .And.Parent.Eq(CurrentPage)
                        .FirstResult(skip)
                        .MaxResults(take + 1)
                        .OrderBy.Published.Desc
                        .Select<BlogPost>();
            }

            var model = new BlogPostContainerModel
            {
                Container = CurrentItem,
                Posts = posts,
                Page = page,
                Tag = tag,
                IsLast = posts.Count <= take,
                IsFirst = page == 1
            };
            if (!model.IsLast)
                model.Posts.RemoveAt(model.Posts.Count - 1);

            return model;
        }
    }
}