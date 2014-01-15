<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Teaser.ascx.cs" Inherits="N2.Templates.UI.Parts.Teaser" %>
<n2:EditableDisplay ID="dt" runat="server" PropertyName="Title" />
<n2:Box runat="server">
    <asp:HyperLink ID="hl" runat="server" NavigateUrl='<%$ CurrentItem: LinkUrl %>'>
        <n2:EditableDisplay runat="server" PropertyName="LinkText" />
    </asp:HyperLink>
</n2:Box>