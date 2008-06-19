ABOUT N2 CMS

N2 is a lightweight CMS framework that helps you build great web sites that 
anyone can update. Web content editors will enjoy a simple and enpowering web 
interface while developers can benefit from a discoverable and programmer 
friendly API.

More information and documentation on the project web site: http://n2cms.com


GETTING STARTED

The easiest option is probably downloading the examples. Either templates 
implementation example for more fluff or the simple example to get the bare 
bones.


INSTALLATION

* see install.txt


TEMPLATES

The templates is a set of out of the CMS functionality. Use this if you want 
the most common features quickly. Open src\N2_Everything-vs2008.sln to view the
code or src\Examples\TemplatesImplementation\MyProject-vs200[5|8].sln.


EXAMPLES

There are a bunch of examples in the Examples src/Examples directory of the 
source code package. To run them you need to copy the compiled output of the
core project and the edit interface into the example's web roots. 
Prepare_Dependencies-vs2008.bat should do this for you. The examples:

* MediumTrust: How to run N2 in a medium trust environment (favoured by many 
  shared hosting providers).
* MultipleDomains: How to run a single N2 installation with multiple domains
* Mvc: Integration with the ASP.NET MVC framework
* OldGlobalization: This example is deprecated. There is a new globalization 
  approach.
* Parts: Bare minimum project using "parts", i.e. drag'n'drop components on a 
  page.
* SimpleWebSite: Very simple web sites in C# and Visual Basic (least noise for 
  grokking the basics).
* TemplatesImplementation: A proposed approach at integrating with the 
  templates project.


CONTRIBUTORS

* Cristian Libardo
* Sten Johanneson
* Michele Scaramal
* Martijn Rasenberg
* esteewhy