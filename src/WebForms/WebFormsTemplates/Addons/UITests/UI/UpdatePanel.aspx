<%@ Page Language="C#" %>
<%@ Register TagPrefix="asp" Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:ScriptManager runat="server" />
		Loaded: <%= DateTime.Now %>
		<asp:UpdatePanel runat="server">
			<ContentTemplate>
				Updated: <%= DateTime.Now %> <asp:Button runat="server" Text="Update" />
			</ContentTemplate>
		</asp:UpdatePanel>

<textarea style="height:250px;width:800px">
Update web.config to try this update panels
	<httpHandlers>
		<remove verb="*" path="*.asmx"/>
		<add verb="*" path="*.asmx" validate="false" type="System.Web.Script.Services.ScriptHandlerFactory, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"/>
		<add verb="*" path="*_AppService.axd" validate="false" type="System.Web.Script.Services.ScriptHandlerFactory, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"/>
		<add verb="GET,HEAD" path="ScriptResource.axd" type="System.Web.Handlers.ScriptResourceHandler, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" validate="false"/>
	</httpHandlers>
</textarea>	
    </div>
    </form>
</body>
</html>
