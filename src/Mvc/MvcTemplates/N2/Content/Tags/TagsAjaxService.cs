using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using N2.Web;
using N2.Edit;

namespace N2.Management.Content.Tags
{
    [Service(typeof(IAjaxService))]
    public class TagsAjaxService : IAjaxService
    {
        private readonly Persistence.TagsRepository tags;
        private readonly IEngine engine;

        public TagsAjaxService(Persistence.TagsRepository tags, IEngine engine)
        {
            this.tags = tags;
            this.engine = engine;
        }

        public string Name
        {
            get { return "tags"; }
        }

        public bool RequiresEditAccess
        {
            get { return true; }
        }

        public bool IsValidHttpMethod(string httpMethod)
        {
            return true;
        }

        public void Handle(HttpContextBase context)
        {
            //throw new Exception();

            string tagName = context.Request["tagName"];
            string term = context.Request["term"];

            var selection = new SelectionUtility(context, engine);
            var startPage = engine.Content.Traverse.ClosestStartPage(selection.SelectedItem);

            var allTags = engine.Resolve<CacheWrapper>()
                .GetOrCreate<IEnumerable<string>>("Tags" + startPage, () => tags.FindTags(startPage, tagName));
            var json = allTags
                .Where(t => t.ToLower().Contains(term))
                .Select(t => new { label = t })
                .ToList()
                .ToJson();

            context.Response.ContentType = "application/json";
            context.Response.Write(json);
        }
    }
}