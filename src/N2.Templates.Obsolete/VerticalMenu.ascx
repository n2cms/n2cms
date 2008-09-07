<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VerticalMenu.ascx.cs" Inherits="N2.Templates.UI.Parts.VerticalMenu" %>
<n2:h4 ID="h" runat="server" />
<n2:Box runat="server">
    <n2:Menu id="m" runat="server" StartLevel='<%$ CurrentItem: StartingDepth %>' />
</n2:Box>