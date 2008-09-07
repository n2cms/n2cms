<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ControlPanel.ascx.cs" Inherits="N2.Templates.UI.Parts.ControlPanel" %>
<n2:Display ID="dt" PropertyName="Title" runat="server" />
<n2:Box runat="server">
    <n2:DragDropControlPanel ID="cp" runat="server" StyleSheetUrl="~/Templates/Secured/Edit.css" />
</n2:Box>
