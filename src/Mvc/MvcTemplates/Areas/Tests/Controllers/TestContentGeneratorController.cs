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
using N2.Edit.Versioning;
using N2.Engine;
using System.Threading;
using N2.Edit.FileSystem;

namespace N2.Templates.Mvc.Areas.Tests.Controllers
{
    [Controls(typeof(TestContentGenerator))]
    public class TestContentGeneratorController : ContentController<TestContentGenerator>
    {
        IDefinitionManager definitions;
        ContentActivator activator;
        private ISecurityManager security;

        public TestContentGeneratorController(IDefinitionManager definitions, ContentActivator activator, ISecurityManager security)
        {
            this.definitions = definitions;
            this.activator = activator;
            this.security = security;
        }

        public override ActionResult Index()
        {
            if ("Tests" != (string)RouteData.DataTokens["area"])
                throw new Exception("Incorrect area: " + RouteData.Values["area"]);

            ViewData["RemainingItems"] = remainingItems;

            if (CurrentItem.ShowEveryone || security.IsEditor(User))
                return PartialView(definitions.GetAllowedChildren(CurrentPage, null).WhereAuthorized(security, User, CurrentPage));
            else
                return Content("");
        }


        public ActionResult Test()
        {
            return View();
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
            return RedirectToParentPage();
        }

        public ActionResult AddVersions()
        {
            foreach (var item in Content.Traverse.DescendantPages())
            {
                var clone = item.Clone(false);
                clone.State = ContentState.Draft;
                var text = clone as IContentPage;
                if (text != null)
                    text.Text += ".";

                Engine.Resolve<IVersionManager>().ReplaceVersion(item, clone, true);

                HttpContext.Response.Write(item + "<br/>");
                HttpContext.Response.Flush();
            }

            return base.Content("<script type='text/javascript'>window.location = '" + CurrentPage.Url + "'</script>");
        }

        static int remainingItems;
        public ActionResult Random(string name, int amount, string discriminator, string relate, string background, string images)
        {
            var rootID = CurrentPage.ID;
            if (background == "on")
            {
                Engine.Resolve<IWorker>().DoWork(() =>
                {
                    Interlocked.Add(ref remainingItems, amount);
                    RandomCreator(rootID, name, amount, discriminator, relate == "on", images == "on", true, (n) =>
                    {
                        Interlocked.Decrement(ref remainingItems);
                    });
                });
                return RedirectToParentPage();
            }
            else
            {
                int i = 0;
                RandomCreator(rootID, name, amount, discriminator, relate == "on", images == "on", false, (n) => 
                {
                    i++;
                    HttpContext.Response.Write(n + " " + i + ", ");
                    if (i % 10 == 0)
                        HttpContext.Response.Write("<br/>");
                    HttpContext.Response.Flush();
                });
                return Content("<script type='text/javascript'>window.location = '" + CurrentPage.Url + "'</script>");
            }
        }

        public static void RandomCreator(int containerID, string name, int amount, string discriminator, bool relate, bool images, bool background, Action<string> callback)
        {
            var definition = Context.Current.Definitions.GetDefinition(discriminator);

            List<int> created = new List<int> { containerID };

            var start = N2.Utility.CurrentTime();

            var sentences = CreateSentences(1000, 10);
            for (int i = 0; i < amount; i++)
            {
                var child = Create(created[r.Next(created.Count)], definition.ItemType, name, 1, i, sentences, created, images);

                if (relate)
                {
                    var sibling = child.Parent.Children.FirstOrDefault(s => s != child);

                    child["ParentRelation"] = child.Parent;
                    child.Parent["ChildRelation"] = child;
                    child["Random"] = N2.Context.Current.Persister.Get(created[r.Next(created.Count)]);

                    var collection = child.GetDetailCollection("Relations", true);
                    collection.Add(child.Parent);
                    collection.Add(N2.Context.Current.Persister.Get(created[r.Next(created.Count)]));

                    child.Parent.GetDetailCollection("Relations", true).Add(child);

                    if (sibling != null)
                    {
                        collection.Add(sibling);
                        child["Sibling"] = sibling;
                    }

                    using (var tx = N2.Context.Current.Persister.Repository.BeginTransaction())
                    {
                        N2.Context.Current.Persister.Repository.SaveOrUpdate(child.Parent);
                        N2.Context.Current.Persister.Save(child);
                        tx.Commit();
                    }
                }
                else
                    N2.Context.Current.Persister.Save(child);
                
                created.Add(child.ID);

                callback(name);

                if (background)
                    N2.Context.Current.Persister.Dispose();
            }
        }

        public ActionResult Create(string name, int width, int depth, string discriminator, string background)
        {
            var definition = definitions.GetDefinition(discriminator);

            if (background == "on")
            {
                Interlocked.Add(ref remainingItems, Enumerable.Range(0, depth).Select(d => (int)Math.Pow(width, d)).Sum());

                Engine.Resolve<IWorker>().DoWork(() =>
                {
                    CreateChildren(CurrentPage, definition.ItemType, name, width, depth, true, (n) =>
                    {
                        Interlocked.Decrement(ref remainingItems);
                    });
                });
                return RedirectToParentPage();
            }
            else
            {
                CreateChildren(CurrentPage, definition.ItemType, name, width, depth, true, (n) =>
                {
                    HttpContext.Response.Write(n + " 0-" + width + " (" + depth + ")" + "<br/>");
                    HttpContext.Response.Flush();
                });

                return Content("<script type='text/javascript'>window.location = '" + CurrentPage.Url + "'</script>");
            }
        }

