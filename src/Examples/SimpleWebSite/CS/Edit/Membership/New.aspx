<%@ Page Language="C#" MasterPageFile="../Framed.Master" AutoEventWireup="true" CodeBehind="New.aspx.cs" Inherits="N2.Edit.Membership.New" Title="New user" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/membership.css" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink runat="server" NavigateUrl="Users.aspx" CssClass="command">cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Outside" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
    <asp:CreateUserWizard ID="createUserWizard" runat="server" StartNextButtonText="Next" OnCreatedUser="createUserWizard_CreatedUser" OnContinueButtonClick="createUserWizard_FinishButtonClick" LoginCreatedUser="false">
        <WizardSteps>
            <asp:CreateUserWizardStep ID="cuwsCreate" runat="server">
                <ContentTemplate>
                    <div>
                        <asp:Label ID="lblUserName" runat="server" AssociatedControlID="UserName">User name</asp:Label>
                        <asp:TextBox ID="UserName" runat="server" />
                    </div>
                    <div>
                        <asp:Label ID="lblPassword" runat="server" AssociatedControlID="Password">Password</asp:Label>
                        <asp:TextBox ID="Password" runat="server" />
                    </div>
                    <div>
                        <asp:Label ID="lblEmail" runat="server" AssociatedControlID="Email">Email</asp:Label>
                        <asp:TextBox ID="Email" runat="server" />
                    </div>
                    <asp:TextBox ID="Question" runat="server" Visible="false" Text="<%$ Code: Guid.NewGuid() %>" />
                    <asp:TextBox ID="Answer" runat="server" Visible="false" Text="<%$ Code: Guid.NewGuid() %>" />
                    
                    <div>
                        <asp:Label ID="lblRoles" runat="server" AssociatedControlID="cblRoles">Roles</asp:Label>
                        <asp:CheckBoxList ID="cblRoles" runat="server" CssClass="cbl" DataSourceID="odsRoles" />
                        <asp:ObjectDataSource ID="odsRoles" runat="server" TypeName="System.Web.Security.Roles" SelectMethod="GetAllRoles" />
                    </div>
                </ContentTemplate>
            </asp:CreateUserWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
</asp:Content>
