===========================
Setting up your environment
===========================

Development Environment
=======================

There are two supported environments that you can use for developing a site with N2:
    1. Git-based environment
    2. Nuget-based environment

======================= =========================================================================================
Choose this              If you want this
======================= =========================================================================================
Nuget-based environment  You want the easiest integration of N2 in an exisitng project. The Nuget framework will 
                         download and install all required dependencies into an existing Visual Studio project.
======================= =========================================================================================
Git-based environment    You always want to have the most recent code, or you want to use one of the sample 
                         template projects. The Git-based environment contains both the WebForms and MVC 
                         template packs, sample data, plus the N2 Core and all dependency binaries. This is a 
                         great way to get started with N2CMS, particularly if you don't already have a project 
                         started. You can use one of the existing projects as a basis for your new N2-based site:
                            * WebForms Templates Pack
                            * ASP.NET MVC Templates Pack
                            * ASP.NET MVC "Dinamico" Templates Pack (uses the Razor engine)
======================= =========================================================================================

Getting the Bits
================

Each of the supplied packages is supplied either as a Git repository or as a Nuget package.

    * Start with Git: check out the N2 repository located at http://github.com/n2cms/n2cms. Note: We don't recommend using Github's 
      archive formats (tgz, zip). Using the archive formats will make your installation more difficult to update as Git's patching 
      infrastructure will not be available.
    * Start with Nuget: install the requisite Nuget package from within your Visual Studio project:     
            - n2cms_webforms
            - n2cms_mvc
            - n2cms_dinamico
            
Overview of Available Packages
==============================

Package
Description 
N2 CMS 2.x Source Code
This package reflects the N2 CMS framework development environment and contains both template packs and all examples along with the framework source code. For site development it’s recommended to start from one of the template packs, or examples.
N2 CMS 2.x ASP.NET MVC Templates Pack
This is the source code of the MVC template package along with a compiled version of the framework. Use this package to develop your own site with existing functionality using ASP.NET MVC.
N2 CMS 2.x ASP.NET WebForms Templates Pack
This is the source code of the WebForms  template package along with a compiled version of the framework. Use this package to develop your own site with existing functionality using ASP.NET WebForms.
N2 CMS 2.x MVC Minimal Example
This package contains a simple example site along with a compiled version of the framework. Use this package to understand the basics of ASP.NET MVC  + N2 CMS or if you don’t need existing templates.
N2 CMS 2.x C# Minimal Example
This package contains a simple example site along with a compiled version of the framework. Use this package to understand the basics of WebForms/C# + N2 CMS or if you don’t need existing templates.
N2 CMS 2.x Visual Basic Minimal Example
This package contains a simple example site along with a compiled version of the framework. Use this package to understand the basics of WebForms/Visual Basic + N2 CMS or if you don’t need existing templates.
N2 CMS 2.x Compiled Framework and Management UI
This is the N2 CMS framework compiled and zipped for upgrade of a previous version, or integration with an existing site.
Getting up and Running
Each package contains one or more Visual Studio Solution (*.sln) files. Open the Solution file for what you want to run, and edit the web.config file to use the database engine of your choosing. Invoke the Run command in Visual Studio to launch a development web server with N2 running inside. You should see the setup wizard right away.
The next step is to either start using N2 and begin building your content using the pre-defined templates, or start developing with N2 to customize the system to suit your needs. Most users will want to do at least some development and customization as every website has different needs and the templates don't cover every possible scenario that can be achieved with N2 CMS.
Web Platform Installer (WPI)
The same WPI package can be installed from the “Microsfot Web Platform Installer”, from “Internet Information Services (IIS) Manager” or from “Microsoft WebMatrix”.
Updating N2CMS
After you've installed N2CMS in one of these ways, you will want to update it from time to time to take advantage of the latest features, security patches, and other updates. If you opted to use the Nuget deployment model, the Nuget packages will be updated periodically, and you can update using the built-in Nuget update mechanism to update your local instance of N2CMS. If you chose the Git deployment model, you can use git pull to get the latest updates in your own Git repository.