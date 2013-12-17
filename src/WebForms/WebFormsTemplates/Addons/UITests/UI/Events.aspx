<%@ Page EnableViewState="false" Language="C#" AutoEventWireup="true" CodeBehind="Events.aspx.cs" Inherits="N2.Addons.UITests.UI.Events" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnSubscribe" runat="server" Text="Subscribe" 
            onclick="btnSubscribe_Click" />
        <asp:Button ID="btnUnsubscribe" runat="server" Text="Unsubscribe" 
            onclick="btnUnsubscribe_Click" />
        <asp:CheckBox ID="chkLoad" runat="server" Text="Log load event" />
        <asp:TextBox ID="txtEventList" runat="server" TextMode="MultiLine" style="width:95%;height:400px;"></asp:TextBox>
    </div>
    </form>
</body>
</html>
