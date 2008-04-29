<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopMenu.ascx.cs" Inherits="N2.Templates.UI.Layouts.Parts.TopMenu" %>
<n2:Menu ID="tm" BranchMode="false" runat="server" MaxLevels='2' CssClass="topMenu menu" />

<div class="languageMenu">
	<asp:Repeater ID="rptLanguages" runat="server">
		<ItemTemplate>
			<a href="<%# Eval("Page.Url") %>" class="language<%# Eval("Page") == CurrentPage ? " current" : "" %>" title="<%# Eval("Language.LanguageTitle") %>">
				<asp:Image ImageUrl='<%# Eval("Language.FlagUrl") %>' AlternateText='<%# Eval("Language.LanguageTitle") %>' runat="server" />
			</a>
		</ItemTemplate>
	</asp:Repeater>
</div>