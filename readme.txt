In order to run the examples you'll need to update certain dependencies. 
Running Prepare_Dependencies-vs2008.bat should do it. If you need to do
this manually this probably involves looking at the error message and 
retrieving the required libraries from the "lib" folder.

The templates project and most of the examples uses an SqLite embedded 
database. However, this won't do in Medium Trust environments hosting.

To try the templates (this is a default implementation with some 
functionality) open src\N2.Sources-vs2008.sln. This project requires 
VS2008 SP1 but you can run the released examples with .NET 2.0 and 
older versions of Visual Studio. Set Templates\N2.Templates as startup 
project and run (Ctrl+F5) from Visual Studio.

To start experiment with minimum level of fluff you can look at the 
projects available in the examples folder. To set up the examples
run examples\Prepare_Dependencies-vs2008.bat.

You are very welcome to let me know about your build experiences in the 
forum so I can improve things.
