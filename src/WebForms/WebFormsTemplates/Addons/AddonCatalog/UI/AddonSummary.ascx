<%@ Import Namespace="N2.Web"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddonSummary.ascx.cs" Inherits="N2.Addons.AddonCatalog.UI.AddonSummary" %>
<h3><a href="<%# CurrentItem.Url %>"><img src="<%= Url.ToAbsolute("~/Addons/AddonCatalog/UI/plugin.png") %>" /> <%# CurrentItem.Title %> <%# CurrentItem.AddonVersion %></a></h3>
<p><%# CurrentItem.Summary.Replace(Environment.NewLine, "<br/>") %></p>
<div>
    <a href="<%# CurrentItem.DownloadUrl %>"><img src="<%= Url.ToAbsolute("~/Addons/AddonCatalog/UI/disk.png") %>" /> Download</a>
    <a href="<%# CurrentItem.Url %>"><img src="<%= Url.ToAbsolute("~/Addons/AddonCatalog/UI/information.png") %>" /> Information</a>
</div>

<hr />
