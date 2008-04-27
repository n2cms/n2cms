<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Special.aspx.cs" Inherits="N2DevelopmentWeb.Templates.Special" %>

<%@ Register Src="NonDynamicUcTest.ascx" TagName="NonDynamicUcTest" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><%= CurrentPage.Title %></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Label ID="Label1" Text="<%$ CurrentItem:Title %>" runat="Server" ForeColor="RED" />
    <div>
        <asp:Label ID="Label2" Text="<%$ CurrentItem:Text %>" runat="Server" ForeColor="Blue" />
        <asp:Label ID="Label3" Text="<%$ CurrentItem:SpecialText %>" runat="Server" />
        <br />
        <br />
        <uc1:NonDynamicUcTest ID="NonDynamicUcTest1" runat="server" />
        
    </div>
    <%= Request.RawUrl %>
    </form>
</body>
</html>
