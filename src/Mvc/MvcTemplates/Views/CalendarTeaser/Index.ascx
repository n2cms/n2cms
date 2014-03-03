<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarTeaser.ascx.cs"
	Inherits="N2.Web.Mvc.ContentViewUserControl<CalendarTeaserModel, CalendarTeaser>" %>
<div class="uc">
	<%= ContentHtml.DisplayContent(m => m.Title)%>
	<div class="box calendarTeaser ">
		<div class="inner">
			<%if (Model.Events.Count > 0){%>
				<ul class="sidelist">
				<%foreach (var @event in Model.Events){%>
					<li class="item">
                        <a href="<%=@event.Url%>">
                            <span class="date"><%= @event.EventDateString %></span>
                            <%=@event.Title%>
                        </a>
					</li>
				<%} %>
				</ul>
			<%}else{%>
				<p><%= GetLocalResourceObject("NoItemsFound") %></p>
			<%} %>
		</div>
	</div>
</div>