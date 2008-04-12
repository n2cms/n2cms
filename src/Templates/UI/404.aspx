<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="404.aspx.cs" Inherits="System.Web.UI.Page" %>

<script runat="server">

	protected override void OnInit(EventArgs args)
	{
		N2.ContentItem page = N2.Templates.Find.StartPage.NotFoundPage;
		if (page != null)
		{
			Server.Transfer(page.RewrittenUrl);
		}
	}

</script>

<h1>Page Not Found (404)</h1>