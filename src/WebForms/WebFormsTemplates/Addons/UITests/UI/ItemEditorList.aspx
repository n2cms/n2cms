<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ItemEditorList.aspx.cs" Inherits="N2.Addons.UITests.UI.ItemEditorList" %>

<script runat="server">

	protected override void OnInit(EventArgs e)
	{
		iel.ZoneName = "Test";
		iel.ParentItem = N2.Context.CurrentPage;
		base.OnInit(e);
	}
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <n2:ItemEditorList ID="iel" runat="server" />
    </div>
    </form>
</body>
</html>
