# N2CMS
#### The best Content Management System (CMS) for custom ASP.NET MVC and WebForm applications

##Give your content managers this user experience
Sharing control of your web application's content makes the site better. 
The site will grow naturally and you won't have to push builds nearly as often.

**Main**

![Management Console](https://pbs.twimg.com/media/BPziGS2CYAAqg7S.png:large)

**Page or Part Edit**

![Page / Part Edit](http://content.screencast.com/users/brianmatic/folders/Jing/media/b9c58f64-853e-4484-8dc1-317eeb2fe80b/00000003.png)

## How do I integrate it in my code?

###MVC Projects

**Model**

```csharp
[PageDefinition("My Page")]
public class MyPage : N2.ContentItem
{
	[EditableFreeTextArea]
	public virtual string Text { get; set; }
}
```

**View**

```html
@model MyPage

<html>
	<head>
		<title>
			@model.Title
		</title>
	</head>
	<body>
		<h1>@model.Title</h1>
		
		<p>This CMS makes it so easy to publish @model.Text!</p>
	</body>
</html>
```

**Controller**

```c#
using N2.Web;


/// <summary>
/// This controller will handle pages deriving from AbstractPage which are not 
/// defined by another controller [Controls(typeof(MyPage))]. The default 
/// behavior is to render a template with this pattern:
///  * "~/Views/SharedPages/{ContentTypeName}.aspx"
/// </summary>
[Controls(typeof(MyPage))]
public class MyPage : N2.ContentController<MyPage>
{
	
}
```

###WebForm Projects

**Class**

```csharp
[PageDefinition("My Page", TemplateUrl = "~/MyPage.aspx")]
[AvailableZones("My zone for N2 Parts", "MyZone")]
public class MyPage : N2.ContentItem
{
	[EditableFreeTextArea]
	public virtual string Text { get; set; }
}

```

**Page**

```html
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyPage.aspx.cs" Inherits="App.UI.Page" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
	<!-- master page and theme is defined in web.config -->

	<!-- The droppable zone enables drop of parts onto the tempalate when in drag&drop mode -->        
    <div class="zone">
		<n2:DroppableZone ID="DroppableZone1" runat="server" ZoneName="MyZone"/>
	</div>

    <asp:SiteMapPath ID="Path" runat="server" CssClass="breadcrumb" />

    <!-- The display control uses the default presentation for an item's property, the title in this case uses header 1 -->
    <n2:Display ID="TitleDisplay" PropertyName="Title" runat="server" />
    <div>
        <!-- This is a way to inject data into a webforms control, in this case we're injecting the current page's text property -->
        <p>N2CMS makes it so easy to publish <asp:Literal ID="TextLiteral" Text="<%$ CurrentPage: Text %>" runat="server" />.</p>
    </div>
</asp:Content>
```

**Here are some other interesting WebControls**

Binds a control to the current page's text property: 
```html
<asp:Literal Text="<%$ CurrentItem: Text %>" runat="server" />
```

Provides create, read, update, delete access to content through ASP.NET the databinding API:
```html
<n2:ItemDataSource ID="Level1Items" runat="server" Path="/" />
<asp:DataGrid DataSourceID="Level1Items" runat="server" />
```

Renders non-page items added to the "RightColumn" zone:

```html
<n2:Zone ZoneName="MyZone" runat="server" />
```

Outputs content using the default control (a literal in this case):
```html
<n2:Display PropertyName="Text" runat="server" />
```

###API

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
**On our Confluence wiki:** https://n2cmsdocs.atlassian.net/wiki/display/N2CMS/Getting+Started

>We know... we need to move this to github wiki or our public site.  We are the cobblers kids.  Would love help if you are interested.  Contact us.

###Examples

**We currently post them on CodePlex: http://n2cms.codeplex.com/releases/**.  You can also find them in the source code within this repo.

>**We recommend starting with the minimal examples if you are new to N2CMS.  This release has easy to download minimal examples: http://n2cms.codeplex.com/releases/view/70951.**

>You may have to make certain configuration changes when moving the code to a 
>hosting provider. Common issues are addressed here:
>http://n2cms.com/wiki/Troubleshooting-site-deployment.aspx

##Clone the Source Code and Contribute to N2CMS

##What is here?

Here you will find the N2 CMS framework and a number of template projects that 
demonstrate alternative ways to use this CMS. They all share a framework that 
consists of N2.dll and the UI management files residing below the /N2/ folder.

##How do I setup my development environment?

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


> Heads up
> N2 CMS supports many databases, this code is set up to use the SQLite embedded 
database. You may want to use SQL Server or MySQL in production.

##More Resources and Documentation

* http://n2cms.codeplex.com/releases/ (Download N2 CMS 2.x Developer Documentation)
* http://n2cms.com/Documentation.aspx
* http://google.com
* http://n2cms.codeplex.com/Thread/List.aspx
* http://n2cms.com/wiki/Project-life-cycle.aspx
* http://stackoverflow.com/questions/tagged/n2cms


##Feedback

You are very welcome to let me know about your build experiences in the 
issues so I can improve things.

##Frequently Asked Questions

####What does the N2 in N2CMS stand for?
> It is short for "en tva" (1-2 in swedish).

####I want to create an open source project based on N2CMS.  I want N2CMS to be part of branding my project. Do you have guidance on naming my project?
> Yes, please use the entire project name "N2CMS" in your project name.  For example, N2CMS.BootstrapBlog would be great project name.
> We feel that just using "N2" is too generic.  Using N2CMS will help with organic search results.
