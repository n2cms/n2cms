===========================
Setting up your environment
===========================

Development Environment
=======================

There are two supported environments that you can use for developing a site with N2:
    1. Git-based environment
    2. Nuget-based environment

+------------------------+----------------------------------------------------------------------------------+
|| Choose this           || If you want this                                                                |
+========================+==================================================================================+
|| Nuget-based           || You want the easiest integration of N2 in an existing project.The Nuget         | 
|| environment           || framework will downloadand install all required dependenciesinto an existing    |
||                       || Visual Studio project.                                                          |
+------------------------+----------------------------------------------------------------------------------+
|| Git-based             || You always want to have the most recent code, or you want to use one of the     |
|| environment           || sample template projects. The Git-based environment contains both the           |      
||                       || WebForms and MVC template packs, sample data, plus the N2 Core and all          |
||                       || dependency binaries.This is a great way to get started with N2CMS, particularly |
||                       || if you don't already have a project started. You can use one of the existing    |
||                       || projects as a basis for your new N2-based site:                                 |
||                       ||                                                                                 |
||                       ||   * WebForms Templates Pack                                                     |
||                       ||   * ASP.NET MVC Templates                                                       |
||                       ||   * ASP.NET MVC "Dinamico" Templates Pack (uses the Razor engine)               |  
+------------------------+----------------------------------------------------------------------------------+


Getting the Bits
================

Each of the supplied packages is supplied either as a Git repository or as a Nuget package.

    * Start with Git: check out the N2 repository located at http://github.com/n2cms/n2cms. Note: We don't recommend using Github's 
      archive formats (tgz, zip). Using the archive formats will make your installation more difficult to update as Git's patching 
      infrastructure will not be available.
    * Start with Nuget: install the requisite Nuget package from within your Visual Studio project
        - n2cms_webforms
        - n2cms_mvc
        - n2cms_dinamico
            

Overview of Available Packages
==============================

+----------------------+----------------------------------------------------------------------------+
|| Package             || Description                                                               |
+======================+============================================================================+
|| N2 CMS 2.x          || This package reflects the N2 CMS framework development environment and    |
|| Source Code         || contains both template packs and all examples along with the framework    |
||                     || source code. For site development it’s recommended to start from one of   |
||                     || the template packs, or examples.                                          |
+----------------------+----------------------------------------------------------------------------+
|| N2 CMS 2.x          || This is the source code of the MVC template package along with a          |
|| ASP.NET             || compiled version of the framework. Use this package to develop your own   |
|| MVC Templates Pack  || site with existing functionality using ASP.NET MVC.                       |
+----------------------+----------------------------------------------------------------------------+
|| N2 CMS 2.x ASP.NET  || This is the source code of the WebForms template package along with       |
|| WebForms            || a compiled version of the framework. Use this package to develop          |
|| Templates Pack      || your own site with existing functionality using ASP.NET WebForms.         |
+----------------------+----------------------------------------------------------------------------+
|| N2 CMS 2.x MVC      || This package contains a simple example site along with a compiled version |
|| Minimal Example     || of the framework. Use this package to understand the basics of            |
||                     || ASP.NET MVC + N2 CMS or if you don’t need existing templates.             |
+----------------------+----------------------------------------------------------------------------+
|| N2 CMS 2.x C#       || This package contains a simple example site along with a compiled version |
|| Minimal Example     || of the framework. Use this package to understand the basics of            |
||                     || WebForms/C# + N2 CMS or if you don’t need existing templates.             |     
+----------------------+----------------------------------------------------------------------------+
|| N2 CMS 2.x          || This package contains a simple example site along with a compiled version |
|| Visual Basic        || of the framework. Use this package to understand the basics of            |
|| Minimal Example     || WebForms/Visual Basic + N2 CMS or if you don’t need existing templates.   |
+----------------------+----------------------------------------------------------------------------+
|| N2 CMS 2.x          || This is the N2 CMS framework compiled and zipped for upgrade              |
|| Compiled Framework  || of a previous version, or integration with an existing site.              |
|| and Management UI   ||                                                                           |
+----------------------+----------------------------------------------------------------------------+


