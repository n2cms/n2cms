<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true" 
	Inherits="N2.Web.Mvc.ContentViewPage<SearchModel, AbstractSearch>" Title="" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
	<link rel="stylesheet" type="text/css" href="<%=ResolveUrl("~/Content/Css/Search.css") %>" />
</asp:Content>

<asp:Content ContentPlaceHolderID="PostContent" runat="server">
	<%using(Html.BeginForm<SearchController>(c => c.Index(null, null), FormMethod.Get)){%>
		<%=Html.TextBox("q")%>
		<%=Html.SubmitButton()%>
	<%} %>

	<%if(Model.Results.Count > 0){%>
		<span><%=Model.TotalResults%> pages found for search term '<%=Html.Encode(Model.SearchTerm)%>'.</span>
		<div class="list">
		<% var i = 0; %>
		<%foreach(var result in Model.Results){%>
			<div class="item hit cf i<%= i++ %> a<%= i % 2 %>">
				<a href="<%= result.Url %>"><%= result.Title %></a>
			</div>
		<%} %>
		</div>
		<div id="pg">
			<%if (Model.PageNumber > 0){%>
			<%=Html.ActionLink<SearchController>(c => c.Index(Model.SearchTerm, Model.PageNumber - 1), "< Prev", new{id="pg-prev"})%>
			<%}%>
			<%for(i = 0; i < Model.TotalPages; i++){%>
				<%if(i == Model.PageNumber){%>
				<strong><%=i + 1%></strong>
				<%}else{%>
				<%=Html.ActionLink<SearchController>(c => c.Index(Model.SearchTerm, i), (i + 1).ToString())%>
				<%}%>
			<%} %>
			<%if(Model.PageNumber < Model.TotalPages - 1){%>
			<%=Html.ActionLink<SearchController>(c => c.Index(Model.SearchTerm, Model.PageNumber + 1), "Next >", new{id="pg-next"})%>
			<%}%>
		</div>
	<%}else if(Model.HasSearchTerm){%>
		<p>No Results could be found</p>
	<%}%>
</asp:Content>