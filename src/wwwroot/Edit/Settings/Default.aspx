<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Settings.Default" meta:resourceKey="settingsPage" Culture="auto" UICulture="auto" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Settings</title>
    <link rel="stylesheet" href="../Css/All.css" type="text/css" />
    <link rel="stylesheet" href="../Css/Framed.css" type="text/css" />
	<script src="../Js/plugins.ashx" type="text/javascript" ></script>
    <script type="text/javascript">
        $(document).ready(function(){
			toolbarSelect("settings");
		});
	</script>
</head>
<body class="navigation settings">
    <form id="form1" runat="server">
        <div>
            <edit:ServiceEditor id="se" runat="server" />
            <asp:Button ID="btnSave" runat="server" Text="Save" meta:resourcekey="btnSaveResource1" OnClick="btnSave_Click" />
        </div>
    </form>
</body>
</html>
