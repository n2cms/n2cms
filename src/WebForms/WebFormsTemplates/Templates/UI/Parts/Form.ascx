<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Form.ascx.cs" Inherits="N2.Templates.UI.Parts.Form" %>
<asp:MultiView ID="mv" runat=server ActiveViewIndex="0">
    <asp:View ID="View1" runat="server">
		<n2:Display runat="server" PropertyName="Title" />
		<n2:Display runat="server" PropertyName="IntroText" />
        <div class="inputForm">
			<asp:ValidationSummary ID="vsQuestions" runat="server" CssClass="vs" HeaderText="The form isn't complete. Please review the following and try again." ValidationGroup="Form" meta:resourcekey="vsQuestions" />
            <n2:DroppableZone id="zq" runat="server" ZoneName="Questions" />
        </div>
        <asp:Button Text="Send" ID="btnSubmit" runat="server" OnCommand="btnSubmit_Command" meta:resourcekey="btnSubmitResource1" />
    </asp:View>
    <asp:View ID="View2" runat="server" EnableViewState="false">
        <n2:Display runat="server" PropertyName="SubmitText" />
    </asp:View>
</asp:MultiView>