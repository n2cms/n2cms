<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
	<h1 title="<%= Html.CurrentPage().Title %>"><%= Html.CurrentItem().Title %></h1>
	<fieldset><legend>Route values</legend>
		<%= Html.Partial("Dictionary", ViewContext.RouteData.Values) %>
	</fieldset>
		
	<fieldset><legend>Route tokens</legend>
		<%= Html.Partial("Dictionary", ViewContext.RouteData.DataTokens) %>
	</fieldset>
    
	<fieldset><legend>Urls</legend>
		<%= Html.Partial("Urls") %>		
	</fieldset>
		
	<fieldset><legend>TestParts</legend>
		<% using (Html.BeginForm("Add", null)){ %>
			<input name="name" />
			<input type="submit" value="Add" />
		<% } %>
		<%= Html.DroppableZone("TestParts") %>
	</fieldset>
</asp:Content>