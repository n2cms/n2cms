<%@ Page Title="" Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
    <title id="t" runat="server" title='<%$ CurrentPage: Title %>' />
</head>
<body>
    <form id="F" runat="server">
    <n2:SlidingCurtain ID="sc" runat="server">
        <n2:DragDropControlPanel ID="cp" runat="server"/>
    </n2:SlidingCurtain>
	</form>
</body>
</html>