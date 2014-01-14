<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FixClass.aspx.cs" Inherits="N2.Edit.Install.FixClass" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../Resources/bootstrap/css/bootstrap.min.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/all.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/framed.css" />
    <link rel="stylesheet" type="text/css" href="../Resources/Css/themes/default.css" />
    <style>b { color:red;}pre{margin:10px 0;border-left:solid 10px #eee;padding-left:10px;}input{vertical-align:middle} .msg{font-weight:bold; font-size:1.4em;color:green;background-color:Yellow;padding:10px 0;}</style>
</head>
<body>
    <form id="form1" runat="server">
	<ul class="tabs">
		<li class="tab selected"><a href="#">Invalid class fixing tool</a></li>
	</ul>
    <div class="tabPanel">
        <asp:Label CssClass="msg" runat="server" ID="lblResult" />
        <h1>Invalid item(s) in database (class namechange or missing dll?)</h1>
        <p>An invalid class was found in the database (NHibernate.InvalidClassException). A few possible explanations:</p>
        <ul>
            <li><b>Missing dll.</b> A dll containing class definitions (e.g. N2.Management.dll) is no longer available in the /bin folder</li>
            <li><b>Changed or removed class.</b> The name of a class has changed or a class has been removed. E.g. a content class such as
                <pre>[Definition]
public class PageItem</pre> 
                could have been renamed to 
                <pre>[Definition]
public class <b>Other</b>PageItem</pre>
            </li>
            <li><b>Changed discriminator.</b> The discriminator of a class definition has changed e.g.
                <pre>[Definition("My page", "Discriminator")]
public class MyPage...</pre>
                <pre>[Definition("My page", "<b>Changed</b>Discriminator")]
public class MyPage...</pre>
            </li>
            <li><b>UFO landing.</b> Someone did nasty experiments to your database...</li>
        </ul>
        <h2>To fix it you can either </h2>
            <ul>
            <li>Act intelligently upon the this information.</li>
            <li>Try <asp:Button OnClientClick="return confirm('The nodes and all their CHILD nodes will be DELETED completly and irrecoverably. You can't get them back afterwards. Please note that child nodes of other types that could otherwise work also will be deleted. Delete?')" runat="server" Text="deleting" OnClick="btnDelete_Click" /> the offending nodes (use at own risk).</li>
            <li>
                Try <asp:Button OnClientClick="return confirm('This option may result in an invalid database state such as stale data and traumatized application logic. Please use with caution. Continue?');" ID="Button1" runat="server" Text="changing" OnClick="btnChange_Click" /> them 
                into a <asp:DropDownList ID="ddlType" runat="server" DataTextField="Title" DataValueField="Discriminator" /> (use with caution).
            </li>
        </ul>
        <p>These are are the problematic classes in the database. Their type/discriminator doesn't match any content class definition (class name or attribute name/discriminator) in the application:</p>
        <asp:DataGrid ID="dgrItems" runat="server" CssClass="table table-striped table-hover table-condensed"></asp:DataGrid>

    </div>
    </form>
</body>
</html>
