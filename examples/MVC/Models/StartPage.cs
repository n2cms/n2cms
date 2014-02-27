using N2.Definitions;
using N2.Definitions.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class StartPage : ContentPage, IStartPage
    {
    }

    [Registration]
    public class StartPageRegisterer : FluentRegisterer<StartPage>
    {
        public override void RegisterDefinition(IContentRegistration<StartPage> register)
        {
            register.Page(title: "Start Page");
            register.UsingConventions();
        }
    }
}
