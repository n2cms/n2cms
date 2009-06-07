<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewUserControl<NewsList>" %>
<%@ Import Namespace="N2.Templates.Mvc.Items.Pages"%>
<%@ Import Namespace="N2.Templates.Mvc.Items.Items"%>
<n2:EditableDisplay runat="server" PropertyName="Title" />
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