using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using N2.Edit.Web;
using N2.Web.UI.WebControls;

namespace N2.Edit
{
	public partial class Toolbar : EditUserControl
	{
        #region Get Resource Methods
        protected string GetLocalResourceString(string resourceKey)
        {
            return (string)GetLocalResourceObject(resourceKey);
        }
        protected string GetGlobalResourceString(string className, string resourceKey)
        {
            return (string)GetGlobalResourceObject(className, resourceKey);
        }

        #endregion
	}
}