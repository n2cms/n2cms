<%@ Page MasterPageFile="../../Layouts/Empty.Master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.Form.UI.Default" %>
<asp:Content ContentPlaceHolderID="Content" runat="server">
    <n2:Path ID="p1" runat="server" />
    <n2:EditableDisplay ID="dti" PropertyName="Title" runat="server" />
    <n2:EditableDisplay ID="dte" PropertyName="Text" runat="server" />
    <n2:Display runat="server" PropertyName="Form" />
</asp:Content>
