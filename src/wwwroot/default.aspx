<%@ Page Language="C#" %>
<html>
	<body>
		<div>
			<h1>Undefined Template or Site Not Installed</h1>
			<h2>How to fix Undefined Template</h2>
			<p>If you know you have installed you might need to decorate the content class for the start page node with the [Template] attribute.</p>
			<pre><code>
	[Template("~/Path/To/Template.aspx")]
	public void MyStartPage : ContentItem
	{
	}
			</code></pre>
			<h2>How to fix Site Not Installed</h2>
			<p>Follow the instructions the <a href="edit/install">installation wizard</a> to set up database and create your start node.</p>
			
			
			<p>Please leave this file here to start ASP.NET when requesting the root folder (/).</p>
		</div>
	</body>
</html>