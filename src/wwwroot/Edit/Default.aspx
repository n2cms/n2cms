<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Default" Title="Edit" meta:resourcekey="DefaultResource" %>
<%@ Register Src="Toolbar.ascx" TagName="Toolbar" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
		<title>Edit</title>
        <link rel="stylesheet" href="Css/All.css" type="text/css" />
        <link rel="stylesheet" href="Css/Default.css" type="text/css" />
        <!--[if IE 6]>
		<link rel="stylesheet" href="Css/IE6.css" type="text/css" />
		<![endif]-->
    </head>
    <body id="default">
        <form id="form1" runat="server" class="frameForm">
			<uc1:Toolbar runat="server" />
			
            <div id="splitter">
				<div id="leftPane" class="pane">
					<iframe id="navigation" src="<%= GetNavigationUrl(SelectedItem) %>" frameborder="0" name="navigation" class="frame"></iframe>
				</div>
				<div id="rightPane" class="pane">
					<iframe id="preview" src="<%= GetPreviewUrl(SelectedItem) %>" frameborder="0" name="preview" class="frame"></iframe>
				</div>
            </div>
        </form>

        <script type="text/javascript">
        	window.name = "top";
        	window.n2ctx.hasTop = function() { return true; }
        	window.n2ctx.setupToolbar('<%= SelectedPath %>');

        	window.n2.frameManager.init();
       </script>
    </body>
</html>
