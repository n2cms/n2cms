
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Web;
using N2.Edit;
using N2.Edit.LinkTracker;

namespace N2.Management.Content.LinkTracker
{
    /// <summary>
    /// Summary description for UpdateReferencesTo
    /// </summary>
    public class UpdateReferencesTo : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var engine = N2.Context.Current;
            var selection = new SelectionUtility(context, engine);
            engine.Resolve<Tracker>().UpdateReferencesTo(selection.SelectedItem);

            context.Response.ContentType = "application/json";
            context.Response.Write(new { success = true }.ToJson());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
