using N2;
using N2.Definitions;
using N2.Definitions.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class ContentPage : ContentItem, IContentPage
    {
        public virtual string Text { get; set; }
    }

    [Registration]
    public class ContentPageRegistration : FluentRegisterer<ContentPage>
    {
        public override void RegisterDefinition(IContentRegistration<ContentPage> register)
        {
            register.Page(title: "Content Page");
            register.UsingConventions();
        }
    }

}
