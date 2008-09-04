<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Subscribe.ascx.cs" Inherits="N2.Templates.UI.Parts.Subscribe" %>
<n2:EditableDisplay runat="server" PropertyName="Title" />
<n2:Box runat="server">
    <%= N2.Web.Link.To(CurrentItem.SelectedFeed).Text(string.Format("<img src='{0}' alt='RSS' /> {1}", VirtualPathUtility.ToAbsolute("~/Templates/UI/Img/feed.png"), N2.Utility.Evaluate(CurrentItem.SelectedFeed, "Title")))%>
</n2:Box>