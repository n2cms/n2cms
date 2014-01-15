<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="N2"%>
<%@ Register TagPrefix="n2" Namespace="N2.Web.UI.WebControls" Assembly="N2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<n2:Display PropertyName="Title" runat="server" />
		<n2:Display PropertyName="Text" runat="server" />
		<n2:Display PropertyName="NonExistant" runat="server" SwallowExceptions="true" />
    </div>
    </form>
</body>
</html>
