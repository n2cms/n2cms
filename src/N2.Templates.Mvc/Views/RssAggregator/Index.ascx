<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RssAggregator>" %>
<% using (Html.BeginAsyncAction("list")) { %>
<div class="uc">
	<h4><%=Model.Title %></h4>
	<div class="box">
		<div class="inner">
			<%=Model.Text%>
			<div class="sidelist">
				...
			</div>
		</div>
	</div>
</div>
<% } %>
