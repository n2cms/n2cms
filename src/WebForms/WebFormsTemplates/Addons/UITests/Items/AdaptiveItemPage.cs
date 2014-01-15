using N2.Integrity;
using N2.Web;
using N2.Details;
using N2.Edit.Trash;
using N2.Templates.Items;
using N2.Collections;
using System.Collections.Generic;
using N2.Definitions;

namespace N2.Addons.UITests.Items
{
    [Definition("Multiple Tests Page", SortOrder = 10000)]
    [WithEditableTitle, WithEditableName]
    [Throwable(AllowInTrash.No)]
    [RestrictParents(typeof(AbstractContentPage))]
    public class AdaptiveItemPage : ContentItem
    {
        /// <summary>Property binding style</summary>
        [EditableUserControl("UC 1", "~/Addons/UITests/UI/EditableUc1.ascx", "PropertyNameOnUserControl", 111)]
        public virtual string Uc1 { get; set; }

        /// <summary>IContentBinder style</summary>
        [EditableUserControl("UC 2", "~/Addons/UITests/UI/EditableUc2.ascx", 112)]
        public virtual string Uc2 { get; set; }

        [EditableFreeTextArea("Text", 100)]
        public virtual string Text
        {
            get { return GetDetail("Text", ""); }
            set { SetDetail("Text", value, ""); }
        }

        public override PathData FindPath(string remainingUrl)
        {
            PathData data = base.FindPath(remainingUrl);
            if(data.CurrentItem != null && data.CurrentItem != this)
                return data;

            if(string.IsNullOrEmpty(remainingUrl))
                return new PathData(this, "~/Addons/UITests/UI/AdaptiveItem.aspx");

            return new PathData(this, "~/Addons/UITests/UI/" + remainingUrl + ".aspx");
        }


        [EditableChildren("None items", "None", 1000)]
        public virtual ItemList NoneItems
        {
            get { return GetChildren(new ZoneFilter("NoneItems")); }
        }

        [EditableChildren("None items", "Any", 1000)]
        public virtual IList<UITestContentCreator> UITestItemItems
        {
            get { return new ItemList<UITestContentCreator>(GetChildren(new ZoneFilter("Any"))); }
        }
    }
}
