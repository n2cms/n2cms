<%@ Page MasterPageFile="../Layouts/ThreeColumns.Master" Language="C#" AutoEventWireup="true" CodeBehind="MyPage.aspx.cs" Inherits="MyProject.UI.MyPage" %>
<asp:Content ContentPlaceHolderID="Content" runat="server">
	<h1><%= CurrentPage.Title %></h1>
	<n2:Display PropertyName="Text" runat="server" />
</asp:Content>
