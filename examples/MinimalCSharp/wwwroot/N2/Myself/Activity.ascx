<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Activity.ascx.cs" Inherits="N2.Management.Myself.Activity" %>

<div class="uc">
	<h4 class="header"><%= CurrentItem.Title %></h4>
	<div class="box">
	<asp:Repeater ID="rptLatestChanges" runat="server">
		<HeaderTemplate>
	<table class="data">
		<thead><tr><th>Changed item</th><th>Saved by</th><th>Last updated</th></tr></thead>
		<tbody>
		</HeaderTemplate>
		<ItemTemplate>
			<tr><td>
				<asp:HyperLink NavigateUrl='<%# ResolveUrl(Eval("Url")) %>' runat="server"><asp:Image ImageUrl='<%# ResolveUrl(Eval("IconUrl")) %>' runat="server" /><%# Eval("Title") %></asp:HyperLink>
			</td><td>
				<%# Eval("SavedBy") %>
			</td><td>
				<%# Eval("Updated") %>
			</td></tr>
		</ItemTemplate>
		<FooterTemplate>
		</tbody>
	</table>
		</FooterTemplate>
	</asp:Repeater>
	</div>
</div>