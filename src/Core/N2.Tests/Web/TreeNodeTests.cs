using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using N2.Web.UI.WebControls;
using System.Web.UI;
using System.IO;

namespace N2.Tests.Web
{
	[TestFixture]
	public class TreeNodeTests : ItemTestsBase
	{
		[Test]
		public void CanRender_TreeNode()
		{
			PageItem item = CreateOneItem<PageItem>(1, "name", null);
			TreeNode tn = new TreeNode(item);

			StringBuilder sb = new StringBuilder();
			using (HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb)))
			{
				tn.RenderControl(writer);
			}

			Assert.AreEqual("<ul><li><a href=\"/name.aspx\">name</a></li></ul>", sb.ToString());
		}

		[Test]
		public void CanRender_TreeNode_WithChildNode()
		{
			PageItem root = CreateOneItem<PageItem>(1, "root", null);
			PageItem item = CreateOneItem<PageItem>(1, "item", root);
			TreeNode tn = new TreeNode(root);
			tn.Controls.Add(new TreeNode(item));

			StringBuilder sb = new StringBuilder();
			using (HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb)))
			{
				tn.RenderControl(writer);
			}

			Assert.AreEqual("<ul><li><a href=\"/root.aspx\">root</a><ul><li><a href=\"/item.aspx\">item</a></li></ul></li></ul>", sb.ToString());
		}

		[Test]
		public void CanRender_ChildNode_WithoutRendereringParent()
		{
			PageItem root = CreateOneItem<PageItem>(1, "root", null);
			PageItem item = CreateOneItem<PageItem>(1, "item", root);
			TreeNode tn = new TreeNode(root);
			tn.Controls.Add(new TreeNode(item));
			tn.ChildrenOnly = true;

			StringBuilder sb = new StringBuilder();
			using (HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb)))
			{
				tn.RenderControl(writer);
			}

			Assert.AreEqual("<li><a href=\"/item.aspx\">item</a></li>", sb.ToString());
			//<li><a href="/item.aspx">item</a></li>]]!=
			//<li><a href="/item.aspx">item</a></li><ul><li><a href="/root.aspx">root</a><ul><li><span>item</span></li></ul></li></ul>
		}
	}
}
