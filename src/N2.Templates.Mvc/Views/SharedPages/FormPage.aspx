<%@ Page MasterPageFile="../Shared/Top+SubMenu.Master" Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<FormPage>" %>
<asp:Content ID="cpc" ContentPlaceHolderID="PostContent" runat="server">
	<%=Html.Display(m => m.Form)%>
</asp:Content>