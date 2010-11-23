﻿<%@ Import Namespace="N2.Edit.Wizard.Items" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Wizards.ascx.cs" Inherits="N2.Management.Myself.Wizards" %>

<div class="uc">
	<h4 class="header"><%= CurrentItem.Title %></h4>
	<div class="box">

	<n2:Repeater ID="rptLocations" runat="server">
		<HeaderTemplate><table class="data"><thead><tr><th colspan="2">Wizard</th><th>Location</th></tr></thead><tbody></HeaderTemplate>
		<ItemTemplate>
			<tr><td>
				<asp:HyperLink ID="hlNew" 
					NavigateUrl='<%# GetEditUrl((MagicLocation)Container.DataItem) %>' 
					ToolTip='<%# Eval("ToolTip") %>' runat="server" 
					meta:resourcekey="hlNewResource1"><asp:Image ID="imgIco" ImageUrl='<%# ResolveUrl(Eval("IconUrl")) %>' CssClass="icon" runat="server" meta:resourcekey="imgIcoResource1" />
<%# Eval("Title") %></asp:HyperLink>
			</td><td>
				<%# Eval("Description") %>
			</td><td>
				<a href="<%# Eval("Location.Url") %>">
					<asp:Image runat=server ImageUrl='<%# ResolveUrl(Eval("Location.IconUrl")) %>' 
						meta:resourcekey="ImageResource1" />
					<%# Eval("Location.Title") %>
				</a>
			</td></tr>
		</ItemTemplate>
		<EmptyTemplate>
			<asp:Label runat="server" ID="lblNoItems" meta:resourcekey="lblNoItems" Text="No locations added." />
		</EmptyTemplate>
		<FooterTemplate></tbody></table></FooterTemplate>
	</n2:Repeater>

</div></div>