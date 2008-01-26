N2 CMS

N2 is a lightweight CMS framework that focuses on ease of use and developer experience. Web content editors will enjoy a simple and powerful web interface while developers can benefit from a discoverable and open-ended API. 

More information at http://n2cms.com


ABOUT THIS PACKAGE:

This is the source code of N2 CMS and a development web containing some test pages and the 
edit interface.

To try out N2 quickly download the examples. No database is included in this package but 
you can either use one of those in the example release or setup an empty database (below).


INSTALLATION:

	* Compile and run the application
	* Browse to /edit/install (use admin/admin to log in) and follow the instructions on the installation page (also below)
		* Create an empty database in your server of choice. 
		* Update the connection string in web.config, look at /edit/install/ConfigurationExamples.aspx for examples
		* If you can't use the web interface for installation you can find create database scripts  below the folder N2.Installation\SqlScripts below in the N2 solution
		* Now you need to insert the first node (the root node). There is an option on the installation page to do this
	* IMPORTANT! Change the default password in web.config. If you've installed, configured and created an administrator account using a membership provider, comment out this section entirely. 
	* If you want intellisense on the n2 configuration section you should place the n2configuraiton.xsd in "C:\Program Files\Microsoft Visual Studio 8\Xml\Schemas"


N2 FEATURES:

Easy to get started with
    * Have an example up and running in less than 5 minutes
    * Step by step startup guide
    * Takes care of database persistence and provides a nice editor UI
    * Built for ASP.NET 2.0; no need to jump on a train or learn any of those pesky scripting languages 

Extendable platform 
    * Define and extend content types using inheritance and attributes
    * Create reusable modules, content types and editors
    * Interact with, replace or override core functionality
    * Easily manage content programmatically

Lightweight and performant
    * Develop snazzy templates (aspx pages) and functionality just the way you like without the CMS getting in the way
    * Is easily plugged into an existing application
    * Quickly delivers content to your templates 

Thought out framework
    * Gently nudges you to define content in a manageable way
    * Built in mechanisms to handle relations, security, constraints, filtering and more

Friendly
    * Enhance any kind of enterprisey feature with user defined content with fewer lines of code and doing less repetitive labor
    * Edit interface is quick and responsive - and intuitive too
    * Allows developers to concentrate on style and functionality while giving users power over structure and content


CONTRIBUTORS
	* Cristian Libardo
	* Sten Johanneson
	* Michele Scaramal