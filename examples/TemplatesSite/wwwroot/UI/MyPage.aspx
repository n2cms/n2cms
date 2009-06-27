<%@ Page MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.master" Language="C#" AutoEventWireup="true" CodeBehind="MyPage.aspx.cs" Inherits="MyProject.UI.MyPage" %>
<asp:Content ContentPlaceHolderID="Content" runat="server">
	<n2:Display PropertyName="Title" runat="server" />
	<n2:Display PropertyName="Text" runat="server" />
	<n2:Display PropertyName="Author" runat="server">
		<HeaderTemplate><hr /></HeaderTemplate>
	</n2:Display>
</asp:Content>
