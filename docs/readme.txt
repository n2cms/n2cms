N2 CMS

This is a lightweight CMS framework that focuses on ease of use and developer experience. Web content editors will enjoy a simple and powerful web interface while developers can benefit from a discoverable and open-ended API. 

More information and documentation at http://n2cms.com


ABOUT THIS PACKAGE:

This is the source code of N2 CMS and a development web containing some test pages and the 
edit interface.

To try out N2 quickly download the examples. No database is included in this package but 
you can either use one of those in the example release or setup an empty database (below).


INSTALLATION:

* Compile and run the application
* Browse to /edit/install (use admin/admin to log in) and follow the instructions on the installation page (also below)
	* Create an empty database in your server of choice. 
	* Update the connection string in web.config
	* If you can't use the web interface for installation you can find create database scripts  below the folder N2.Installation\SqlScripts below in the N2 solution
	* Now you need to insert the first node (the root node). There is an option on the installation page to do this as well as some final configuration options
* IMPORTANT! Change the default password in web.config. If you've installed, configured and created an administrator account using a membership provider, comment out this section entirely. 


EXAMPLES:

There are a bunch of examples in the Examples src/Examples directory:
* SimpleWebSite: Very simple web sites in C# and Visual Basic (understand these before you go on)
* Globalization: Exemplifies an approach at globalizing content within the same content tree
* MediumTrust: How to run N2 in a medium trust environment (many shared hosting providers)
* Parts: Bare minimum project using "parts", i.e. drag'n'drop components on a page


TEMPLATES

There is a proposed implementation project called simply templates. It's located in the src/Templates directory.


CONTRIBUTORS:

* Cristian Libardo
* Sten Johanneson
* Michele Scaramal