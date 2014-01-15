<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<NewsContainerModel>" %>
<% for(var i = 0; i < Model.News.Count; i++){ %>
    <div class="item i<%= i + Model.Skip %> a<%= i % 2 %>">
        <span class="date"><%= Model.News[i].Published %></span>
        <a href="<%= Model.News[i].Url %>"><%= Model.News[i].Title %></a>
        <p><%= Model.News[i].Summary %></p>
    </div>
<% } %>

<% if (!Model.IsLast) { %>
<%= Html.ActionLink(string.Format("{0} {1}-{2} »", Model.Container.Title, (Model.Skip + Model.Take), (Model.Skip + Model.Take * 2)), Model.Container, "range", new { skip = Model.Skip + Model.Take, take = Model.Take, tag = Model.Tag }, new { @class = "scroller" })%>
<% } %>