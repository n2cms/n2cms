<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="true" CodeBehind="LanguageMenu.ascx.cs" Inherits="N2.Edit.Globalization.LanguageMenu" %>
<asp:PlaceHolder ID="plhNew" runat="server" Visible='<%# CreatingNew %>'>
	<asp:HyperLink Enabled="false" ID="hlNew" runat="server" CssClass="command" NavigateUrl='<%# "Default.aspx?selected=" + Server.UrlEncode(Selection.SelectedItem.Path) %>' ToolTip="<%# CurrentLanguage.LanguageCode %>">
		<asp:Image ID="imgNew" ImageUrl='<%# CurrentLanguage.FlagUrl %>' AlternateText="flag" runat="server" />
		<%# CurrentLanguage.LanguageTitle %>
	</asp:HyperLink>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" Visible='<%# !CreatingNew %>'>
	<n2:OptionsMenu id="om" runat="server">
		<asp:HyperLink runat="server" CssClass="command plain" NavigateUrl='<%# "Default.aspx?selected=" + Server.UrlEncode(Selection.SelectedItem.Path) %>' ToolTip="<%# CurrentLanguage.LanguageCode %>">
			<asp:Image ID="imgCurrent" ImageUrl='<%# CurrentLanguage.FlagUrl %>' AlternateText="flag" runat="server" />
			<%# CurrentLanguage.LanguageTitle %>
		</asp:HyperLink>
		<asp:Repeater runat="server" id="rptLanguages">
			<ItemTemplate>
				<asp:HyperLink NavigateUrl='<%# Eval("EditUrl") %>' CssClass="command plain" runat="server" ToolTip='<%# Eval("Language.LanguageCode") %>'>
					<asp:Image ImageUrl='<%# Eval("FlagUrl") %>' AlternateText="flag" runat="server" />
					<%# Eval("Language.LanguageTitle") %>
				</asp:HyperLink>
			</ItemTemplate>
		</asp:Repeater>
	</n2:OptionsMenu>
</asp:PlaceHolder>
