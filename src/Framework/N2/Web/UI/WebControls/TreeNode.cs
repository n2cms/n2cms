using System.Web.UI;
using System;

namespace N2.Web.UI.WebControls
{
    [Obsolete("Not used any more")]
    public class TreeNode : Control
    {
        ContentItem node;
        string liClass;
        string aClass;
        string ulClass;
        Control linkControl;
        bool childrenOnly;

        public bool ChildrenOnly
        {
            get { return childrenOnly; }
            set { childrenOnly = value; }
        }

        public Control LinkControl
        {
            get { return linkControl; }
            set { linkControl = value; }
        }

        public string AClass
        {
            get { return aClass; }
            set { aClass = value; }
        }

        public string LiClass
        {
            get { return liClass; }
            set { liClass = value; }
        }

        public string UlClass
        {
            get { return ulClass; }
            set { ulClass = value; }
        }

        public ContentItem Node
        {
            get { return node; }
            set { node = value; }
        }

        public TreeNode()
        {
        }
        public TreeNode(ContentItem node)
            : this(node, Link.To(node).ToControl())
        {
        }
        public TreeNode(ContentItem node, Control link)
        {
            this.node = node;
            linkControl = link;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            bool isRoot = Parent is TreeNode == false;

            if (ChildrenOnly)
            {
                base.RenderChildren(writer);
            }
            else
            {
                RenderBeginTag(writer, isRoot);
                RenderChildren(writer);
                RenderEndTag(writer, isRoot);
            }
        }

        protected override void RenderChildren(HtmlTextWriter writer)
        {
            if (Controls.Count > 0)
            {
                if (string.IsNullOrEmpty(UlClass))
                    writer.Write("<ul>");
                else
                    writer.Write("<ul class=\"{0}\">", UlClass);

                base.RenderChildren(writer);

                writer.Write("</ul>");
            }
        }

        protected void RenderBeginTag(HtmlTextWriter writer, bool isRoot)
        {
            if (isRoot)
                writer.Write("<ul>");

            if (string.IsNullOrEmpty(LiClass))
                writer.Write("<li>");
            else
                writer.Write("<li class=\"{0}\">", LiClass);

            linkControl.RenderControl(writer);
        }

        protected void RenderEndTag(HtmlTextWriter writer, bool isRoot)
        {
            writer.Write("</li>");
            if (isRoot)
                writer.Write("</ul>");
        }
    }
}
