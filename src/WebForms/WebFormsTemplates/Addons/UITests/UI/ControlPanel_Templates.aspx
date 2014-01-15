<%@ Page Title="" Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
    <title id="t" runat="server" title='<%$ CurrentPage: Title %>' />
</head>
<body>
    <form id="F" runat="server">
    <n2:SlidingCurtain ID="sc" runat="server">
        <n2:ControlPanel ID="cp" runat="server">
			<DragDropFooterTemplate><div>DragDropFooterTemplate</div></DragDropFooterTemplate>
			<DragDropHeaderTemplate><div>DragDropHeaderTemplate</div></DragDropHeaderTemplate>
			<EditingFooterTemplate><div>EditingFooterTemplate</div></EditingFooterTemplate>
			<EditingHeaderTemplate><div>EditingHeaderTemplate</div></EditingHeaderTemplate>
			<HiddenTemplate><div>HiddenTemplate</div></HiddenTemplate>
			<PreviewingFooterTemplate><div>PreviewingFooterTemplate</div></PreviewingFooterTemplate>
			<PreviewingHeaderTemplate><div>PreviewingHeaderTemplate</div></PreviewingHeaderTemplate>
			<VisibleFooterTemplate><div>VisibleFooterTemplate</div></VisibleFooterTemplate>
			<VisibleHeaderTemplate><div>VisibleHeaderTemplate</div></VisibleHeaderTemplate>
        </n2:ControlPanel>
    </n2:SlidingCurtain>
    <n2:Zone ZoneName="Right" runat="server" />
		<br /><br /><br />
		<n2:EditableDisplay ID="EditableDisplay1" runat="server" PropertyName="Title" />
		<hr />
		<a href="<%= N2.Web.Url.Parse(Request.RawUrl).SetQueryParameter("preview", null).SetQueryParameter("edit", null) %>">none</a>
		<a href="<%= N2.Web.Url.Parse(Request.RawUrl).SetQueryParameter("edit", "drag") %>">drag</a>
		<a href="<%= N2.Web.Url.Parse(Request.RawUrl).SetQueryParameter("preview", "true") %>">preview</a>
		<a href="<%= N2.Web.Url.Parse(Request.RawUrl).SetQueryParameter("edit", "true") %>">edit</a>
		<hr />
		<n2:Zone ID="Zone1" ZoneName="Right" runat="server" />
	</form>
</body>
</html>
