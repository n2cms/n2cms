<%@ Page Title="" Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
    <title id="t" runat="server" title='<%$ CurrentPage: Title %>' />
</head>
<body>
    <form id="F" runat="server">
    <n2:SlidingCurtain ID="sc" runat="server">
        <n2:DragDropControlPanel ID="cp" runat="server">
			<DragDropFooterTemplate><div>DragDropFooterTemplate</div></DragDropFooterTemplate>
			<DragDropHeaderTemplate><div>DragDropHeaderTemplate</div></DragDropHeaderTemplate>
			<EditingFooterTemplate><div>EditingFooterTemplate</div></EditingFooterTemplate>
			<EditingHeaderTemplate><div>EditingHeaderTemplate</div></EditingHeaderTemplate>
			<HiddenTemplate><div>HiddenTemplate</div></HiddenTemplate>
			<PreviewingFooterTemplate><div>PreviewingFooterTemplate</div></PreviewingFooterTemplate>
			<PreviewingHeaderTemplate><div>PreviewingHeaderTemplate</div></PreviewingHeaderTemplate>
			<VisibleFooterTemplate><div>VisibleFooterTemplate</div></VisibleFooterTemplate>
			<VisibleHeaderTemplate><div>VisibleHeaderTemplate</div></VisibleHeaderTemplate>
        </n2:DragDropControlPanel>
    </n2:SlidingCurtain>
    <n2:Zone ZoneName="Right" runat="server" />
	</form>
</body>
</html>