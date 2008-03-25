<%@ Page Language="C#" MasterPageFile="~/Layouts/Top+SubMenu.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.ImageGallery.UI.Default" %>
<asp:Content ID="c" ContentPlaceHolderID="Content" runat="server">
    <n2:ItemDataSource id="idsImages" runat="server" />
    <asp:Repeater runat="server" DataSourceID="idsImages">
        <ItemTemplate>
            <a href='<%# Eval("ResizedImageUrl") %>' title='<%# Eval("Title") %>' target='larger' class="thumbnail">
                <img alt='<%# Eval("Title") %>' src='<%# Eval("ThumbnailImageUrl") %>' />
            </a>
        </ItemTemplate>
    </asp:Repeater>
    <div class="nav cf"><div id="prev"></div><div id="next"></div><h1 id="title"></h1></div>
    <div id="large"></div>
    <script type="text/javascript">
		$(window).load(function(){
			$(".thumbnail").n2gallery("#large", {navigate:true, prev:"#prev", next:"#next", title:"#title"});
		});
    </script>
    <n2:EditableDisplay PropertyName="Text" runat="server" />
</asp:Content>

