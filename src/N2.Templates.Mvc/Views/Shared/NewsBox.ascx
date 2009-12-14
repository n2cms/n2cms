<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<NewsList>" %>
<%@ Import Namespace="N2.Templates.Mvc.Items.Pages"%>
<%@ Import Namespace="N2.Templates.Mvc.Items.Items"%>

<%= Html.Display(m => m.Title) %>
<div class="box"><div class="inner">
		<div class="sidelist">
		<%
    		var i = 0;
			foreach (var item in Model.Container.GetChildren<News>().Take(Model.MaxNews))
			{
				i++;%>
			<div class="news i<%= i %> a<%= i % 2 %>">
				<a href='<%=item.Url%>' title='<%=item.Published + ", " + item.Introduction%>'><%=item.Title%></a>
			</div>
		<%
			}
		%>
		</div>
</div></div>