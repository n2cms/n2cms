<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Languages.ascx.cs" Inherits="N2.Edit.Globalization.Languages" %>
<asp:Repeater ID="rptLang" runat="server" DataSource='<%# DataSource %>'>
	<ItemTemplate>
		<td>
			<asp:HyperLink ID="HyperLink1" NavigateUrl='<%# Eval("EditUrl") %>' CssClass='<%# GetClass() %>' runat="server">
				<%# Eval("ExistingItem.Title") ?? "Create" %>
			</asp:HyperLink>
		</td>
	</ItemTemplate>
</asp:Repeater>
