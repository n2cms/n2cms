<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LanguageSwitcher.ascx.cs" Inherits="Languaging_LanguageSwitcher" %>
<h4><%= CurrentItem.Title %></h4>
<div class="uc">
    <asp:DropDownList ID="ddlLanguages" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLanguages_SelectedIndexChanged" />
</div>