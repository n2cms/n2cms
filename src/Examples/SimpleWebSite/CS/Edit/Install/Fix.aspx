<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Fix.aspx.cs" Inherits="N2.Edit.Install.Fix" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:SqlDataSource ID="SqlDataSource1" runat="server"></asp:SqlDataSource>
		<asp:GridView ID="GridView1" runat="server" DataSourceID="SqlDataSource1">
		</asp:GridView>
    
    </div>
    </form>
</body>
</html>
