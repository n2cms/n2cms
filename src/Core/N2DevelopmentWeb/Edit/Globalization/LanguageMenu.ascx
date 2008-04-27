<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="true" CodeBehind="LanguageMenu.ascx.cs" Inherits="N2.Edit.Globalization.LanguageMenu" %>
<n2:OptionsMenu id="om" runat="server">
	<asp:HyperLink runat="server" CssClass="command" NavigateUrl='<%# "~/Edit/Globalization/Default.aspx?selected=" + Server.UrlEncode(SelectedItem.Path) %>'>Languages</asp:HyperLink>
	<asp:Repeater runat="server" id="rptLanguages">
		<ItemTemplate>
			<asp:HyperLink NavigateUrl='<%# Eval("EditUrl") %>' CssClass="command" runat="server">
				<asp:Image ImageUrl='<%# Eval("FlagUrl") %>' AlternateText="flag" runat="server" />
				<%# Eval("Language.LanguageTitle") %>
				(<%# Eval("Language.LanguageCode") %>)
			</asp:HyperLink>
		</ItemTemplate>
	</asp:Repeater>
</n2:OptionsMenu>