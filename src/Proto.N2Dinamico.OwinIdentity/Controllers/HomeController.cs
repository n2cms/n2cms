using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
 
using N2;
using N2.Persistence;
using N2.Security;
using N2.Security.Items;
using N2.Security.AspNet.Identity;
using Proto.OwinIdentity.Models;

namespace Proto.OwinIdentity.Controllers
{
    public class HomeController : Controller
    { 
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string Echo(string id)
        {
            return string.Format("ECHO: {0}",id);
        }

        public string Test()
        {
            try
            {
                var engine = N2.Content.Current.Engine;
                var bridge = engine.Resolve<ItemBridge>();

                // var u1 = bridge.CreateUser("a1", "p1", "a1@x.y", "", "", false, null);
                // var u2 = bridge.CreateUser("a2", "p2", "a2@x.y", "", "", false, null);

                var item = engine.Persister.Get(194);

                var userList = bridge.GetUserContainer(true);
                var users = new List<ContentItem>(userList.GetChildren());
                foreach (var user in users)
                {
                    var subs = new List<ContentItem>(user.GetChildren());
                    foreach(var sub in subs)
                      bridge.Delete(sub);
                }

                foreach (var user in users)
                    bridge.Delete(user);

                bridge.GetUser("xx");
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}