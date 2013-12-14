using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using N2.Web;
using N2.Edit;
using N2.Web.UI;

namespace N2.Management.Content.Tags
{
    [Service(typeof(IAjaxService))]
    public class TagsAjaxService : IAjaxService
    {
        private Persistence.TagsRepository tags;
        private IEngine engine;

        public TagsAjaxService(N2.Persistence.TagsRepository tags, IEngine engine)
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
            string tagName = context.Request["tagName"];
            string term = context.Request["term"];
            
            var selection = new SelectionUtility(context, engine);
            var startPage = engine.Content.Traverse.ClosestStartPage(selection.SelectedItem);

            var allTags = engine.Resolve<CacheWrapper>()
                .GetOrCreate<IEnumerable<string>>("Tags" + startPage, () => tags.FindTags(startPage, tagName));
            var json = allTags
                .Where(t => t.StartsWith(term, StringComparison.InvariantCultureIgnoreCase))
                .Select(t => new { label = t })
                .ToJson();
            context.Response.ContentType = "application/json";
            context.Response.Write(json);
        }
    }
}