Getting up and Running
======================

Each package contains one or more Visual Studio Solution (*.sln) files. Open the Solution file for what you 
want to run, and edit the web.config file to use the database engine of your choosing. Invoke the Run command 
in Visual Studio to launch a development web server with N2 running inside. You should see the setup wizard right away.

The next step is to either start using N2 and begin building your content using the pre-defined templates, or 
start developing with N2 to customize the system to suit your needs. Most users will want to do at least some 
development and customization as every website has different needs and the templates don't cover every possible 
scenario that can be achieved with N2 CMS.

Web Platform Installer (WPI)
----------------------------

The same WPI package can be installed from the “Microsfot Web Platform Installer”, from “Internet Information 
Services (IIS) Manager” or from “Microsoft WebMatrix”.

Updating N2CMS
--------------

After you've installed N2CMS in one of these ways, you will want to update it from time to time to take advantage 
of the latest features, security patches, and other updates. If you opted to use the Nuget deployment model, the 
Nuget packages will be updated periodically, and you can update using the built-in Nuget update mechanism to update 
your local instance of N2CMS. If you chose the Git deployment model, you can use git pull to get the latest updates 
in your own Git repository.

Visual Studio Snippets and Templates
====================================

The downloadable packages on Codeplex contains a number of templates and snippets which are useful 
when developing N2 sites.

	You need to update the path with your own version of Visual Studio. For example, if you are using Visual Studio 2008 
	you need to substitute in 2008 for 2010 in the paths below.

Snippets are copied to [Documents]\Visual Studio 2010\Code Snippets\Visual C#\My Code Snippets. Once the 
snippets have been placed here they can be invoked from Visual Studio by their name and tapping tab twice 
(e.g. n2propfta [tab] [tab]). This will expand a property with an editable attribute. Available snippets include:

	* n2prop.snippet
	* n2propcb.snippet
	* n2propenum.snippet
	* n2propfta.snippet
	* n2propimage.snippet
	* n2proptb.snippet
	* n2propuc.snippet
	* n2propurl.snippet

Installing Visual Studio Item Templates
---------------------------------------

The snippets folder also contains some Visual Studio Item Templates that appears when adding new items in Visual Studio. 
Copy them from the Snippets folder in the template package zip to 
[Documents]\Visual Studio 2010\Templates\ItemTemplates\Visual C#. The item templates creates a content class and a 
corresponding template or controller. Available templates:

	* N2 Item Template.zip
	* N2 Page Template.zip
	* N2 Page Controller.zip

Installing IntelliSense Documentation for Visual Studio
-------------------------------------------------------

The IntelliSense documentation should be installed automatically. If it is not installed, check for ~/bin/N2.xml. 
This enables code documentation during IntelliSense operations and hovering in Visual Studio. 

Installing N2CMS NuGet Packages
===============================

First, download the release and extract to a path such as C:\\N2Packages (this example path is used below)

	* Compiled N2CMS Releases are available here https://github.com/n2cms/n2cms/releases

Next, decide whether you want to use the ZIP or NOZIP management packs.

	| *Before doing anything listed on this page* 
	| Back up your project before installing or uninstalling any N2CMS nuget packages.
	| Realize that N2CMS will not work on Web Site projects. You must install N2CMS in a Web Application project.

Which management pack should I choose?
--------------------------------------

You can choose whichever management pack is right for you. Consider the following benefits and drawbacks.

	* | The benefit of the ZIP management pack is that it is a single file for the N2 Management 
	  | Interface. However, if you store your website in a source control depot (e.g. Git repo), 
	  | you end up with a lot of bloat as the ~5 MB N2.zip file gets upgraded over time. You also 
	  | need to upload the entire N2.zip when you update it.
	  |

	* | The benefit of the NOZIP management pack is that you can take advantage of Web 
	  | Deploy incremental uploads, as well as more efficient source control storage as the 
	  | files are installed separately (not extracted). Additionally, the Zip Virtual Path 
	  | Provider is not installed, which means that less memory is used by N2CMS.

