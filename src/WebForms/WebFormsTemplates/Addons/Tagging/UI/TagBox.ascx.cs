using System;
using System.Collections.Generic;
using N2.Addons.Tagging.Details;
using N2.Details;
using N2.Web.UI;

namespace N2.Addons.Tagging.UI
{
    public partial class TagBox : ContentUserControl<ContentItem, Items.TagBox>
    {
        protected List<AppliedTags> Categories { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Categories = new List<AppliedTags>();
            DetailCollection tags = CurrentPage.GetDetailCollection("Tags", false);
            if(tags == null)
                return;
            
            foreach(ITag tag in tags)
            {
                AppliedTags applied = Categories.Find(at => at.Group == tag.Category);
                if(applied == null)
                {
                    applied = new AppliedTags {Group = tag.Category, Tags = new List<string>()};
                    Categories.Add(applied);
                }
                applied.Tags.Add(tag.Title);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Visible = Categories.Count > 0;
            base.OnPreRender(e);
        }
    }
}
