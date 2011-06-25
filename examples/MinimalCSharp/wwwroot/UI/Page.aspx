<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page.aspx.cs" Inherits="App.UI.Page" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
	<!-- master page and theme is defined in web.config -->
	
	<!-- The droppable zone enables drop of parts onto the tempalate when in drag&drop mode -->        
    <div class="zone">
		<n2:DroppableZone ID="DroppableZone1" runat="server" ZoneName="SecondaryContent"/>
	</div>
	
    <asp:SiteMapPath ID="Path" runat="server" CssClass="breadcrumb" />

    <!-- The display control uses the default presentation for an item's property, the title in this case uses header 1 -->
    <n2:Display ID="TitleDisplay" PropertyName="Title" runat="server" />
    <div>
        <!-- This is a way to inject data into a webforms control, in this case we're injecting the current page's text property -->
        <asp:Literal ID="TextLiteral" Text="<%$ CurrentPage: Text %>" runat="server" />
    </div>
</asp:Content>