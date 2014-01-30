using N2;
using N2.Definitions;
using N2.Definitions.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSharp.Models
{
    [PageDefinition("Start Page", TemplateUrl = "~/Views/ContentView.aspx")]
    public class StartPage : ContentPage, IStartPage
    {
    }
}
