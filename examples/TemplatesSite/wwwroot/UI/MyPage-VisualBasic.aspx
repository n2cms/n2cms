<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="MyPage-VisualBasic.aspx.vb" Inherits="MyProject.MyPage_VisualBasic" %>
<asp:Content ContentPlaceHolderID="Content" runat="server">
	<n2:Display ID="Display1" PropertyName="Title" runat="server" />
	<n2:Display ID="Display2" PropertyName="Text" runat="server" />
	<n2:Display ID="Display3" PropertyName="Author" runat="server">
		<HeaderTemplate><hr /></HeaderTemplate>
	</n2:Display>
</asp:Content>
