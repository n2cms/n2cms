<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HelpFileNotFound.aspx.cs" Inherits="N2.DocSite.HelpFileNotFound" 
Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Help file not found</title>
</head>
<body>
  <form id="form1" runat="server">
    <h2><asp:Localize runat="server" ID="helpFileNotFoundHeader" Text='<%$ Resources:PageResource1.Title %>' /></h2>
    <strong><asp:Localize runat="server" ID="helpFileNotFoundMessageLine1Localize" meta:resourcekey="helpFileNotFoundMessageLine1Localize"
                          Text="The help file you have requested does not exist." /></strong>
    <p><asp:Localize runat="server" ID="helpFileNotFoundMessageLine2Localize" meta:resourcekey="helpFileNotFoundMessageLine2Localize"
                     Text="Please make sure that you have entered the correct web address into your browser.  For further assistance please contact the site administrator." /></p>
  </form>
</body>
</html>
