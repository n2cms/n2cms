<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.Master" AutoEventWireup="true"
	Inherits="N2.Web.Mvc.N2ViewPage<N2.Templates.Mvc.Items.Pages.SiteMap>" %>
<%@ Import Namespace="N2.Collections"%>

<asp:Content ContentPlaceHolderID="Content" runat="server">
	<%=this.EditableDisplay(m => m.Title)%>
	<%=this.EditableDisplay(m => m.Text)%>

	<div id="product-category-info">
		<%=RenderSiteMap()%>
	</div>
</asp:Content>

<script runat="server">
	static string RenderSiteMap()
	{
		var tree = N2.Web.Tree.From(N2.Context.Current.UrlParser.StartPage)
			.Filters(new VisibleFilter(), new PageFilter());

		return tree.ToString();
	}
</script>