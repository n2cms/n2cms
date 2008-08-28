<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocSiteLetterBar.ascx.cs" Inherits="N2.DocSite.Controls.DocSiteLetterBar" %>

<script runat="server">
private int position = 0;
</script>
<asp:Repeater runat="server" ID="letterRepeater" DataSource='<%# Letters %>'>
  <ItemTemplate>
    <span class="nowrap"><asp:LinkButton runat="server" ID="letterLink" OnCommand="letter_Command" CommandName='<%# Container.DataItem %>'
          Text='<%# char.ToUpper((char) Container.DataItem) %>' /><asp:Label runat="server" ID="separatorLabel" Visible='<%# ++position < LettersCount %>'> |</asp:Label></span>
  </ItemTemplate>
</asp:Repeater>