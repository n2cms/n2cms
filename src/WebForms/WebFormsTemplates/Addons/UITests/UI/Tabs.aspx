<%@ Page Title="" Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
    <title id="t" runat="server" title='<%$ CurrentPage: Title %>' />
    <link rel="Stylesheet" href="/N2/Resources/Css/framed.css" />
</head>
<body>
	<n2:TabPanel ID="TabPanel1" runat="server" ToolTip="Tab 1" CssClass="tabPanel group1">
		Tab 1 Content
		
		<n2:TabPanel ID="TabPanel7" runat="server" ToolTip="Tab A" CssClass="tabPanel group3">
			Tab A Content
		</n2:TabPanel>
		<n2:TabPanel ID="TabPanel8" runat="server" ToolTip="Tab B" CssClass="tabPanel group3">
			Tab B Content
		</n2:TabPanel>
		<n2:TabPanel ID="TabPanel9" runat="server" ToolTip="Tab C" CssClass="tabPanel group3">
			Tab C Content
		</n2:TabPanel>
	</n2:TabPanel>
	<n2:TabPanel ID="TabPanel2" runat="server" ToolTip="Tab 2" CssClass="tabPanel group1">
		Tab 2 Content
	</n2:TabPanel>
	<n2:TabPanel ID="TabPanel3" runat="server" ToolTip="Tab 3" CssClass="tabPanel group1">
		Tab 3 Content
	</n2:TabPanel>
	<n2:TabPanel ID="TabPanel4" runat="server" ToolTip="Tab A" CssClass="tabPanel group2">
		Tab A Content
	</n2:TabPanel>
	<n2:TabPanel ID="TabPanel5" runat="server" ToolTip="Tab B" CssClass="tabPanel group2">
		Tab B Content
	</n2:TabPanel>
	<n2:TabPanel ID="TabPanel6" runat="server" ToolTip="Tab C" CssClass="tabPanel group2">
		Tab C Content
	</n2:TabPanel>
</body>
</html>
