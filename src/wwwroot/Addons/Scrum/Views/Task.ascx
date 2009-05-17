<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Task.ascx.cs" Inherits="N2.Templates.Scrum.UI.Task" %>
<h4 style="color:<%= CurrentItem.Color %>"><%= CurrentItem.Title %></h4>
<p><%= CurrentItem.Story %></p>
<p><%= CurrentItem.Description %></p>
<n2:Zone ZoneName="SubTask" runat="server" />