<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentVersions.ascx.cs" Inherits="N2.Management.Content.Versions.RecentVersions" %>
<%@ Register TagPrefix="n2" Namespace="N2.Web.UI.WebControls" Assembly="N2" %>
<n2:Box ID="boxVersions" HeadingText="Recenet Versions" CssClass="box versionBox" runat="server" meta:resourceKey="boxVersions">
	<table class="gv">
		<thead>
			<tr><td><%= GetLocalResourceObject("bfVersion.HeaderText") %></td><td><%= GetLocalResourceObject("bfSavedBy.HeaderText") %></td><td></td></tr>
		</thead>
		<tbody>
		<% foreach(var version in Versions){ %>
			<tr class="<%= version.ID == CurrentItem.ID ? "current" : "" %>">
				<td title="<%= version.State %>" class="<%= version.State %> State"><%= version.VersionIndex %></td>
				<td><%= version.SavedBy %></td>
				<td><%= version.Info %></td>
			</tr>
		<% } %>
		</tbody>
	</table>

	<asp:HyperLink ID="hlMoreVersions" CssClass="moreVersions" NavigateUrl="<%# VersionsUrl %>" Text="More versions &raquo;" Visible="<%# ShowMoreVersions %>" runat="server" meta:resourceKey="hlMoreVersions"/>
</n2:Box>
<n2:Box ID="boxActivity" HeadingText="Recent Activity" CssClass="box versionBox" runat="server" Visible="<%# ShowActivities %>" meta:resourceKey="boxActivity">
	<table class="gv">
		<thead>
			<tr><td><%= GetLocalResourceObject("bfOperation.HeaderText") %></td><td><%= GetLocalResourceObject("bfBy.HeaderText") %></td><td></td></tr>
		</thead>
		<tbody>
		<% foreach(var activity in Activities){ %>
			<tr>
				<td><%= activity.Operation %></td>
				<td><%= activity.PerformedBy %></td>
				<td><%= activity.AddedDate %></td>
			</tr>
		<% } %>
		</tbody>
	</table>
</n2:Box>
