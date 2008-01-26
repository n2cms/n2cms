<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ItemEditorTesting.aspx.cs" Inherits="N2DevelopmentWeb.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <script src="/Edit/Js/jquery-1.1.4.js" type="text/javascript"></script>
    <script src="/WebResource.axd?d=m2IHvDINsPyUYjPJwXmYsLBlkt-BE9cVONhEeYBll0w1&amp;t=633259202878515376" type="text/javascript"></script>
    <link rel="stylesheet" href="/edit/Css/All.css" type="text/css" />
    <link rel="stylesheet" href="/edit/Css/Framed.css" type="text/css" />
    <link rel="stylesheet" href="/edit/Css/edit.css" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnSave" runat="server" Text="save" OnClick="btnSave_Click" />
        <n2:ItemEditor ID="ieItem" runat="server" ParentItemID="1" ItemTypeName="N2.TemplateWeb.Domain.MyPageData,N2DevelopmentWeb" />
    </div>
    </form>
</body>
</html>
