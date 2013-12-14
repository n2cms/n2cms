using System;
using N2.Addons.Wiki;
using System.Web.UI.WebControls;

namespace N2.Templates.Tests.Wiki
{
    public class FakeTemplateRenderer : ITemplateRenderer
    {
        public FakeTemplateRenderer()
        {
            Name = "FakeTemplate";
        }

        #region ITemplateRenderer Members

        public string VirtualPath
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IPlugin Members

        public string Name { get; set; }
        public Type Decorates { get; set; }
        public int SortOrder
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAuthorized(System.Security.Principal.IPrincipal user)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<IPlugin> Members

        public int CompareTo(N2.Plugin.IPlugin other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRenderer Members

        public System.Web.UI.Control AddTo(System.Web.UI.Control container, ViewContext context)
        {
            Literal l = new Literal();
            l.Text = "Template:" + Name;
            container.Controls.Add(l);
            return l;
        }

        #endregion
    }
}
