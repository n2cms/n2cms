<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Root.aspx.cs" Inherits="N2.Templates.UI.Secured.Root" Title="Root" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
	<title>Root</title>
	<style>
		body { font-family: Arial; background-color: #C3C0BD; }
		label { margin-right: 10px; }
		.uc { background-color: #fff; padding: 10px; border: solid 1px #A9A6A3; }
		h1, h2, h3, h4 { margin: .5em 0; }
		.field { border-bottom: solid 1px #C3C0BD; padding-bottom: 2px; margin-bottom: 2px; }
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<n2:Zone ID="Zone1" ZoneName="Above" runat="server" />
		<table>
			<tr>
				<td>
					<n2:Zone ID="Zone2" ZoneName="Left" runat="server" />
				</td>
				<td>
					<n2:Zone ID="Zone3" ZoneName="Center" runat="server" />
				</td>
				<td>
					<n2:Zone ID="Zone4" ZoneName="Right" runat="server" />
				</td>
			</tr>
		</table>
		<n2:Zone ID="Zone5" ZoneName="Below" runat="server" />
	</div>
	</form>
</body>
</html>
