using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Adapters;

namespace N2.Edit.Web.UI.Controls
{
    public class TreeView : System.Web.UI.WebControls.TreeView
    {
        private int _checkboxIndex = 1;

        protected override void Render(HtmlTextWriter writer)
        {
            BuildItems(this.Nodes, true, true, writer);
        }

        private void BuildItems(TreeNodeCollection items, bool isRoot, bool isExpanded, HtmlTextWriter writer)
        {
            if (items.Count > 0)
            {
                writer.WriteLine();

                writer.WriteBeginTag("ul");

                if (isRoot)
                {
                    writer.WriteAttribute("id", this.ClientID);
                }
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent++;

                foreach (System.Web.UI.WebControls.TreeNode item in items)
                {
                    BuildItem(item, writer);
                }

                writer.Indent--;
                writer.WriteLine();
                writer.WriteEndTag("ul");
            }
        }

        private void BuildItem(System.Web.UI.WebControls.TreeNode item, HtmlTextWriter writer)
        {
            TreeView treeView = this;
            if ((treeView != null) && (item != null) && (writer != null))
            {
                writer.WriteLine();
                writer.WriteBeginTag("li");
                if (item.Selected || IsChildNodeSelected(item.ChildNodes))
                    writer.WriteAttribute("class", "open");
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent++;
                writer.WriteLine();
                
                if (!string.IsNullOrEmpty(item.NavigateUrl))
                {
                    WriteNodeLink(treeView, item, writer);
                }
                else
                {
                    WriteNodePlain(treeView, item, writer);
                }

                if (HasChildren(item))
                {
                    BuildItems(item.ChildNodes, false, item.Expanded.Equals(true), writer);
                }

                writer.Indent--;
                writer.WriteLine();
                writer.WriteEndTag("li");
            }
        }

        private void WriteNodeImage(TreeView treeView, System.Web.UI.WebControls.TreeNode item, HtmlTextWriter writer)
        {
            string imgSrc = GetImageSrc(treeView, item);
            if (!string.IsNullOrEmpty(imgSrc))
            {
                writer.WriteBeginTag("img");
                writer.WriteAttribute("src", treeView.ResolveClientUrl(imgSrc));
                writer.WriteAttribute("alt", !string.IsNullOrEmpty(item.ToolTip) ? item.ToolTip : (!string.IsNullOrEmpty(treeView.ToolTip) ? treeView.ToolTip : item.Text));
                writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            }
        }

        private void WriteNodeCheckbox(TreeView treeView, System.Web.UI.WebControls.TreeNode item, HtmlTextWriter writer)
        {
            writer.WriteBeginTag("input");
            writer.WriteAttribute("type", "checkbox");
            writer.WriteAttribute("id", treeView.ClientID + "n" + _checkboxIndex.ToString() + "CheckBox");
            writer.WriteAttribute("name", treeView.UniqueID + "n" + _checkboxIndex.ToString() + "CheckBox");

            if (!string.IsNullOrEmpty(treeView.Attributes["OnClientClickedCheckbox"]))
            {
                writer.WriteAttribute("onclick", treeView.Attributes["OnClientClickedCheckbox"]);
            }

            if (item.Checked)
            {
                writer.WriteAttribute("checked", "checked");
            }
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);

            if (!string.IsNullOrEmpty(item.Text))
            {
                writer.WriteLine();
                writer.WriteBeginTag("label");
                writer.WriteAttribute("for", treeView.ClientID + "n" + _checkboxIndex.ToString() + "CheckBox");
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(item.Text);
                writer.WriteEndTag("label");
            }

            _checkboxIndex++;
        }


        private void WriteNodeLink(TreeView treeView, System.Web.UI.WebControls.TreeNode item, HtmlTextWriter writer)
        {
            writer.WriteBeginTag("a");
            writer.WriteAttribute("href", ToAbsolute(item));
            
            if (item.Selected)
                writer.WriteAttribute("class", "selected");

            if (!string.IsNullOrEmpty(item.Target))
            {
                writer.WriteAttribute("target", item.Target);
            }
            else if (!string.IsNullOrEmpty(this.Target))
            {
                writer.WriteAttribute("target", this.Target);
            }

            if (!string.IsNullOrEmpty(item.ToolTip))
            {
                writer.WriteAttribute("title", item.ToolTip);
            }
            else if (!string.IsNullOrEmpty(treeView.ToolTip))
            {
                writer.WriteAttribute("title", treeView.ToolTip);
            }
            writer.Write(HtmlTextWriter.TagRightChar);
            
            WriteNodeImage(treeView, item, writer);
            writer.Write(item.Text);

            writer.WriteEndTag("a");
        }

        private string ToAbsolute(System.Web.UI.WebControls.TreeNode item)
        {
            if(item.NavigateUrl.StartsWith("~"))
                return VirtualPathUtility.ToAbsolute(item.NavigateUrl);
            else
                return item.NavigateUrl;
        }

        private void WriteNodePlain(TreeView treeView, System.Web.UI.WebControls.TreeNode item, HtmlTextWriter writer)
        {
            writer.WriteBeginTag("span");
            writer.Write(HtmlTextWriter.TagRightChar);
            
            WriteNodeImage(treeView, item, writer);
            writer.Write(item.Text);

            writer.Indent--;
            writer.WriteEndTag("span");
        }

        private string GetImageSrc(TreeView treeView, System.Web.UI.WebControls.TreeNode item)
        {
            string imgSrc = "";

            if ((treeView != null) && (item != null))
            {
                imgSrc = item.ImageUrl;

                if (string.IsNullOrEmpty(imgSrc))
                {
                    if (item.Depth == 0)
                    {
                        if ((treeView.RootNodeStyle != null) && (!string.IsNullOrEmpty(treeView.RootNodeStyle.ImageUrl)))
                        {
                            imgSrc = treeView.RootNodeStyle.ImageUrl;
                        }
                    }
                    else
                    {
                        if (!IsExpandable(item))
                        {
                            if ((treeView.LeafNodeStyle != null) && (!string.IsNullOrEmpty(treeView.LeafNodeStyle.ImageUrl)))
                            {
                                imgSrc = treeView.LeafNodeStyle.ImageUrl;
                            }
                        }
                        else if ((treeView.ParentNodeStyle != null) && (!string.IsNullOrEmpty(treeView.ParentNodeStyle.ImageUrl)))
                        {
                            imgSrc = treeView.ParentNodeStyle.ImageUrl;
                        }
                    }
                }

                if ((string.IsNullOrEmpty(imgSrc)) && (treeView.LevelStyles != null) && (treeView.LevelStyles.Count > item.Depth))
                {
                    if (!string.IsNullOrEmpty(treeView.LevelStyles[item.Depth].ImageUrl))
                    {
                        imgSrc = treeView.LevelStyles[item.Depth].ImageUrl;
                    }
                }
            }

            return imgSrc;
        }

        private bool HasChildren(System.Web.UI.WebControls.TreeNode item)
        {
            return ((item != null) && ((item.ChildNodes != null) && (item.ChildNodes.Count > 0)));
        }

        private bool IsExpandable(System.Web.UI.WebControls.TreeNode item)
        {
            return (HasChildren(item) || ((item != null) && item.PopulateOnDemand));
        }

        private bool IsChildNodeSelected(TreeNodeCollection nodes)
        {
            if (nodes != null)
            {
                foreach (System.Web.UI.WebControls.TreeNode node in nodes)
                {
                    if (node.Selected || IsChildNodeSelected(node.ChildNodes))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
