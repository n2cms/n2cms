namespace N2.Tests.Definitions.Definitions
{
    [N2.Definition("Item With Details", ToolTip = "Detailed item tooltip", SortOrder = 123)]
    [N2.Integrity.AllowedChildren(typeof(ItemInZone1Or2))]
    [N2.Integrity.RestrictParents(typeof(ItemInZone1Or2))]
    [N2.Integrity.AvailableZone("Zone1", "Zone1")]
    [N2.Integrity.AvailableZone("Zone2", "Zone2")]
    public class ItemWithDetails : N2.ContentItem
    {
        [N2.Details.Editable("TestProperty1", typeof(System.Web.UI.WebControls.TextBox), "Text", 100)]
        [N2.Web.UI.EditorModifier("TextMode", System.Web.UI.WebControls.TextBoxMode.Password)]
        public virtual string TestProperty1
        {
            get { return (string)(GetDetail("TestProperty1") ?? ""); }
            set { SetDetail<string>("TestProperty1", value); }
        }

        [N2.Tests.Definitions.Definitions.Details.CustomizedEditable]
        public virtual string TestProperty2
        {
            get { return (string)(GetDetail("TestProperty2") ?? ""); }
            set { SetDetail<string>("TestProperty2", value); }
        }

        [N2.Details.EditableFreeTextArea("TestProperty3", 333)]
        public virtual string TestProperty3
        {
            get { return (string)(GetDetail("TestProperty2") ?? ""); }
            set { SetDetail<string>("TestProperty2", value); }
        }

        [N2.Details.EditableText("TestProperty4", 444)]
        [N2.Web.UI.EditorModifier("Text", "hello")]
        public virtual string TestProperty4
        {
            get { return (string)(GetDetail("TestProperty2") ?? ""); }
            set { SetDetail<string>("TestProperty2", value); }
        }

        public override string IconUrl
        {
            get
            {
                return "~/Url/To/Icon.gif";
            }
        }
    }
}
