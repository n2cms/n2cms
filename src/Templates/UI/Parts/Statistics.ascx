<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Statistics.ascx.cs" Inherits="N2.Templates.UI.Parts.Statistics" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Edit" %>
<h4><%= CurrentItem.Title %></h4>
<div class="field">
    <edit:InfoLabel Label="# of pages" id="lblPages" runat="server" />
</div>
<div class="field">
    <edit:InfoLabel Label="Total # of items:" id="lblItems" runat="server" />
</div>
<div class="field">
    <edit:InfoLabel Label="Pages served (since startup):" id="lblServed" runat="server" />
</div>
<div class="field">
    <edit:InfoLabel Label="Versions per item:" id="lblVersionsRatio" runat="server" />
</div>
<div class="field">
    <edit:InfoLabel Label="# of changes last week:" id="lblChangesLastWeek" runat="server" />
</div>
<div>
    <label>Latest changes:</label>
</div>
<div>
    <asp:Repeater ID="rptLatestChanges" runat="server">
        <ItemTemplate>
            <div>
                <asp:HyperLink NavigateUrl='<%# Eval("Url") %>' runat="server" Text='<%# Eval("Title") %>' ToolTip='<%# Eval("SavedBy") +", "+ Eval("Updated") %>' />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>