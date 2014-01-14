using N2.Details;
using N2.Integrity;

namespace N2.Addons.Tagging.Items
{
    [PageDefinition(IconUrl = "~/Addons/Tagging/UI/tag_green.png", TemplateUrl = "~/Addons/Tagging/UI/Tag.aspx")]
    [RestrictParents(typeof(TagGroup))]
    [WithEditableName, WithEditableTitle]
    public class Tag : ContentItem, ITag
    {
        public Tag()
        {
            Visible = false;
        }

        #region ITag Members

        public int ReferenceCount
        {
            get
            {
                return Find.Items.Where.Detail().Eq(this).Count();
            }
        }

        public IGroup Category
        {
            get { return Parent as IGroup; }
        }

        #endregion

        
    }
}
