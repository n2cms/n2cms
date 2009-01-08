<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageMethods.aspx.cs" Inherits="N2.Addons.UITests.UI.PageMethods" %>
<%@ Register TagPrefix="asp" Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<b>This doesn't work</b>
		<asp:ScriptManager runat="server" EnablePageMethods="true"  />
		
		<span id="UpdateTarget" /><input type="button" value="Update" onclick="UpdateTime();" />
		
		<script language="javascript">
			function UpdateTime() {
				var $t = $get('UpdateTarget');
				PageMethods.GetCurrentDate(function(result, userContext, methodName) {
					$t.innerHTML = result;
				}, function(error, userContext, methodName) {
					$t.innerHTML = "Error: " + error;
				});
			}
		</script>

    </div>
    </form>
</body>
</html>
