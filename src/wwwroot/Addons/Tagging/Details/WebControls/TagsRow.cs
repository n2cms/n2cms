using System.Web.UI.WebControls;
using System.Collections.Generic;
using N2.Addons.Tagging.Details.WebControls;

namespace N2.Addons.Tagging.Details.WebControls
{
	internal class TagsRow : TableRow
	{
		readonly TagsEditor table;
		ListBox AvailableList { get; set; }
		TextBox AdditionalBox { get; set; }
		ListBox AddedList { get; set; }
		Button UntagButton { get; set; }
		Button TagButton { get; set; }
		Button AddButton { get; set; }
		Label Label { get; set; }


		public TagsRow(TagsEditor table)
		{
			this.table = table;

			AddedList = new ListBox();
			TagButton = new Button();
			UntagButton = new Button();
			AddButton = new Button();
			AdditionalBox = new TextBox();
			AvailableList = new ListBox();
			Label = new Label();

			Label.CssClass = "editorLabel";
			AddedList.SelectionMode = ListSelectionMode.Multiple;
			TagButton.Text = "<";
			UntagButton.Text = ">";
			AddButton.Text = "+";
			AvailableList.SelectionMode = ListSelectionMode.Multiple;

			TagButton.Click += delegate
				{
					table.HasChanges = true;
					MoveBetween(AvailableList, AddedList);
				};
			UntagButton.Click += delegate
				{
					table.HasChanges = true;
					MoveBetween(AddedList, AvailableList);
				};
			AddButton.Click += delegate
				{
					table.HasChanges = true;
					AddNewTag(AdditionalBox.Text);
					AdditionalBox.Text = "";
				};
		}

		private void AddNewTag(string tagName)
		{
			if (AddedList.Items.FindByValue(tagName) != null )
				return;

			if (AvailableList.Items.FindByValue(tagName) != null)
			{
				var li = AvailableList.Items.FindByValue(tagName);
				AvailableList.Items.Remove(li);
				AddedList.Items.Add(li);
				return;
			}
			
			AddedList.Items.Add(tagName);
		}



		private void MoveBetween(ListBox from, ListBox to)
		{
			while(from.SelectedItem != null)
			{
				ListItem selected = from.SelectedItem;

				from.Items.Remove(selected);
				to.Items.Add(selected);
			}
		}

		public void BindTo(ITagCategory container)
		{
			EnsureChildControls();

			Label.Text = container.Title;

			if(AvailableList.Items.Count != 0 || AddedList.Items.Count > 0)
				return;
			
			foreach (ITag tag in container.GetTags())
			{
				ListItem item = new ListItem(tag.Title + " (" + tag.ReferenceCount + ")", tag.Title);
				AvailableList.Items.Add(item);
			}
		}

		public void Select(IEnumerable<string> selectedTags)
		{
			foreach(string tagName in selectedTags)
			{
				ListItem li = AvailableList.Items.FindByValue(tagName);
				if(li != null)
				{
					AvailableList.Items.Remove(li);
					AddedList.Items.Add(li);
				}
			}
		}

	
		
		protected override void OnInit(System.EventArgs e)
		{
			base.OnInit(e);
			CssClass = "tag";
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			TableCell labelCell = new TableCell();
			labelCell.CssClass = "tagsLabel";
			TableCell leftCell = new TableCell();
			leftCell.CssClass = "tagsLeft";
			Panel addPanel = new Panel();
			TableCell middleCell = new TableCell();
			middleCell.CssClass = "tagsMiddle";
			TableCell rightCell = new TableCell();
			rightCell.CssClass = "tagsRight";

			Controls.Add(labelCell);
			Controls.Add(leftCell);
			Controls.Add(middleCell);
			Controls.Add(rightCell);

			labelCell.Controls.Add(Label);
			leftCell.Controls.Add(AddedList);
			leftCell.Controls.Add(addPanel);
			addPanel.Controls.Add(AdditionalBox);
			addPanel.Controls.Add(AddButton);
			middleCell.Controls.Add(TagButton);
			middleCell.Controls.Add(UntagButton);
			rightCell.Controls.Add(AvailableList);
		}

		public IEnumerable<string> GetAddedTags()
		{
			foreach (ListItem item in AddedList.Items)
			{
				yield return item.Value;
			}
		}
	}
}