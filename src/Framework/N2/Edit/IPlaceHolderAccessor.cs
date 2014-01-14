using System.Web.UI;

namespace N2.Edit
{
    public interface IPlaceHolderAccessor
    {
        Control GetPlaceHolder(string name);
    }
}
