<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Register TagPrefix="n2" Namespace="N2.Web.UI.WebControls" Assembly="N2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:SiteMapDataSource ID="SiteMapSource" SiteMapProvider="PublicSiteMap" runat="server" />
		<asp:TreeView runat="server" DataSourceID="SiteMapSource" />
    </div>
    </form>
</body>
</html>
