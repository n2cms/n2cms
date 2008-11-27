<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdaptiveItem.aspx.cs" Inherits="N2.Addons.UITests.UI.AdaptiveItem" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<ul>
		<% foreach (string file in System.IO.Directory.GetFiles(Server.MapPath("~/Addons/UITests/UI"), "*.aspx")) { %>
			<li><a href="<%= N2.Web.Url.Parse(CurrentPage.Url).AppendSegment(System.IO.Path.GetFileNameWithoutExtension(file)) %>"><%= System.IO.Path.GetFileNameWithoutExtension(file)%></a></li>
		<% } %>
		</ul>
    </div>
    </form>
</body>
</html>
