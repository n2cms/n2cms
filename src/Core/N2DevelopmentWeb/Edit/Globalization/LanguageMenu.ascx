<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LanguageMenu.ascx.cs" Inherits="N2.Edit.Globalization.LanguageMenu" %>
<n2:OptionsMenu id="om" runat="server">
	<asp:HyperLink runat="server" CssClass="command" NavigateUrl='<%# "~/Edit/Globalization/Default.aspx?selected=" + Server.UrlEncode(SelectedItem.Path) %>'>Languages</asp:HyperLink>
	<asp:Repeater runat="server" id="rptLanguages">
		<ItemTemplate>
			<a href="#" class="command">
				<asp:Image ImageUrl='<%# Eval("FlagUrl") %>' AlternateText="flag" runat="server" />
				<%# Eval("LanguageTitle") %>
			</a>
		</ItemTemplate>
	</asp:Repeater>
</n2:OptionsMenu>