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
				MyPageData item = N2.Context.Definitions.CreateInstance<MyPageData>(parent);
				item.Title = prefix + i;
				item.Name = prefix + i;
				item.Text = @"<p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Pellentesque placerat commodo turpis. Ut tempor interdum leo. In hac habitasse platea dictumst. Proin arcu velit, semper nec, feugiat vel, ultrices a, eros. Integer purus. Sed mollis tellus quis arcu. Nam ut erat. Integer purus. Integer dolor felis, porta ac, commodo ac, hendrerit vel, felis. Quisque cursus bibendum lorem. Curabitur cursus, magna et tincidunt bibendum, ante urna egestas orci, ut placerat arcu dolor a neque. Nulla dapibus tincidunt risus. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Pellentesque mattis. Donec tempor libero eget magna. Phasellus a mi et massa lobortis faucibus.</p>
<p>Aenean convallis aliquet orci. Nullam posuere ante vitae libero. Sed vel lectus. Proin porta neque eget massa. Nulla vitae sapien. In placerat ante. In volutpat eros quis nisi placerat hendrerit. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Quisque dapibus nibh id turpis. Aenean semper fermentum dui. Vivamus sed lacus. Donec commodo diam sit amet tellus. Fusce sed justo quis purus mattis tristique. Suspendisse dapibus justo vitae libero. Mauris tempor tempus velit. Aenean aliquam hendrerit nisl. Nullam est metus, suscipit vitae, pulvinar a, mattis at, velit. Ut egestas. Vivamus massa dui, eleifend vitae, posuere sit amet, pulvinar ac, mi.</p>
<p>Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin id orci nec est dapibus pretium. Sed sagittis, tellus sit amet molestie pulvinar, mauris felis consequat augue, ac dictum nulla mauris eget lacus. Nullam viverra. Donec eleifend lectus sit amet nunc. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Cras lacinia, nibh sed ornare lacinia, felis orci rhoncus mauris, ut cursus tellus ante ut dui. Integer dictum aliquet nibh. Nunc feugiat risus at nisl. Duis ac elit sit amet enim fermentum semper. Etiam ultrices mauris non turpis. Nulla facilisi. Donec ultrices commodo justo. Vestibulum sit amet lacus.</p>";
				N2.Context.Persister.Save(item);
				CreateItems(depth-1, count, item, prefix);
			}
		}
	}
}