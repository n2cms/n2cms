<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<RootPage>" Title="Root" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
	<title>Root</title>
	<link rel="stylesheet" type="text/css" href="<%=ResolveUrl("~/edit/css/edit.css")%>" />
	<style type="text/css">
		@import url(/edit/Css/edit.css);

		.nameEditor, .titleEditor{
			display:block;
			width:auto;
			width:100%;
		}
		#main .columns{
		}
		#main .columns .left, #main .columns .right{
			margin-top:10px;
			width:auto;
			float:none;
		}
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
		<% Html.RenderZone("Above"); %>
		<table>
			<tr>
				<td>
					<% Html.RenderZone(Zones.Left); %>
				</td>
				<td>
					<% Html.RenderZone("Center"); %>
				</td>
				<td>
					<% Html.RenderZone(Zones.Right); %>
				</td>
			</tr>
		</table>
		<% Html.RenderZone("Below"); %>
	</div>
	</form>
</body>
</html>