        private static void CreateChildren(ContentItem parent, Type type, string name, int width, int depth, bool background, Action<string> callback)
        {
            if (depth == 0) return;

            for (int i = 1; i <= width; i++)
            {
                ContentItem item = Create(parent.ID, type, name, depth, i, CreateSentences(width + 100, 10));
                if (depth > 1)
                    item.ChildState |= Collections.CollectionState.ContainsVisiblePublicPages;
                
                Context.Current.Persister.Save(item);
                CreateChildren(item, type, name, width, depth - 1, background, callback);
            }
            
            callback(name);
        }

        static Random r = new Random();
        private static ContentItem Create(int parentID, Type type, string name, int depth, int i, string[] sentences, List<int> linkSource = null, bool images = false)
        {
            var parent = Context.Current.Persister.Get(parentID);
            ContentItem item = Context.Current.Resolve<ContentActivator>().CreateInstance(type, parent, null, asProxy: true);
            item.Name = name + i;
            item.Title = name + " " + i + " (" + depth + ")";
            item.ChildState = Collections.CollectionState.IsEmpty;
            item.State = ContentState.Published;
            var fs = Context.Current.Resolve<IFileSystem>();
            var persister = Context.Current.Persister;
            var imageHtmls = images
                ? fs.GetFilesRecursive("~/Upload/").Where(f => f.Name.EndsWith("jpg")).Select(f => "<img src='" + f.VirtualPath + "' alt='" + f.Name + "'/>").ToList()
                : new List<string>();

            if (item is IContentPage)
            {
                (item as IContentPage).Text += string.Join("", Enumerable.Range(0, r.Next(1, 10)).Select(pi =>
                    "<p>" + string.Join(" ", Enumerable.Range(0, r.Next(8) + 2).Select(si =>
                        {
                            if (imageHtmls.Count > 0 && r.Next(100) < 5)
                                return imageHtmls[r.Next(imageHtmls.Count)];
                            if (r.Next(100) < 10 && linkSource != null && linkSource.Count > 1)
                                return "<a href='" + persister.Get(linkSource[r.Next(1, linkSource.Count)]).Url + "'>" + sentences[r.Next(sentences.Length)] + "</a>";
                            else
                                return sentences[r.Next(sentences.Length)];
                        }).ToArray()) + "</p>").ToArray());
            }
            parent.ChildState |= Collections.CollectionState.ContainsVisiblePublicPages;
            item.AddTo(parent);
            return item;
        }

        private static string[] CreateSentences(int sentenceCount, int sentenceLength)
        {
            var generator = new Services.MarkovNameGenerator(Services.Words.Thousand, 3, 2);
            var words = Enumerable.Range(0, sentenceCount * sentenceLength / 5).Select(i => generator.NextName.ToLower()).ToList();

            var r = new Random();
            var sentences = Enumerable.Range(0, sentenceCount)
                .Select(i => CreateSentence(sentenceLength, words, r))
                .ToArray();
            return sentences;
        }

        private static string CreateSentence(int sentenceLength, List<string> words, Random r)
        {
            return Capitalize(words[r.Next(0, words.Count)]) + " " + string.Concat(Enumerable.Range(0, sentenceLength / 2 + r.Next(sentenceLength - 1)).Select(i2 => " " + words[r.Next(0, words.Count)]).ToArray()) + ".";
        }

        private static string Capitalize(string word)
        {
            return word.Substring(0, 1).ToUpper() + word.Substring(1);
        }

        //    new[] {
        //"Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
        //"Fusce nec sagittis mi. Donec pharetra vestibulum facilisis.",
        //"Sed sodales risus vel nulla vulputate volutpat.",
        //"Mauris vel arcu in purus porta dapibus. Aliquam erat volutpat.",
        //"Maecenas suscipit tincidunt purus porttitor auctor.",
        //"Quisque eget elit at justo facilisis malesuada sit amet sit amet eros.", 
        //"Duis convallis porta congue.",
        //"Nulla commodo faucibus diam in mollis.", "Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Donec ut nibh eu sapien ornare consectetur", 
        //"Aliquam id massa nec mi pellentesque rhoncus id vel neque.", 
        //"Pellentesque malesuada venenatis sollicitudin.", 
        //"Maecenas at nisl urna, vel feugiat ipsum.", 
        //"Integer imperdiet rhoncus libero sit amet ullamcorper.", 
        //"Vestibulum et purus et ipsum dignissim molestie id sed urna.", 
        //"Nulla vitae neque neque, tempor fermentum lectus.", 
        //"Proin pellentesque blandit diam, in vehicula ipsum suscipit vel.", 
        //"Pellentesque elementum feugiat egestas.", 
        //"Duis scelerisque metus suscipit massa mattis tempor.", 
        //"Vestibulum sed dolor sed felis pharetra semper eu sed quam.", 
        //"Nam vitae lectus nunc, in placerat dui.", 
        //"Vivamus massa lorem, faucibus in semper ac, tincidunt non massa."
        //};

    }
}
#endif
