<%@ Page MasterPageFile="~/Templates/UI/Layouts/Top+Submenu.master" Language="C#" AutoEventWireup="true" CodeBehind="AddonPage.aspx.cs" Inherits="N2.Addons.AddonCatalog.UI.AddonPage" %>
<%@ Import Namespace="N2.Web"%>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    <h1><%= CurrentPage.Title %> <%# CurrentItem.AddonVersion %></h1>
    <p><%= CurrentPage.Summary.Replace(Environment.NewLine, "<br/>")%></p>
    <p><%= CurrentPage.Text.Replace(Environment.NewLine, "<br/>")%></p>
    <p><label>Published</label> <%= CurrentPage.Created %></p>
    <p><label>Author</label> <%= CurrentPage.ContactName %></p>
    <p><label>Contains</label> <%= CurrentPage.Category %></p>
    <p><label>Downloads</label> <%= CurrentPage.Downloads %></p>
    <p><label>Tested on</label> <%= CurrentPage.LastTestedVersion %></p>
    <p><label>Requirements</label> <%= CurrentPage.Requirements %></p>
    <p>
        <a href="<%= CurrentPage.DownloadUrl %>"><img src="<%= Url.ToAbsolute("~/Addons/AddonCatalog/UI/disk.png") %>" /> Download</a>
        <a href="<%= CurrentPage.HomepageUrl %>"><img src="<%= Url.ToAbsolute("~/Addons/AddonCatalog/UI/house.png") %>" /> Homepage</a>
        <a href="<%= CurrentPage.SourceCodeUrl %>"><img src="<%= Url.ToAbsolute("~/Addons/AddonCatalog/UI/page_white_code.png") %>" /> Source Code</a>
        <a href="<%= Url.Parse(CurrentPage.Url).AppendSegment("edit") %>"><img src="<%= Url.ToAbsolute("~/Addons/AddonCatalog/UI/plugin_edit.png") %>" /> Edit Addon</a>
    </p>
  
    <hr />
    <div>
        <a href="<%= Url.Parse((CurrentPage.Parent ?? CurrentPage.VersionOf.Parent).Url) %>">&laquo; Back</a>
    </div>
</asp:Content>
