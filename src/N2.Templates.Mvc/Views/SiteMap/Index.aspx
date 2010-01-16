<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.Master" AutoEventWireup="true"
	Inherits="System.Web.Mvc.ViewPage<N2.Templates.Mvc.Models.Pages.SiteMap>" %>
<%@ Import Namespace="N2.Collections"%>

<asp:Content ContentPlaceHolderID="Content" runat="server">
	<%=Html.Display(m => m.Title)%>
	<%=Html.Display(m => m.Text)%>

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