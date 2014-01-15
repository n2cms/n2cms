//using System.Web.UI;
//using System.Web.UI.WebControls;
//using N2.Definitions;
//using N2.Details;

//namespace N2.Edit.Wizard.Details
//{
//    public class EditableDefinitionAttribute : AbstractEditableAttribute
//    {
//        public EditableDefinitionAttribute(string title, int sortOrder)
//            : base(title, sortOrder)
//        {
//            Required = true;
//        }

//        public override bool UpdateItem(ContentItem item, Control editor)
//        {
//            DropDownList ddl = editor as DropDownList;
//            item[Name] = ddl.SelectedValue;
//            return true;
//        }

//        public override void UpdateEditor(ContentItem item, Control editor)
//        {
//            DropDownList ddl = editor as DropDownList;
//            string value = item[Name] as string;
//            if(ddl.Items.FindByValue(value) != null)
//                ddl.SelectedValue = value;
//        }

//        protected override Control AddEditor(Control container)
//        {
//            DropDownList ddl = new DropDownList();
//            ddl.ID = Name;
//            ddl.Items.Add(string.Empty);
//            foreach(ItemDefinition definition in N2.Context.Definitions.GetDefinitions())
//            {
//                ddl.Items.Add(new ListItem(definition.Title, definition.Discriminator));
//            }
//            container.Controls.Add(ddl);

//            return ddl;
//        }
//    }
//}
