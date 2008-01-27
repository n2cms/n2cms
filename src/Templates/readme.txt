The templates project is a somewhat more complete implementation of the N2 CMS
engine.


HOW TO RUN:

* Open src\Templates\N2.Templates-vs2008.sln and hit ctrl+F5


WHAT'S HERE:

UI
Templates and other user interface related files for the templates project. 
The various modules are located in separate projects in directories below the
UI folder.

N2.Templates
A library project used by the templates. Defines a few content items and some 
web controls.

Security
An implementation of the ASP.NET membership providers storing user as content
items.

Survey
A library used by the Poll and Form modules.

Syndication
A library that enables items to be syndicated (RSS). 