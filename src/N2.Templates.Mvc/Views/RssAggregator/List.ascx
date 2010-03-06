<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<RssAggregatorModel>" %>
<div class="uc">
	<h4><%=Model.Item.Title %></h4>
	<div class="box">
		<div class="inner">
			<%=Model.Item.Text%>
			<div class="sidelist">
				<% var i = 0; %>
				<%foreach (var item in Model.Items){%>
				<div class="item news i<%=i%> a<%=i++ % 2 %>">
					<a href="<%=item.Url%>" title="<%=item.Published%>">
						<%=item.Title%></a>
				</div>
				<%}%>
			</div>
		</div>
	</div>
</div>