<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.master" AutoEventWireup="true" 
	Inherits="N2.Web.Mvc.ContentViewPage<SearchModel, SearchBase>" Title="" %>

<asp:Content ContentPlaceHolderID="PostContent" runat="server">
	<%using(Html.BeginForm("Index", "Search", FormMethod.Post)){%>
		<%=Html.TextBox("q")%>
		<input type="submit" />
	<%} %>

	<%if(Model.Results.Count > 0){
       string resultMessage = string.Format((string)GetLocalResourceObject("SearchResultFmt"), Model.TotalResults, Html.Encode(Model.SearchTerm));
       %>
		<span><%=Html.Encode(resultMessage)%></span>
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
			<%=Html.ActionLink((string)GetLocalResourceObject("PrevLink"), "Index", "Search", new { q = Html.Encode(Model.SearchTerm), p = Model.PageNumber - 1 }, new { id = "pg-prev" })%>
			<%}%>
			<%for(i = 0; i < Model.TotalPages; i++){%>
				<%if(i == Model.PageNumber){%>
				<strong><%=i + 1%></strong>
				<%}else{%>
				<%=Html.ActionLink((i + 1).ToString(), "Index", "Search", new { q = Html.Encode(Model.SearchTerm), p = i }, null)%>
				<%}%>
			<%} %>
			<%if(Model.PageNumber < Model.TotalPages - 1){%>
			<%=Html.ActionLink((string)GetLocalResourceObject("NextLink"), "Index", "Search", new { q = Html.Encode(Model.SearchTerm), p = Model.PageNumber + 1 }, new { id = "pg-next" })%>
			<%}%>
		</div>
	<%}else if(Model.HasSearchTerm){%>
		<p>No Results could be found</p>
	<%}%>
</asp:Content>
