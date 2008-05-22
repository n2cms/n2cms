<%@ Page Language="C#" MasterPageFile="~/Layouts/Empty.Master" AutoEventWireup="true" CodeBehind="Root.aspx.cs" Inherits="N2.Templates.UI.Secured.Root" Title="Untitled Page" %>
<asp:Content ContentPlaceHolderID="PageArea" runat="server">
    <style>
        body {font-family:Arial;background-color:#C3C0BD;}
        label {margin-right:10px;}
        .uc {background-color:#fff;padding:10px;border:solid 1px #A9A6A3;}
        h1,h2,h3,h4{margin:.5em 0;}
        .field {border-bottom:solid 1px #C3C0BD;padding-bottom:2px;margin-bottom:2px;}
    </style>
    <n2:Zone ZoneName="Above" runat="server"/>
    <table><tr>
        <td><n2:Zone ZoneName="Left" runat="server"/></td>
        <td><n2:Zone ZoneName="Center" runat="server"/></td>
        <td><n2:Zone ZoneName="Right" runat="server"/></td>
    </tr></table>
    <n2:Zone ZoneName="Below" runat="server"/>
</asp:Content>
