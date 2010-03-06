<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RssAggregator>" %>
<div class="uc">
<% using (Html.BeginAsyncAction("list")) { %>
	<h4><%=Model.Title %></h4>
	<div class="box">
		<div class="inner">
			<%=Model.Text%>
			<div class="sidelist">
				...
			</div>
		</div>
	</div>
<% } %>
</div>