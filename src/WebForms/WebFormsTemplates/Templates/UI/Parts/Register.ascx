<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Register.ascx.cs" Inherits="N2.Templates.UI.Parts.Register" %>
<asp:CustomValidator ID="cvError" runat="server" EnableViewState="false" />
<asp:CreateUserWizard ID="UserCreator" runat="server">
	<wizardsteps>
		<asp:CreateUserWizardStep runat="server"/>
		<asp:CompleteWizardStep runat="server"/>
	</wizardsteps>
</asp:CreateUserWizard>