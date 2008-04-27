<%@ Page Language="C#" MasterPageFile="../Framed.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Globalization._Default" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Styles.css" type="text/css" />
</asp:Content>
<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" AccessKey="C" meta:resourceKey="hlCancel">Cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
	<div class="languages">
		<asp:Repeater runat="server" ID="rptLanguages">
			<HeaderTemplate><table><tbody></HeaderTemplate>
			<ItemTemplate>
				<tr><td>
				<asp:HyperLink NavigateUrl='<%# Eval("EditUrl") %>' CssClass='<%# GetClass() %>' runat="server">
					<%# Eval("ExistingItem.Title") ?? "Create" %> 
				</asp:HyperLink>
				</td><td>
					<span>
						<asp:Image ID="Image1" ImageUrl='<%# Eval("FlagUrl") %>' AlternateText="flag" runat="server" />
						<%# Eval("Language.LanguageTitle") %>
						(<%# Eval("Language.LanguageCode") %>)
					</span>
				</td></tr>
			</ItemTemplate>
			<FooterTemplate></tbody></table></FooterTemplate>
		</asp:Repeater>
	</div>
</asp:Content>