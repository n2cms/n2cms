<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Default" Title="Edit" meta:resourcekey="DefaultResource" %>
<%@ Register Src="Toolbar.ascx" TagName="Toolbar" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
		<title>Edit</title>
        <link rel="stylesheet" href="Css/All.css" type="text/css" />
        <link rel="stylesheet" href="Css/Default.css" type="text/css" />
        <script src="Js/plugins.ashx" type="text/javascript" ></script>
        <!--[if IE 6]>
		<link rel="stylesheet" href="Css/IE6.css" type="text/css" />
		<![endif]-->
    </head>
    <body id="default">
        <form id="form1" runat="server">
			<uc1:Toolbar runat="server" />
			
            <script type="text/javascript">
                $(document).ready(function() {
                    window.n2 = new frameManager();
                    n2.initFrames();
                });
            </script>
            <div id="splitter">
				<div id="leftPane">
					<iframe id="navigation" src="<%= GetNavigationUrl(SelectedItem) %>" frameborder="0" name="navigation" width="25%" height="500"></iframe>
				</div>
				<div id="rightPane">
					<iframe id="preview" src="<%= GetPreviewUrl(SelectedItem) %>" frameborder="0" name="preview" width="75%" height="500"></iframe>
				</div>
            </div>
        </form>
    </body>
</html>
