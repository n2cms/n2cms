<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Index</title>
</head>
<body>
    <div>
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
    </div>
</body>
</html>
