using N2.Engine;
using N2.Management.Api;
using N2.Plugin;
using N2.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace N2.Management.Api
{
    //[Service]
    //public class ProfileInjector : IAutoStart
    //{
    //  private InterfaceBuilder builder;
    //  public ProfileInjector(InterfaceBuilder builder)
    //  {
    //      this.builder = builder;
    //  }

    //  void builder_InterfaceBuilt(object sender, InterfaceBuiltEventArgs e)
    //  {
            
    //  }

    //  public void Start()
    //  {
    //      builder.InterfaceBuilt += builder_InterfaceBuilt;
    //  }

    //  public void Stop()
    //  {
    //      builder.InterfaceBuilt -= builder_InterfaceBuilt;
    //  }
    //}

    [Service(typeof(IApiHandler))]
    public class ProfileHandler : IHttpHandler, IApiHandler
    {
        private IProfileRepository repository;
        
        public ProfileHandler()
            : this(Context.Current.Resolve<IProfileRepository>())
        {
        }

        public ProfileHandler(IProfileRepository repository)
        {
            this.repository = repository;
        }

        public void ProcessRequest(System.Web.HttpContextBase context)
        {
            context.Response.SetNoCache();

            switch (context.Request.HttpMethod)
            {
                case "GET":
                    switch (context.Request.PathInfo)
                    {
                        case "":
                            var user = GetUser(context.User);
                            context.Response.WriteJson(new { User = user });
                            break;
                    }
                    break;
                case "PUT":
                case "POST":
                    switch (context.Request.PathInfo)
                    {
                        case "":
                            Save(context);
                            break;
                    }
                    break;
                case "PATCH":
                    switch (context.Request.PathInfo)
                    {
                        case "":
                            Patch(context);
                            break;
                    }
                    break;
            }
        }

        private void Save(System.Web.HttpContextBase context)
        {
            var requestBody = context.GetOrDeserializeRequestStreamJsonDictionary<object>();
            var user = GetUser(context.User);
            if (requestBody.ContainsKey("Settings") && requestBody["Settings"] is IDictionary<string, object>)
                user.Settings = (IDictionary<string, object>)requestBody["Settings"];
            repository.Save(user);
            context.Response.WriteJson(new { User = user });
        }

        private void Patch(HttpContextBase context)
        {
            var requestBody = context.GetOrDeserializeRequestStreamJsonDictionary<object>();
            var user = GetUser(context.User);
            if (requestBody.ContainsKey("Settings") && requestBody["Settings"] is IDictionary<string, object>)
                foreach (var kvp in (IDictionary<string, object>)requestBody["Settings"])
                {
                    if (kvp.Value != null)
                        user.Settings[kvp.Key] = kvp.Value;
                    else
                        user.Settings.Remove(kvp.Key);
                }
            repository.Save(user);
            context.Response.WriteJson(new { User = user });
        }

        private ProfileUser GetUser(IPrincipal user)
        {
            return repository.GetOrCreate(user);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(context.GetHttpContextBase());
        }
    }
}
