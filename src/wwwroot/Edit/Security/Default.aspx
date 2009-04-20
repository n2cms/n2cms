<%@ Page Language="C#" MasterPageFile="../Framed.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Security.Default" Title="Untitled Page" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>
<asp:Content ID="ch" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
    <n2:OptionsMenu id="om" runat="server">
        <asp:LinkButton ID="btnSave" runat="server" CssClass="command" OnCommand="btnSave_Command" meta:resourcekey="btnSaveResource1">Save</asp:LinkButton>
        <asp:LinkButton ID="btnSaveRecursive" runat="server" CssClass="command" OnCommand="btnSaveRecursive_Command" meta:resourcekey="btnSaveRecursiveResource1">Save whole branch</asp:LinkButton>
    </n2:OptionsMenu>
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" meta:resourcekey="hlCancelResource1">Cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
    <asp:CustomValidator ID="cvSomethingSelected" runat="server" Display="Dynamic" CssClass="validator" ErrorMessage="At least one role must be selected" OnServerValidate="cvSomethingSelected_ServerValidate" meta:resourcekey="cvSomethingSelectedResource1" />
    
    <asp:CheckBox ID="cbEveryone" runat="server" CssClass="cb" AutoPostBack="true" oncheckedchanged="cbEveryone_CheckedChanged" />
    
    <hr />

    <asp:CheckBoxList ID="cblAllowedRoles" runat="server" CssClass="cbl" OnSelectedIndexChanged="cblAllowedRoles_SelectedIndexChanged" OnDataBound="cblAllowedRoles_DataBound" meta:resourcekey="cblAllowedRolesResource1" />
</asp:Content>
