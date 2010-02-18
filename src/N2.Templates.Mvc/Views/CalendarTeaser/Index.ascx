<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarTeaser.ascx.cs"
	Inherits="N2.Web.Mvc.N2ModelViewUserControl<CalendarTeaserModel, CalendarTeaser>" %>

<%= ContentHtml.DisplayContent(m => m.Title)%>
<div class="box calendarTeaser">
	<div class="inner">
		<%if (Model.Events.Count > 0){%>
			<ul>
			<%foreach (var @event in Model.Events){%>
				<li><a href="<%=@event.Url%>"><%=@event.Title%></a></li>
			<%} %>
			</ul>
		<%}else{%>
			<p><%= GetLocalResourceObject("NoItemsFound") %></p>
		<%} %>
	</div>
</div>
<style type="text/css">
	@import "<%=ResolveUrl("~/Content/Css/Calendar.css")%>";
</style>