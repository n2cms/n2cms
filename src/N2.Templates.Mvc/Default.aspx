<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%--<html>
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
			<p>Follow the instructions the <a href="N2/Installation">installation wizard</a> to set up database and create your start node.</p>
			
			
			<p>Please leave this file here to start ASP.NET when requesting the root folder (/).</p>
		</div>
	</body>
</html>--%>

<script runat="server">
	void Page_Load(object sender, EventArgs e)
	{
		// Change the current path so that the Routing handler can correctly interpret
		// the request, then restore the original path so that the OutputCache module
		// can correctly process the response (if caching is enabled).

		string originalPath = Request.Path;
		HttpContext.Current.RewritePath(Request.ApplicationPath, false);
		IHttpHandler httpHandler = new MvcHttpHandler();
		httpHandler.ProcessRequest(HttpContext.Current);
		HttpContext.Current.RewritePath(originalPath, false);
	}
</script>