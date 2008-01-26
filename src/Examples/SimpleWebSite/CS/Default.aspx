<%@ Page Theme="Default" Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><%= CurrentPage.Title %></title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>N2 Example Site</h1>
        <div class="menu">
			<asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" SiteMapProvider="PublicSiteMap" ShowStartingNode="false" />
			<asp:Menu ID="Menu1" runat="server" DataSourceID="SiteMapDataSource1" Orientation="Horizontal" StaticSelectedStyle-Font-Bold="true">
				<DynamicMenuStyle CssClass="subMenu" />
				<DynamicHoverStyle CssClass="menuHover" />
				<StaticHoverStyle CssClass="menuHover" />
			</asp:Menu>
        </div>
        <asp:SiteMapPath runat="server" CssClass="breadcrumb" />
        <h2><n2:Display ID="Display1" PropertyName="Title" runat="server" /></h2>
        <div>
            <asp:Literal ID="Literal1" Text="<%$ CurrentPage: Text %>" runat="server" />
        </div>
    </form>
</body>
</html>
