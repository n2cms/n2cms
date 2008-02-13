<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MediumTrustTest._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
		<n2:SlidingCurtain runat="server">
			<n2:DragDropControlPanel runat="server" />
		</n2:SlidingCurtain>
		
        <asp:SiteMapDataSource ID="smds" runat="server" />
        <asp:Menu runat="server" DataSourceID="smds"></asp:Menu>
        <asp:SiteMapPath runat="server" />
        <div>
            <n2:Display runat="server" PropertyName="Title" />
            <asp:Literal runat="server" Text="<%$ CurrentItem: Text %>" />
        </div>
        <hr />
        <n2:DroppableZone ZoneName="Content" runat="server" />
    </form>
</body>
</html>
