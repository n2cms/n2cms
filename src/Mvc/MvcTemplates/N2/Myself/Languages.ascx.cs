using System;
using System.Collections.Generic;
using N2.Details;
using N2.Engine.Globalization;
using N2.Persistence.Finder;
using N2.Persistence;
using System.Linq;

namespace N2.Management.Myself
{
    [PartDefinition("Languages", 
        TemplateUrl = "{ManagementUrl}/Myself/Languages.ascx",
        IconUrl = "{ManagementUrl}/Resources/icons/world.png")]
    [WithEditableTitle("Title", 10)]
    public class LanguagesPart : RootPartBase
    {
        public override string Title
        {
            get { return base.Title ?? "Languages"; }
            set { base.Title = value; }
        }

        [EditableNumber("Latest changes max count", 100)]
        public virtual int LatestChangesMaxCount
        {
            get { return (int)(GetDetail("LatestChangesMaxCount") ?? 2); }
            set { SetDetail("LatestChangesMaxCount", value); }
        }
    }

    public class LanguageViewModel
    {
        public ILanguage Language { get; set; }
        public int TotalItems { get; set; }
        public ContentItem Root { get; set; }
        public IList<ContentItem> Changes { get; set; }
    }

    public partial class Languages : N2.Web.UI.ContentUserControl<ContentItem, LanguagesPart>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var lg = Engine.Resolve<N2.Engine.Globalization.ILanguageGateway>();

            List<LanguageViewModel> languages = new List<LanguageViewModel>();
            foreach (var language in lg.GetAvailableLanguages())
            {
                var root = language as ContentItem;
                if (root != null && root.ID != 0)
                {
                    var lvm = new LanguageViewModel { Language = language, Root = root };

                    var q = Parameter.BelowOrSelf(root);
                    lvm.TotalItems = (int)Engine.Persister.Repository.Count(q);
                    lvm.Changes = Engine.Persister.Repository.Find(q.Take(CurrentItem.LatestChangesMaxCount)).ToList();
                    //var q = Engine.Resolve<IItemFinder>()
                    //  .Where.AncestralTrail.Like(likeness)
                    //  .Or.ID.Eq(root.ID);

                    //lvm.TotalItems = q.Count();
                    //lvm.Changes = q.OrderBy.Updated.Desc
                    //  .MaxResults(CurrentItem.LatestChangesMaxCount)
                    //  .Select();
                    
                    languages.Add(lvm);
                }
            }
            rptLanguages.DataSource = languages;
        
            DataBind();
        }
    }
}
