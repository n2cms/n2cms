In order to run the examples and the templates you'll need to update certain 
dependencies. Running Prepare_Dependencies-vs2008.bat should do it.

The solution uses an Sql Express 2005 database. If you can't use Sql Express 
you'll need to perform a few additional steps:
- Create a database
- Configure database connection in src\wwwroot\web.config
- Open the solution src\N2_Everything-vs2008.sln 
- Make sure N2.Templates.UI is Default Web Site
- Run (Ctrl+F5)
- Navigate to .../edit/install/default.aspx in your browser
- Follow the instructions to install

You are very welcome to let me know about your build experiences in the forum
so I can improve things.
