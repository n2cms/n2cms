<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ModelViewUserControl<StatisticsModel, Statistics>" %>

<div class="uc statistics">
	<h4><%= CurrentItem.Title %></h4>
	<div class="field">
		<label># of pages:</label>
		<span><%=Model.NumberOfPages%></span>
	</div>
	<div class="field">
		<label>Total # of items:</label>
		<span><%=Model.NumberOfItems%></span>
	</div>
	<div class="field">
		<label>Pages served (since startup):</label>
		<span><%=Model.PagesServed == null ? "unknown" : Model.PagesServed.ToString()%></span>
	</div>
	<div class="field">
		<label>Versions per item:</label>
		<span><%=String.Format("{0:F2}", Model.VersionsPerItem)%></span>
	</div>
	<div class="field">
		<label># of changes last week:</label>
		<span><%=Model.ChangesLastWeek%></span>
	</div>
	<div>
		<label>Latest changes:</label>
	</div>
	<div>
		<%foreach(var changedItem in Model.LatestChanges){%>
			<div>
				<a href="<%=changedItem.Url%>" title="<%=changedItem.SavedBy + ", "+ changedItem.Updated%>"><%=changedItem.Title%></a>
			</div>
		<%}%>
	</div>
</div>