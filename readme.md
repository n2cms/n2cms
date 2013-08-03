# N2CMS
### A Content Management System (CMS) for custom ASP.NET MVC and WebForm applications.

## How do you integrate it?

###MVC Example

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

```

###WebForm Example

Class

```csharp
[PageDefinition("My Page", TemplateUrl = "~/MyPage.aspx")]
public class MyPage : N2.ContentItem
{
	[EditableFreeTextArea]
	public virtual string Text { get; set; }
}

```



##Source Code

Here you will find the N2 CMS framework and a number of template projects that 
demonstrate alternative ways to use this CMS. They all share a framework that 
consists of N2.dll and the UI management files residing below the /N2/ folder.



##How to setup your development environment

1. Clone this repo.
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

##Resources and Documentation

* http://n2cms.codeplex.com/releases/ (Download N2 CMS 2.x Developer Documentation)
* http://n2cms.com/Documentation.aspx
* http://google.com
* http://n2cms.codeplex.com/Thread/List.aspx
* http://n2cms.com/wiki/Project-life-cycle.aspx



##Helpful tips

You may have to make certain configuration changes when moving the code to a 
hosting provider. Common issues are addressed here:
* http://n2cms.com/wiki/Troubleshooting-site-deployment.aspx







FEEDBACK

You are very welcome to let me know about your build experiences in the 
forum so I can improve things.
