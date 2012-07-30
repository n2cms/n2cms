using System;
namespace N2.Tests.Edit.Items
{
	[N2.Web.UI.FieldSetContainer("first", "Fieldset with two fieldsets", 10)]
    [N2.Web.UI.FieldSetContainer("inside1", "Fieldset with one fieldset inside", 20, ContainerName = "first")]
    [N2.Web.UI.FieldSetContainer("inside2", "A fieldset", 30, ContainerName = "first")]
    [N2.Web.UI.FieldSetContainer("inside3", "A fieldset", -10, ContainerName = "first")]
    [N2.Web.UI.FieldSetContainer("inside1_1", "A fieldset within two fieldsets", 40, ContainerName = "inside1")]
    public class ComplexContainersItem : N2.ContentItem
	{
		public ComplexContainersItem()
		{
		}
		public ComplexContainersItem(int id, string name)
		{
			ID = id;
			Name = name;
		}

		[N2.Details.EditableText("My Property 0", 0)]
        public virtual string MyProperty0
        {
            get { return (string)(GetDetail("MyProperty0") ?? ""); }
            set { SetDetail<string>("MyProperty0", value); }
        }
        [N2.Details.EditableText("My Property 1", 1, ContainerName = "first")]
		public virtual string MyProperty1
		{
            get { return (string)(GetDetail("MyProperty1") ?? ""); }
            set { SetDetail<string>("MyProperty1", value); }
		}
        [N2.Details.EditableText("My Property 2", 2, ContainerName = "inside1")]
        public virtual string MyProperty2
        {
            get { return (string)(GetDetail("MyProperty2") ?? ""); }
            set { SetDetail<string>("MyProperty2", value); }
        }
		[N2.Details.EditableFreeTextArea("My Property 3", 3, ContainerName = "inside2")]
        public virtual string MyProperty3
        {
            get { return (string)(GetDetail("MyProperty3") ?? ""); }
            set { SetDetail<string>("MyProperty3", value); }
        }
        [N2.Details.EditableCheckBox("My Property 4", 4, ContainerName = "inside1_1")]
        public virtual bool MyProperty4
        {
			get { return (bool)(GetDetail("MyProperty4") ?? false); }
            set { SetDetail<bool>("MyProperty4", value); }
		}

		[N2.Details.EditableCheckBox("My Property 5", 2001, AuthorizedRoles = new [] { "ÜberEditor" })]
		public virtual bool MyProperty5
		{
			get { return (bool)(GetDetail("MyProperty5") ?? false); }
			set { SetDetail<bool>("MyProperty5", value); }
		}

		[N2.Details.EditableText("My Property 6", 2010, AuthorizedRoles = new [] { "ÜberEditor" })]
		public virtual string MyProperty6
		{
			get { return (string)(GetDetail("MyProperty6") ?? false); }
			set { SetDetail<string>("MyProperty6", value); }
		}
    }
}
