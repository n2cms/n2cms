<%@ Page Language="C#" Inherits="System.Web.UI.Page" %>
Culture: <%= System.Threading.Thread.CurrentThread.CurrentCulture.Name %><br />
GetGlobalResourceObject: <%= GetGlobalResourceObject("Zones", "Right") %><br />
N2.Utility.GetResourceString (global): <%= N2.Utility.GetResourceString("Zones", "Right") %><br />
AppRelativeCurrentExecutionFilePath: <%= HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath %><br />

<%--N2.Utility.GetResourceString (local): <%= N2.Utility.GetResourceString(null, "versions.ToolTip")%><br />--%>
<%= HttpContext.GetLocalResourceObject("~/edit/Toolbar.ascx", "versions.ToolTip")%>
<%--<%= GetLocalResourceObject("versions.ToolTip") %>--%>


