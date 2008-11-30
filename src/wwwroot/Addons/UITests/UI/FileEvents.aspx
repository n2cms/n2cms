<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileEvents.aspx.cs" Inherits="N2.Addons.UITests.UI.FileEvents" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    	<br />
		<asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="true"
			oncheckedchanged="CheckBox2_CheckedChanged" Text="Throw" />
		<asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="true"
			oncheckedchanged="CheckBox1_CheckedChanged" Text="show events" />
		<br />
		<asp:TextBox ID="TextBox1" runat="server" Rows="100" Columns="100" TextMode="MultiLine"></asp:TextBox>
    
    </div>
    </form>
</body>
</html>
