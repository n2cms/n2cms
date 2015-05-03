===================
Server Requirements
===================

Operating System
================
 
N2CMS runs on .NET Framework 4.5, which runs on the following operating systems.

*Server operating systems*

======================== ================================ ==================================
Operating system         Supported .NET Framework Version Additional information  
======================== ================================ ==================================
Win Server 2012 R2       4.5                              Includes .NET Framework 4.5.1   
======================== ================================ ==================================
Win Server 2012 (64-bit) 4.5                              Includes .NET Framework 4.5
======================== ================================ ==================================


Client operating systems
========================

N2CMS supports Visual Studio 2010, 2012, and 2013 on the following operating systems:
 
Web Servers
===========

N2CMS has been tested successfully on the Visual Studio embedded web server, IIS, and IIS Express. 

.NET Framework
==============

N2CMS 1.5 can run on .NET 2.0. We recommend that you run N2CMS on the latest version of the .NET framework possible. 

Databases
=========

Supported databases include:
SQL Server 2008 *
SQL Server 2008 Express *
SQL Server 2005 *
SQL Server 2005 Express *
SQL Server 2000
SqlCe
MySQL *
SQLite *
Firebird
Jet
DB2
Oracle9i
Oracle10g
 
* A connection string example for these database engines can be found in web.config. 

Wildcard mapping (IIS 5-6.1)
============================

On IIS 5 to IIS 6.1 wildcard mapping can be enabled to support “extensionless urls” (no ending .aspx on each page address). Wilcard mapping is an IIS feature that enables requests to all types of resources to pass through managed ASP.NET code. This allows N2 CMS to serve pages for pages not ending with .aspx. This is configured slightly differently depending on IIS version. Try searching and pick the version you're using: http://www.google.com/search?q=iis+wildcard+mapping

Visual Studio Development
=========================

    * Visual Studio 2012 is recommended for site development with N2CMS.
    * Visual Studio Express can also be used.
    
Visual Studio 2012 project files are included in the N2CMS source tree.

Shared Hosting
==============

Some users report no problems running in shared hosting under medium trust, others have had problems. N2CMS has been tested on unmodified medium trust. It’s recommended you ask the hosting provider before you buy a long-term plan. Note that N2CMS can be somewhat RAM intensive, and requires a minimum of 64 MB of RAM dedicated to your website. More RAM may be needed depending on the plugins you require or the amount of traffic your website receives. Plug-ins such as site search can also increase the memory requirement.