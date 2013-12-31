<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.Master" AutoEventWireup="true" Inherits="N2.Templates.Web.UI.TemplatePage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
	<script type="text/javascript">
		window.document.title = "This was inserted into the head content area";
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Top" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">Top</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="PageWrapper" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">PageWrapper</div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTop" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">PageTop</div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="Menu" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">Menu</div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentAndSidebar" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">ContentAndSidebar</div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="PreContent" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">PreContent</div>
</asp:Content>
<asp:Content ID="Content8" ContentPlaceHolderID="Content" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">Content</div>
</asp:Content>
<asp:Content ID="Content9" ContentPlaceHolderID="TextContent" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">TextContent</div>
</asp:Content>
<asp:Content ID="Content10" ContentPlaceHolderID="PostContent" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">PostContent</div>
</asp:Content>
<asp:Content ID="Content11" ContentPlaceHolderID="Sidebar" runat="server">
	<div style="border:solid 1px red;background-color:Yellow">Sidebar</div>
</asp:Content>
