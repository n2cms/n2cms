<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" MasterPageFile="UI/Site.master" Inherits="App._Default" %>
<asp:Content ContentPlaceHolderID="content" runat="server">
    <!-- The display control uses the default presentation for an item's property, the title in this case uses header 1 -->
    <n2:Display ID="TitleDisplay" PropertyName="Title" runat="server" />
    <div>
        <!-- This is a way to inject data into a webforms control, in this case we're injecting the current page's text property -->
        <asp:Literal ID="TextLiteral" Text="<%$ CurrentPage: Text %>" runat="server" />
    </div>
</asp:Content>
