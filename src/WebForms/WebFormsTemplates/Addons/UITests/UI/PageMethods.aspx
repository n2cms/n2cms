<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PageMethods.aspx.cs" Inherits="N2.Addons.UITests.UI.PageMethods" %>

<%@ Register TagPrefix="asp" Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
	Namespace="System.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<asp:ScriptManager runat="server" EnablePageMethods="true" ScriptPath="~/Addons/UITests/UI/" ScriptMode="Release" />
		<span id="UpdateTarget">Waiting for update...</span>
		<input type="button" value="Update" onclick="UpdateTime();" />

		<script language="javascript">
			function UpdateTime(e) {
				var $t = $get('UpdateTarget');
				PageMethods.GetCurrentDate(function(result, userContext, methodName) {
					$t.innerHTML = result.d;
				}, function(error, userContext, methodName) {
					$t.innerHTML = "Error: " + error;
				});
				return false;
			}
		</script>

<hr />
Update web.config to use:
<textarea style="height:150px; width:400px; vertical-align:top">
<httpModules>
	<add name="ScriptModule" type="System.Web.Handlers.ScriptModule, System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35"/>
</httpModules>
</textarea>
	</div>
	</form>
</body>
</html>
