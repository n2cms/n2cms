## Welcome to N2CMS

**N2CMS is a lightweight CMS framework.** With just a few strokes of your keyboard a 
wonderful strongly-typed model emerges complete with a management UI. You can 
spend the rest of your day building the best site imaginable.

**It's so .NET!** With N2CMS, you build the model of the data that needs to be managed using C# or 
VB code in Visual Studio. The type below is picked up by the N2 engine at runtime 
and made available to be edited. The code uses an open API with multiple built-in 
options and unlimited customization options.

*All you have to do is design your model class (inherit N2.ContentItem) and define 
which properties are editable by adding attributes*

```csharp
[PageDefinition(TemplateUrl = "~/my/pageitemtemplate.aspx")]
public class PageItem : N2.ContentItem
{
    [EditableFreeTextArea]
    public virtual string Text { get; set; }
}
```

### Quick Start

**For a quick start**, follow these instructions, which assume that you are using ASP.NET MVC + Razor (the "Dinamico" template pack -- see below for details). 

1. Create a new, empty Web Application Project in Visual Studio 2012 or 2013. 
2. Go to *Tools > Library Package Manager > Package Manager Console*
3. In the Package Manager console run the following commands: 
  ```
Install-Package N2CMS.Dinamico
Install-Package N2CMS.Management
```

Please note, N2CMS supports the following ASP.NET view engines:
* ASP.NET MVC + Razor ("Dinamico" template pack)
* ASP.NET MVC + MVC Views ("MVC" template pack)
* ASP.NET Web Forms


Detailed installation instructions are available at: https://github.com/n2cms/n2cms/blob/master/README_SETUP.md
or in our documentation wiki: https://n2cmsdocs.atlassian.net/wiki/display/N2CMS/Getting+Started+using+N2CMS


----

##API

You can use the API within your methods and properties to develop advanced content manageable features.

```csharp
public void DoSomeStuffWithSomeItems(DateTime minDate, DateTime maxDate)
{
	IList<ContentItem> items = N2.Find.Items
		.Where.Created.Between(minDate, maxDate)
		.And.SavedBy.Eq("admin")
		.OrderBy.Published.Desc
		.Select();

	foreach (ContentItem item in items)
		DoSomethingWith(item);
}
```
There are more API usage examples here: http://n2cms.com/Documentation/Manipulating%20content/Finding%20content.aspx.

##I want this in my project.  Where do I download it?

**Install the Nuget package: http://www.nuget.org/packages/N2CMS/**

##Where do I get more advanced documentation?

###Reference Documentation
**On our Confluence wiki:** https://n2cmsdocs.atlassian.net/wiki/display/N2CMS/Getting+Started+using+N2CMS

>We know... we need to move this to github wiki or our public site.  We are the cobblers kids.  Would love help if you are interested.  Contact us.


## Screenshots

### Management Console 

![Management Console](https://pbs.twimg.com/media/BPziGS2CYAAqg7S.png:large)

### Page or Part Edit

![Page / Part Edit](http://content.screencast.com/users/brianmatic/folders/Jing/media/b9c58f64-853e-4484-8dc1-317eeb2fe80b/00000003.png)


## Examples

**We currently post them on CodePlex: http://n2cms.codeplex.com/releases/**.  You can also find them in the source code within this repo.


You may have to make certain configuration changes when moving the code to a 
hosting provider. Common issues are addressed here:
http://n2cms.com/wiki/Troubleshooting-site-deployment.aspx

##Clone the Source Code and Contribute to N2CMS

### What is here?

Here you will find the N2 CMS framework and a number of template projects that 
demonstrate alternative ways to use this CMS. They all share a framework that 
consists of N2.dll and the UI management files residing below the /N2/ folder.
 
### How do I setup my development environment?

1. Clone this repo to your PC.
2. Double-click on Prepare_AllDependencies.bat
3. Choose amount of templates (ranging from minimal example to many features in the box):
	* Examples - Minimal C#
	* Examples - Minimal Visual Basic
	* Examples - Minimal MVC
	* Src - Dinamico
	* Src - MVC Templates
	* Src - WebForm Templates
4. Choose between N2.Everything.sln to open everything, or venture down the 
   directory structure of Src or Examples and open a solution down there.
5. Find the Visual Studio solution explorer find the web project you chose (2.), 
   right-click on it and select "Set as StartUp Project".
6. Set the a web site project as startup project (N2.Templates.* or Dinamico in src)
7. Compile and run (Ctrl+F5)

N2 CMS supports many databases, this code is set up to use the SQLite embedded 
database. You may want to use SQL Server or MySQL in production.

##More Resources and Documentation

* http://n2cms.codeplex.com/releases/ (Download N2 CMS 2.x Developer Documentation)
* http://n2cms.com/Documentation.aspx
* http://google.com
* http://n2cms.codeplex.com/Thread/List.aspx
* http://n2cms.com/wiki/Project-life-cycle.aspx
* http://stackoverflow.com/questions/tagged/n2cms


## Feedback

You are very welcome to let us know about your build experiences in the issues 
so we can continue to improve things. Pull requests are also welcomed. 



## Frequently Asked Questions

*What does the N2 in N2CMS stand for?*
It is short for "en tva" (1-2 in swedish).

*I want to create an open source project based on N2CMS.  I want N2CMS to be 
part of branding my project. Do you have guidance on naming my project?*

Yes, please use the entire project name "N2CMS" in your project name.  For 
example, N2CMS.BootstrapBlog would be great project name. We feel that just 
using "N2" is too generic.  Using N2CMS will help with organic search results.

