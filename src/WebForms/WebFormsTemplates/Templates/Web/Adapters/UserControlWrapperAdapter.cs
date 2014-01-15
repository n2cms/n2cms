using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls.Adapters;
using System.Web.UI;
using System.Web.UI.Adapters;
using N2.Web.UI;

namespace N2.Templates.Web.Adapters
{
    public class UserControlWrapperAdapter : ControlAdapter
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            throw new Exception("init");
        }

        public UserControl UCControl
        {
            get { return base.Control as UserControl; }
        }

        public IItemContainer ItemContainer
        {
            get { return base.Control as IItemContainer; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string typeName = ItemContainer.CurrentItem.GetContentType().Name;

            writer.Write("<div class='uc ");
            writer.Write(typeName[0] + typeName.Substring(1));
            writer.Write("'><div class='inner'>");
            base.Render(writer);
            writer.Write("</div></div>");
        }
    }
}
