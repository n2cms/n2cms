using System.Security.Principal;
using System.Web.UI;
using N2.Definitions;

namespace N2.Edit
{
    public class ToolbarOption : IContainable
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public bool Selected { get; set; }


        #region IContainable Members

        public virtual Control AddTo(Control container)
        {
            var link = new LiteralControl(
                    string.Format("<a href='{0}' data-url-template='{0}' target='{1}' class='option templatedurl {2}'>{3}</a>", N2.Web.Url.ResolveTokens(Url), Target, Selected ? "selected" : "", Title)
                );
            container.Controls.Add(link);
            return link;
        }

        public string ContainerName { get; set; }

        public int SortOrder { get; set; }

        public bool IsAuthorized(IPrincipal user)
        {
            return true;
        }

        #endregion

        #region IUniquelyNamed Members

        public string Name { get; set; }

        #endregion

        #region IComparable<IContainable> Members

        public int CompareTo(IContainable other)
        {
            return SortOrder - other.SortOrder;
        }

        #endregion
    }
}
