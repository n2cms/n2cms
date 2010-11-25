<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true" 
	Inherits="N2.Web.Mvc.ContentViewPage<SearchModel, SearchBase>" Title="" %>

<asp:Content ContentPlaceHolderID="PostContent" runat="server">
	<%using(Html.BeginForm("Index", "Search", FormMethod.Get)){%>
		<%=Html.TextBox("q")%>
		<input type="submit" />
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
			<%=Html.ActionLink("< Prev", "Index", "Search", new{id="pg-prev"})%>
			<%}%>
			<%for(i = 0; i < Model.TotalPages; i++){%>
				<%if(i == Model.PageNumber){%>
				<strong><%=i + 1%></strong>
				<%}else{%>
				<%=Html.ActionLink((i + 1).ToString(), "Index", "Search")%>
				<%}%>
			<%} %>
			<%if(Model.PageNumber < Model.TotalPages - 1){%>
			<%=Html.ActionLink("Next >", "Index", "Search", new{id="pg-next"})%>
			<%}%>
		</div>
	<%}else if(Model.HasSearchTerm){%>
		<p>No Results could be found</p>
	<%}%>
</asp:Content>