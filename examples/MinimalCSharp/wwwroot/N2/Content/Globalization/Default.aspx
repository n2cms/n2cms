<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Globalization._Default" %>
<%@ Register TagPrefix="lang" TagName="Languages" Src="Languages.ascx" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
<%@ Import Namespace="N2.Web" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="../../Resources/Css/Globalization.css" type="text/css" />
</asp:Content>
<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
    <edit:CancelLink ID="hlCancel" runat="server" meta:resourceKey="hlCancel">Cancel</edit:CancelLink>
</asp:Content>
<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
    <asp:CustomValidator Text="Globalization is not enabled." ID="cvGlobalizationDisabled" meta:resourceKey="cvGlobalizationDisabled" Display="Dynamic" CssClass="validator info" runat="server" />
    <asp:CustomValidator Text="This page cannot be translated." ID="cvOutsideGlobalization" meta:resourceKey="cvOutsideGlobalization" Display="Dynamic" CssClass="validator info" runat="server" />
	<asp:CustomValidator Text="Select at least two items to associate." ID="cvAssociate" meta:resourceKey="cvAssociate" runat="server" CssClass="validator info" Display="Dynamic" />
	<asp:CustomValidator Text="Couldn't enable globalization. Please configure it manually in the web configuration." ID="cvEnable" meta:resourceKey="cvEnable" runat="server" CssClass="validator info" Display="Dynamic" />
	<asp:CustomValidator Text="No language roots available. Please add one or more start pages and set the language on the site tab." ID="cvLanguageRoots" meta:resourceKey="cvLanguageRoots" runat="server" CssClass="validator info" Display="Dynamic" />
	<asp:CustomValidator Text="Cannot associate language roots. They are assumed to be translations of each other." ID="cvAssociateLanguageRoots" meta:resourceKey="cvAssociateLanguageRoots" runat="server" CssClass="validator info" Display="Dynamic" />
	
	<asp:Panel ID="pnlLanguages" runat="server" CssClass="languages">
		<table class="gv">
		    <thead>
			    <asp:Repeater runat="server" DataSource='<%# GetTranslations(Selection.SelectedItem) %>'>
				    <HeaderTemplate><tr class="th"><td></td></HeaderTemplate>
				    <ItemTemplate>
					    <td title='<%# Eval("Language.LanguageCode") %>'><asp:Image ImageUrl='<%# Eval("FlagUrl") %>' AlternateText='<%# Eval("Language.LanguageCode", "{0} flag") %>' runat="server" /> <%# Eval("Language.LanguageTitle") %></td>
				    </ItemTemplate>	
				    <FooterTemplate></tr></FooterTemplate>
			    </asp:Repeater>

			    <tr class="selected">
			        <td>
						<% if (Selection.SelectedItem.Parent != null){ %>
							<a href="Default.aspx?selected=<%# Selection.SelectedItem.Parent.Path %>"><img src="../../Resources/icons/bullet_toggle_minus.png" class="up" /></a>
						<% } %>
					</td>
			        <lang:Languages runat="server" DataSource='<%# GetTranslations(Selection.SelectedItem) %>' />
                </tr>
		    </thead>
			<asp:Repeater runat="server" DataSource="<%# GetChildren(true) %>">
				<HeaderTemplate><tbody></HeaderTemplate>
				<ItemTemplate>
					<tr class="<%# Container.ItemIndex % 2 == 1 ? "alt" : "" %> i<%# Container.ItemIndex %>">
					    <td>
							<asp:HyperLink runat="server" Visible="<%# ((N2.ContentItem)Container.DataItem).GetChildren().Count > 0 %>" href='<%# Eval("Path", "Default.aspx?selected={0}") %>'><img src="../../Resources/icons/bullet_toggle_plus.png" class="down" /></asp:HyperLink>
						</td>
						<lang:Languages runat="server" DataSource='<%# GetTranslations((N2.ContentItem)Container.DataItem) %>' />
					</tr>
				</ItemTemplate>
				<FooterTemplate></tbody></FooterTemplate>
			</asp:Repeater>
			<asp:Repeater runat="server" DataSource="<%# GetChildren(false) %>">
			    <HeaderTemplate><tbody></HeaderTemplate>
				<ItemTemplate>
					<tr class="<%# Container.ItemIndex % 2 == 1 ? "alt" : "" %> i<%# Container.ItemIndex %>">
					    <td>
							<asp:HyperLink runat="server" Visible="<%# ((N2.ContentItem)Container.DataItem).GetChildren().Count > 0 %>" href='<%# Eval("Path", "Default.aspx?selected={0}") %>'><img src="../../Resources/icons/bullet_toggle_plus.png" class="down" /></asp:HyperLink>
						</td>
						<lang:Languages runat="server" DataSource='<%# GetTranslations((N2.ContentItem)Container.DataItem) %>' />
					</tr>
				</ItemTemplate>
				<FooterTemplate></tbody></FooterTemplate>
			</asp:Repeater>
		</table>
		<asp:Label runat="server" Text="Selected: " id="lblSelected" meta:resourceKey="lblSelected" />
		<asp:Button ID="btnAssociate" runat="server" Text="Associate" OnClick="btnAssociate_Click" meta:resourceKey="btnAssociate" />
		<asp:Button ID="btnUnassociate" runat="server" Text="Unassociate" OnClick="btnUnassociate_Click" meta:resourceKey="btnUnassociate" />
	</asp:Panel>
	<asp:Button ID="btnEnable" runat="server" Text="Enable Globalization" OnClick="btnEnable_Click" meta:resourceKey="btnEnable" />
</asp:Content>