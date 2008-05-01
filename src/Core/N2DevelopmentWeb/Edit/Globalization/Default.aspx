<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Globalization._Default" %>
<%@ Register TagPrefix="lang" TagName="Languages" Src="Languages.ascx" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Styles.css" type="text/css" />
</asp:Content>
<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" AccessKey="C" meta:resourceKey="hlCancel">Cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
	<div class="languages">
		<table class="gv"><thead>
			<asp:Repeater runat="server" DataSource='<%# gateway.GetEditTranslations(SelectedItem, true) %>'>
				<HeaderTemplate><tr class="th"></HeaderTemplate>
				<ItemTemplate>
					<td>
						<asp:Image ImageUrl='<%# Eval("FlagUrl") %>' AlternateText="flag" runat="server" /> <%# Eval("Language.LanguageTitle") %> (<%# Eval("Language.LanguageCode") %>)
					</td>
				</ItemTemplate>	
				<FooterTemplate></tr></FooterTemplate>
			</asp:Repeater>

			<tr><lang:Languages runat="server" DataSource='<%# gateway.GetEditTranslations(SelectedItem, true) %>' /></tr>
		</thead>
		<tbody>
			<asp:Repeater runat="server" DataSource="<%# SelectedItem.GetChildren() %>">
				<ItemTemplate>
					<tr class="<%# Container.ItemIndex % 2 == 1 ? "alt" : "" %>">
						<lang:Languages runat="server" DataSource='<%# gateway.GetEditTranslations((N2.ContentItem)Container.DataItem, true) %>' />
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</tbody></table>
	</div>
</asp:Content>