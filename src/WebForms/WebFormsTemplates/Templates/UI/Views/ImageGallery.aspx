<%@ Page Language="C#" MasterPageFile="../Layouts/Top+SubMenu.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Templates.UI.Views.ImageGallery" %>
<asp:Content ID="c" ContentPlaceHolderID="ContentAndSidebar" runat="server">
	<n2:Path ID="p" runat="server" Visible="<%$ StartPage: ShowBreadcrumb %>" />
	<n2:EditableDisplay PropertyName="Title" runat="server" />
	<n2:EditableDisplay PropertyName="Text" runat="server" />
	
	<div id="thumbnails">	
		<asp:Repeater ID="rptImages" runat="server">
			<ItemTemplate>
				<a id="t<%# Eval("ID") %>" href='<%# Eval("ResizedImageUrl") %>' class="thumbnail">
					<img alt='<%# Eval("Title") %>' src='<%# Eval("ThumbnailImageUrl") %>' />
				</a>
				<div class="text">
					<h2><%# Eval("Title") %></h2>
					<%# Eval("Text") %>
				</div>
			</ItemTemplate>
		</asp:Repeater>
	</div>
	<div id="preview">
	</div>

    <script type="text/javascript">
		$(document).ready(function(){
			var show = function(){
				$(".selected", "#thumbnails").removeClass("selected");
				$("#preview").html($(this).addClass("selected").next().children().clone()).prepend("<img src='" + this.href + "'/>");
				
				return false;
			};
			$(".thumbnail").click(show).filter(location.hash || "*").slice(0,1).each(show);
		});
    </script>
</asp:Content>

