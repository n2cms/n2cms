<%@ Page Language="C#" %>
<html>
	<body>
		<h1>Undefined template</h1>
		<p>Please decorate your start content item with the [Template] attribute.</p>
		<pre><code>
	[Template("~/Path/To/Template.aspx")]
	public void MyStartPage : ContentItem
	{
	}
		</code></pre>
		<p>Please leave this file here to start ASP.NET when requesting the root folder (/).</p>
	</body>
</html>