In order to run the examples you'll need to update certain dependencies. 
Running Prepare_Dependencies-vs2008.bat should do it. If you need to do
this manually this probably involves looking at the error message and 
retrieving the required libraries from the "lib" folder.

The templates project and most of the examples uses an SqLite embedded 
database. This won't do in Medium Trust environments such as godaddy

To try the templates (this is a default implementation with some 
functionality) open src\N2.Everything-vs2008.sln. Some of the projects
requires VS2008 SP1. If you don't have these you can just unload those 
projects.

To start experiment with minimum level of fluff you can look at the 
projects available in the examples folder.

You are very welcome to let me know about your build experiences in the forum
so I can improve things.
