<%@ Control Language="C#" AutoEventWireup="true" Inherits="N2.Web.Mvc.N2ViewUserControl<TextItem>" %>
<n2:H4 Text="<%$ CurrentItem: Title %>" Visible="<%$ HasValue: Title %>" runat="server" />
<%=this.EditableDisplay(m => m.Text)%>