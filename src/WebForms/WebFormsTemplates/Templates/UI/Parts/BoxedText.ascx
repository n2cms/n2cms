<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BoxedText.ascx.cs" Inherits="N2.Templates.UI.Parts.BoxedText" %>
<n2:hn Text="<%$ CurrentItem: Title %>" Visible="<%$ HasValue: Title %>" runat="server" />
<n2:Box runat="server">
	<n2:EditableDisplay runat="server" id="t" PropertyName="Text" />
</n2:Box>