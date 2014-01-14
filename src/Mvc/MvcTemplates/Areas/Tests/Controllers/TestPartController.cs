#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using N2.Web;
using N2.Templates.Mvc.Areas.Tests.Models;
using N2.Definitions;
using N2.Persistence;
using N2.Web.Mvc;
using N2.Templates.Mvc.Models.Pages;
using N2.Security;

namespace N2.Templates.Mvc.Areas.Tests.Controllers
{
    [Controls(typeof(TestPart))]
    public class TestPartController : ContentController
    {
        IDefinitionManager definitions;
        ContentActivator activator;
        private ISecurityManager security;

        public TestPartController(IDefinitionManager definitions, ContentActivator activator, ISecurityManager security)
        {
            this.definitions = definitions;
            this.activator = activator;
            this.security = security;
        }

        public override ActionResult Index()
        {
            if ("Tests" != (string)RouteData.DataTokens["area"])
                throw new Exception("Incorrect area: " + RouteData.Values["area"]);

            return PartialView(definitions.GetAllowedChildren(CurrentPage, null).WhereAuthorized(security, User, CurrentPage));
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult TheAction()
        {
            return Content("TheAction");
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            var part = activator.CreateInstance<TestPart>(CurrentItem);
            part.Name = name;
            part.Title = name;
            part.ZoneName = "TestParts";

            Engine.Persister.Save(part);

            return View("Index");
        }

        public ActionResult Remove()
        {
            Engine.Persister.Delete(CurrentItem);
            return ViewParentPage();
        }

        public ActionResult Random(string name, int amount, string discriminator)
        {
            var definition = definitions.GetDefinition(discriminator);

            List<ContentItem> created = new List<ContentItem> { CurrentPage };

            Random r = new Random();
            for (int i = 0; i < amount; i++)
            {
                var child = Create(created[r.Next(created.Count)], definition.ItemType, name, 1, i);
                Engine.Persister.Save(child);
                created.Add(child);
                HttpContext.Response.Write(name + " " + i + "<br/>");
                HttpContext.Response.Flush();
            }
            return Content("<script type='text/javascript'>window.location = '" + CurrentPage.Url + "'</script>");
        }

        public ActionResult Create(string name, int width, int depth, string discriminator)
        {
            var definition = definitions.GetDefinition(discriminator);
            CreateChildren(CurrentPage, definition.ItemType, name, width, depth);
            return Content("<script type='text/javascript'>window.location = '" + CurrentPage.Url + "'</script>");
        }

        private void CreateChildren(ContentItem parent, Type type, string name, int width, int depth)
        {
            if (depth == 0) return;

            for (int i = 1; i <= width; i++)
            {
                ContentItem item = Create(parent, type, name, depth, i);
                Engine.Persister.Repository.SaveOrUpdate(item);
                CreateChildren(item, type, name, width, depth - 1);
            }
            parent.ChildState |= Collections.CollectionState.ContainsVisiblePublicPages;
            Engine.Persister.Repository.SaveOrUpdate(parent);

            HttpContext.Response.Write(name + " 0-" + width + " (" + depth + ")" + "<br/>");
            HttpContext.Response.Flush();
        }

        private ContentItem Create(ContentItem parent, Type type, string name, int depth, int i)
        {
            ContentItem item = activator.CreateInstance(type, parent);
            item.Name = name + i;
            item.Title = name + " " + i + " (" + depth + ")";
            item.ChildState = Collections.CollectionState.IsEmpty;
            if (item is IContentPage)
                (item as IContentPage).Text = @"<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce nec sagittis mi. Donec pharetra vestibulum facilisis. Sed sodales risus vel nulla vulputate volutpat. Mauris vel arcu in purus porta dapibus. Aliquam erat volutpat. Maecenas suscipit tincidunt purus porttitor auctor. Quisque eget elit at justo facilisis malesuada sit amet sit amet eros. Duis convallis porta congue. Nulla commodo faucibus diam in mollis. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Donec ut nibh eu sapien ornare consectetur.</p>
<p>Aliquam id massa nec mi pellentesque rhoncus id vel neque. Pellentesque malesuada venenatis sollicitudin. Maecenas at nisl urna, vel feugiat ipsum. Integer imperdiet rhoncus libero sit amet ullamcorper. Vestibulum et purus et ipsum dignissim molestie id sed urna. Nulla vitae neque neque, tempor fermentum lectus. Proin pellentesque blandit diam, in vehicula ipsum suscipit vel. Pellentesque elementum feugiat egestas. Duis scelerisque metus suscipit massa mattis tempor. Vestibulum sed dolor sed felis pharetra semper eu sed quam. Nam vitae lectus nunc, in placerat dui. Vivamus massa lorem, faucibus in semper ac, tincidunt non massa.</p>";
            item.AddTo(parent);
            return item;
        }

        public ActionResult Json()
        {
            return Json(new { hello = "world", where = CurrentItem.Title }, JsonRequestBehavior.AllowGet); ;
        }
    }
}
#endif
