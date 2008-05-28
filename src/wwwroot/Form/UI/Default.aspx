<%@ Page MasterPageFile="~/Layouts/Top+SubMenu.Master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.Form.UI.Default" %>
<asp:Content ID="cpc" ContentPlaceHolderID="PostContent" runat="server">
	<n2:Display ID="df" runat="server" PropertyName="Form" />
</asp:Content>