<%@ Import Namespace="N2.Addons.MyAddon.UI"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyPart.ascx.cs" Inherits="N2.Addons.MyAddon.UI.MyPart" %>
<div style="border:solid 2px silver">
    I was created <%= CurrentItem.Created %> 
    and has since then been visited <%= CurrentItem.TimesVisited %> times
    and the last time was <%= CurrentItem.LastVisited %>
</div>