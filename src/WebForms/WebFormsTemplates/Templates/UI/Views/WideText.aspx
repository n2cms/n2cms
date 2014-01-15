<%@ Page Language="C#" MasterPageFile="../Layouts/Top+SubMenu.master" AutoEventWireup="true" Inherits="N2.Templates.UI.Views.Text" Title="Untitled Page" %>
<asp:Content ContentPlaceHolderID="ContentAndSidebar" runat="server">
    <n2:EditableDisplay ID="dti" PropertyName="Title" SwallowExceptions="true" runat="server" />
    <n2:EditableDisplay ID="dte" PropertyName="Text" SwallowExceptions="true" runat="server">
		<HeaderTemplate><div id="textContent"></HeaderTemplate>
		<FooterTemplate></div></FooterTemplate>
    </n2:EditableDisplay>
</asp:Content>
