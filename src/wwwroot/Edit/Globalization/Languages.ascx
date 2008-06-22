<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Languages.ascx.cs" Inherits="N2.Edit.Globalization.Languages" %>
<asp:Repeater ID="rptLang" runat="server" DataSource='<%# DataSource %>'>
	<ItemTemplate>
		<td class="item">
		    <%# (bool)Eval("IsNew") || Eval("ExistingItem") == Eval("Language") ? null : Html.Radio((string)Eval("Language.LanguageCode"), Eval("ExistingItem.ID").ToString()).Attr("style", "float:left") %>
			<asp:HyperLink ID="hlEdit" NavigateUrl='<%# Eval("EditUrl") %>' CssClass='<%# GetClass() %>' runat="server" ToolTip='<%# Eval("ExistingItem.Updated") %>'>
				<asp:Literal ID="ltCreateNew" runat="server" Text='create new' Visible='<%# (bool)Eval("IsNew") %>' meta:resourceKey="ltCreateNew" />
				
				<asp:Image ID="imgNew" ImageUrl='<%# Eval("ExistingItem.IconUrl")%>' AlternateText="icon" runat="server" Visible='<%# !(bool)Eval("IsNew") %>'/>
				<asp:Literal ID="ltExisting" runat="server" Text='<%# Eval("ExistingItem.Title") ?? "(untitled)" %>' meta:resourceKey="ltExisting" Visible='<%# !(bool)Eval("IsNew") %>'/>
			</asp:HyperLink>
		</td>
	</ItemTemplate>
</asp:Repeater>
