<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.master" AutoEventWireup="true" 
	Inherits="N2.Web.Mvc.N2ModelViewPage<SearchModel, AbstractSearch>" Title="" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
	<link rel="stylesheet" type="text/css" href="<%=ResolveUrl("~/Content/Css/Search.css") %>" />
</asp:Content>

<asp:Content ContentPlaceHolderID="PostContent" runat="server">
	<%using(Html.BeginForm<SearchController>(c => c.Index(null), FormMethod.Get)){%>
		<%=Html.TextBox("q")%>
		<%=Html.SubmitButton()%>
	<%} %>

	<%if(Model.Results.Count > 0){%>
		<span><%=Model.Results.Count%> pages found for search term '<%=Html.Encode(Model.SearchTerm)%>'.</span>
		<div class="list">
		<% var i = 0; %>
		<%foreach(var result in Model.Results){%>
			<div class="item hit cf i<%= i++ %> a<%= i % 2 %>">
				<a href="<%= result.Url %>"><%= result.Title %></a>
			</div>
		<%} %>
		</div>
	<%}else if(Model.HasSearchTerm){%>
		<p>No Results could be found </p>
	<%}%>
</asp:Content>