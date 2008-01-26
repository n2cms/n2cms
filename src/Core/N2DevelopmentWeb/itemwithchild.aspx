<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="itemwithchild.aspx.cs" Inherits="N2.TemplateWeb.itemwithchildren" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <%= CurrentPage.Title %>
        <hr />
        ChildItem: <%= CurrentPage.ChildItem.Text %>
        <hr />
        MyChildItem: <%= ((N2.TemplateWeb.Domain.MyItemData)CurrentPage.GetChild("MyChildItem")).Text %>
    </div>
    </form>
</body>
</html>
