<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.Master" AutoEventWireup="true" Inherits="N2.Templates.Web.UI.TemplatePage" %>
<script runat="server">
	protected override void OnPreInit(EventArgs e)
	{
		CurrentPage = new N2.Templates.Items.Redirect();
		CurrentPage.Parent = N2.Templates.Find.StartPage;
		base.OnPreInit(e);
	}
</script>
<asp:Content ID="Content8" ContentPlaceHolderID="Content" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">Content</div>
</asp:Content>
