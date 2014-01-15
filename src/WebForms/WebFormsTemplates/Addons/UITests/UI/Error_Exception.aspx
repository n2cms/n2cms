<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.Master" AutoEventWireup="true" Inherits="N2.Templates.Web.UI.TemplatePage" %>
<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		throw new ApplicationException("Exception at " + Request.RawUrl);	
		base.OnLoad(e);
	}
</script>
<asp:Content ID="Content8" ContentPlaceHolderID="Content" runat="server">
	
</asp:Content>
