<%@ Page Language="C#" MasterPageFile="../Framed.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Security.Default" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:LinkButton ID="btnSave" runat="server" CssClass="command" OnCommand="btnSave_Command">save</asp:LinkButton>
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command">cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Outside" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content" runat="server">
    <asp:CustomValidator ID="cvSomethingSelected" runat="server" Display="Dynamic" ErrorMessage="At least one role must be selected" OnServerValidate="cvSomethingSelected_ServerValidate" />
    <asp:CheckBoxList ID="cblAllowedRoles" runat="server" DataSourceID="odsRoles" CssClass="cbl" OnSelectedIndexChanged="cblAllowedRoles_SelectedIndexChanged" OnDataBound="cblAllowedRoles_DataBound" />
    <asp:ObjectDataSource ID="odsRoles" runat="server" TypeName="System.Web.Security.Roles" SelectMethod="GetAllRoles" />
</asp:Content>
