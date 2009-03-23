<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="N2Site.Edit.Test" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/Edit/wmd/wmd.css" />
	<script type="text/javascript">		wmd_options = { output: "Markdown" };</script>
	<script src="wmd/jQuery/jquery-1.2.6.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Edit/wmd/showdown.js"></script>
    <script type="text/javascript" src="/Edit/wmd/wmd.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<div id="wmd-button-bar" class="wmd-panel"></div>
		<textarea name="ctl00$Content$ie$ctl10" rows="2" cols="20" class="wmd-panel wmd-textarea"></textarea>
		
    </div>
    </form>
</body>
</html>
