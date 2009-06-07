<%@ Page MasterPageFile="../Shared/Top+SubMenu.Master" Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewPage<FormPage>" %>
<asp:Content ID="cpc" ContentPlaceHolderID="PostContent" runat="server">
	<%=this.Display(m => m.Form)%>
</asp:Content>