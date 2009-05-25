<%@ Page Theme="Default" Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
            <!-- We can output raw content data into the template like this -->
    <title><%= CurrentPage.Title %></title>
</head>
<body>
    <form id="form1" runat="server">
		<n2:SlidingCurtain runat="server"><n2:ControlPanel runat="server" /></n2:SlidingCurtain>
        <h5 class="header">The N2 Example Site</h5>
        <div class="menu">
			<asp:SiteMapDataSource ID="N2SiteMap" runat="server" SiteMapProvider="PublicSiteMap" ShowStartingNode="false" />
			<asp:Menu ID="Menu1" runat="server" DataSourceID="N2SiteMap" Orientation="Horizontal" StaticSelectedStyle-Font-Bold="true">
				<DynamicMenuStyle CssClass="subMenu" />
				<DynamicHoverStyle CssClass="menuHover" />
				<StaticHoverStyle CssClass="menuHover" />
			</asp:Menu>
        </div>
		
		<!-- The droppable zone enables drop of parts onto the tempalate when in drag&drop mode -->        
        <div class="zone">
			<n2:DroppableZone runat="server" ZoneName="SecondaryContent"/>
		</div>
		
        <asp:SiteMapPath ID="Path" runat="server" CssClass="breadcrumb" />


        <!-- The display control uses the default presentation for an item's property, the title in this case uses header 1 -->
        <n2:Display ID="TitleDisplay" PropertyName="Title" runat="server" />
        <div>
            <!-- This is a way to inject data into a webforms control, in this case we're injecting the current page's text property -->
            <asp:Literal ID="TextLiteral" Text="<%$ CurrentPage: Text %>" runat="server" />
        </div>
    </form>
</body>
</html>
