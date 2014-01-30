using N2.Details;
using N2.Templates;
using N2.Templates.Items;
using N2.Integrity;

namespace N2.Addons.MyAddon.Items
{
    /// <summary>
    /// Since we're overriding "AbstractPage" from N2.Templates we get an editable title and a few zones for free.
    /// </summary>
    [Definition("My Page")]
    [WithEditableName]
    [AvailableZone("Content", "Content")]
    public class MyPage : AbstractPage
    {

        [EditableFreeTextArea("My Text", 100, ContainerName = Tabs.Content)]
        public virtual string Text
        {
            get { return GetDetail("Text", ""); }
            set { SetDetail("Text", value, ""); }
        }


        public override string TemplateUrl
        {
            get { return "~/Addons/MyAddon/UI/MyPage.aspx"; }
        }
    }
}
