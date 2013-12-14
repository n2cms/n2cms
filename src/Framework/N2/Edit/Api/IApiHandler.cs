using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace N2.Management.Api
{
    public interface IApiHandler
    {
        void ProcessRequest(HttpContextBase context);
    }
}
