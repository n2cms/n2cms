<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Top+SubMenu.master" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewPage<News>" %>

<asp:Content ContentPlaceHolderID="TextContent" runat="server">
	<n2:EditableDisplay ID="edt" PropertyName="Title" runat="server" />
	<span class="date"><%= CurrentItem.Published %></span>
	<n2:EditableDisplay PropertyName="Introduction" runat="server">
		<HeaderTemplate><p class="introduction"></HeaderTemplate>
		<FooterTemplate></p></FooterTemplate>
	</n2:EditableDisplay>
	<n2:EditableDisplay PropertyName="Text" runat="server" />
</asp:Content>