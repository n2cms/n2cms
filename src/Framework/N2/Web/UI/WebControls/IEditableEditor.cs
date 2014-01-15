using System.Web.UI;

namespace N2.Web.UI.WebControls
{
    public interface IEditableEditor : IItemContainer
    {
        Control Editor { get; }
        string PropertyName { get; }
    }
}
