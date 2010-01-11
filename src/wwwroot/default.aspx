<%@ Page Language="C#" %>
<html>
	<head>
		<style>
			body { background:#ccc; font-size:.9em; }
			h1, h2, h3, p { font-family: Sans-Serif; }
			div { background: #fff; padding: 10px 20px; margin: 10px auto; }
		</style>
	</head>
	<body>
		<div>
			<h1>Cannot find start page</h1>
			<p>Here are a few known causes.</p>

			<h3>Site Not Installed</h3>
			<p>Follow the instructions in the <a href="N2/Installation">installation wizard</a> to set up database and create your start node.</p>
			<p>Please leave this file here to start ASP.NET when requesting the root folder (/).</p>
			
			<h3>Undefined Template</h3>
			<p>If you know you have installed you might need to decorate the content class for the start page node with the [Template] attribute.</p>
			<pre><code>
	[Template("~/Path/To/Template.aspx")]
	public void MyStartPage : ContentItem
	{
	}
			</code></pre>

			<h3>Error during initialization</h3>
			<p>This could happen when the database contains nodes that have no corresponding class. When using medium trust this could be due to an assmebly not beeing defined.</p>
		</div>
	</body>
</html>