<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Text.ascx.cs" Inherits="N2.Templates.UI.Parts.Text" %>
<n2:H4 Text="<%$ CurrentItem: Title %>" Visible="<%$ HasValue: Title %>" runat="server" />
<n2:EditableDisplay runat="server" id="t" PropertyName="Text" />