<%@ Page Title="" Language="C#" MasterPageFile="../Site.master" AutoEventWireup="true"
	Inherits="System.Web.Mvc.ViewPage<N2.Templates.Mvc.Models.Pages.SiteMap>" %>
<%@ Import Namespace="N2.Collections"%>

<asp:Content ContentPlaceHolderID="Content" runat="server">
	<%=Html.DisplayContent(m => m.Title)%>
	<%=Html.DisplayContent(m => m.Text)%>

	<ul>
		<%= N2.Web.Tree.From(Find.ClosestLanguageRoot).ExcludeRoot(true).Filters(new NavigationFilter()) %>
	</ul>
</asp:Content>
