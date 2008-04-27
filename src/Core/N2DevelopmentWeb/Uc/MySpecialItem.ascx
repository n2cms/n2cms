<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MySpecialItem.ascx.cs" Inherits="N2DevelopmentWeb.Uc.MySpecialItem" %>
<li style="border:solid <%# IsHighlighted ? "3" : "1" %>px green">
    <%# DateTime.Now.ToString("hh:MM:ss") %><br />
    <asp:Label runat="server" ID="label1" Text="<%# CurrentData.Text %>" />
    <asp:Label runat="server" ID="label2" Text="0" />
	<asp:Button ID="btnPostback" runat="server" Text="PostBack" OnClick="btnPostback_Click" />
</li>