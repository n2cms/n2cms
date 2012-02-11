<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<News>>" %>
<%@ Import Namespace="N2.Templates.Mvc.Models.Pages"%>
<%@ Import Namespace="N2.Templates.Mvc.Models.Parts"%>
<div class="uc">
	<%= Html.DisplayContent("Title") %>
	<div class="box"><div class="inner">
			<div class="sidelist">
			<%
    			var i = 0;
				foreach (var item in Model)
				{
					i++;%>
				<div class="item news i<%= i %> a<%= i % 2 %>">
					<a href='<%=item.Url%>' title='<%=item.Published + ", " + item.Summary%>'><%=item.Title%></a>
				</div>
			<%
				}
			%>
			</div>
	</div></div>
</div>