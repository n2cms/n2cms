using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Addons.Tagging.Items;
using N2.Resources;

namespace N2.Addons.Tagging.Details.WebControls
{
    internal class TagsEditor : Control, INamingContainer
    {
        HiddenField countField = new HiddenField();
        Table containersTable = new Table();
        
        protected int Count
        {
            get { return Convert.ToInt32(countField.Value); }
            set { countField.Value = value.ToString(); }
        }

        public bool HasChanges
        {
            get { return (bool)(ViewState["HasChanges"] ?? false); }
            set { ViewState["HasChanges"] = value; }
        }
        


        public TagsEditor()
        {
            countField.ID = "tagsCount";
        }



        public void BindTo(IList<AppliedTags> selectedTags)
        {
            Count = selectedTags.Count;
            EnsureChildControls();

            int index = 0;
            foreach(AppliedTags change in selectedTags)
            {
                TagsRow row = containersTable.Controls[index++] as TagsRow;
                row.BindTo(change.Group);
                row.Select(change.Tags);
            }
        }

        public IEnumerable<AppliedTags> GetAddedTags(IList<TagGroup> containers)
        {
            for (int i = 0; i < Count; i++)
            {
                IEnumerable<string> tags = ((TagsRow) containersTable.Controls[i]).GetAddedTags();
                yield return new AppliedTags
                {
                    Group = containers[i],
                    Tags = new List<string>(tags)
                };
            }
        }



        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            containersTable.CssClass = "tags";
            
            Register.JQuery(Page);
            Register.JavaScript(Page, "~/Addons/Tagging/UI/EditableTags.js");
            Register.StyleSheet(Page, "~/Addons/Tagging/UI/EditableTags.css");

            Controls.AddAt(0, countField);
            Controls.AddAt(1, containersTable);

            countField.Value = Page.Request.Form[countField.UniqueID] ?? "0";

            if(Page.IsPostBack)
                EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            for (int i = 0; i < Count; i++)
            {
                TagsRow row = new TagsRow(this);
                row.ID = "row" + i;
                containersTable.Controls.Add(row);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Visible = Count > 0;
            base.OnPreRender(e);
        }
    }
}
