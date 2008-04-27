using System;
using N2;
using N2DevelopmentWeb.Domain;
using N2.Web.UI;

namespace N2DevelopmentWeb.Uc
{
	public partial class ItemCreatorUI : UserControl<ContentItem, ContentItem>
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void btnCreate_Click(object sender, EventArgs e)
		{
			string prefix = txtPrefix.Text;
			int count = int.Parse(txtCount.Text);
			int depth = int.Parse(txtDepth.Text);
			N2.ContentItem parent = CurrentPage;

			CreateItems(depth, count, parent, prefix);
		}

		private void CreateItems(int depth, int count, ContentItem parent, string prefix)
		{
			if(depth <= 0)
				return;
			
			for (int i = 0; i < count; i++)
			{
				ContentItem item = N2.Context.Definitions.CreateInstance<MyPageData>(parent);
				item.Title = prefix + i;
				item.Name = prefix + i;
				N2.Context.Persister.Save(item);
				CreateItems(depth-1, count, item, prefix);
			}
		}
	}
}