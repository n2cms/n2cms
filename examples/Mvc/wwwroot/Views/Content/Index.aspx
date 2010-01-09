<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="ViewPage<ContentPage>" %>
<%@ Import Namespace="MvcTest.Models"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h1><%= Model.Title %></h1>
<%= Model.Text %>
</asp:Content>
