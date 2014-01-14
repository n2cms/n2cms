<%@ Page Title="" Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="N2.Engine.Globalization" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="h" runat="server">
</head>
<body>
<fieldset>
    <legend>Default</legend>

    <% var lg = N2.Context.Current.Resolve<ILanguageGateway>(); %>
    <% foreach (var language in lg.GetAvailableLanguages()) { %>
    <img src="<%= ResolveUrl(language.FlagUrl) %>" />
    <%= language.LanguageTitle %>
    (<%= language.LanguageCode %>)
    <% } %>
</fieldset>

<% var mlgs = N2.Context.Current.Resolve<LanguageGatewaySelector>(); %>
<fieldset>
    <legend>For current site</legend>

    <% foreach (var language in mlgs.GetLanguageGateway().GetAvailableLanguages()) { %>
    <img src="<%= ResolveUrl(language.FlagUrl) %>" />
    <%= language.LanguageTitle %>
    (<%= language.LanguageCode %>)
    <% } %>
</fieldset>

<fieldset>
    <legend>All sites</legend>

    <% foreach (var site in N2.Context.Current.Resolve<N2.Web.IHost>().Sites) { %>

    <fieldset>
        <legend><%= site %></legend>

        <% foreach (var language in mlgs.GetLanguageGateway().GetAvailableLanguages()) { %>
        <img src="<%= ResolveUrl(language.FlagUrl) %>" />
        <%= language.LanguageTitle %>
        (<%= language.LanguageCode %>)
        <% } %>
    </fieldset>
    <% } %>
</fieldset>

</body>
</html>
