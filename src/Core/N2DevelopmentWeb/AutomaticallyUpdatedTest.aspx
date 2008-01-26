<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutomaticallyUpdatedTest.aspx.cs" Inherits="N2.TemplateWeb.AutomaticallyUpdatedTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    I hadn't figured out before that changes are automatically persisted.
    <b>Last updated: </b><%= CurrentItem.Updated %>
    </div>
    </form>
</body>
</html>
