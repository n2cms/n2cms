<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.Master" CodeBehind="AdaptiveItem.aspx.cs" Inherits="N2.Addons.UITests.UI.AdaptiveItem" %>
<asp:Content ID="Content9" ContentPlaceHolderID="PostContent" runat="server">
    State: <%= CurrentItem.State %>
	<ul>
	<% foreach (string file in System.IO.Directory.GetFiles(Server.MapPath("~/Addons/UITests/UI"), "*.aspx")) { %>
		<li><a href="<%= N2.Web.Url.Parse(CurrentPage.Url).AppendSegment(System.IO.Path.GetFileNameWithoutExtension(file)) %>"><%= System.IO.Path.GetFileNameWithoutExtension(file)%></a></li>
	<% } %>
	</ul>
</asp:Content>
