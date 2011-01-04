<%@ Import Namespace="N2.Web"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddonEditForm.ascx.cs" Inherits="N2.Addons.AddonCatalog.UI.AddonEditForm" %>
<asp:CustomValidator Text="You need to be logged in (and be original author) to submit or modify add-ons" id="cvAuthenticated" runat="server" Display="Dynamic" />
<asp:Panel ID="pnlAddon" runat="server" CssClass="addonForm">
    <fieldset><legend>Summary</legend>
        <h3 class="title">
            <asp:TextBox ID="txtTitle" runat="server" ToolTip="My Add-On Title" MaxLength="50"/>
            <asp:RequiredFieldValidator ID="rfvTitle" ControlToValidate="txtTitle" runat="server" Text="*" Display="Dynamic" />
        </h3>
        <div class="row">
            <asp:TextBox ID="txtSummary" runat="server" TextMode="MultiLine" Rows="2" ToolTip="A short summary of what this add-on brings to the table"/>
            <asp:RequiredFieldValidator ID="rfvSummary" ControlToValidate="txtSummary" runat="server" Text="*" Display="Dynamic" />
        </div>
        <div class="row">
            <label>Upload ZIP </label><asp:FileUpload ID="fuAddon" runat="server" value="Select file" CssClass="fu"/>
            <asp:RequiredFieldValidator ID="rfvAddon" ControlToValidate="fuAddon" runat="server" Text="*" Display="Dynamic" />
        </div>
    </fieldset>
    <fieldset><legend>Details</legend>
        <div class="row">
            <asp:TextBox ID="txtEmail" runat="server" ToolTip="Contact e-mail address (not displayed)"/>
            <asp:RequiredFieldValidator ID="rfvEmail" ControlToValidate="txtEmail" runat="server" Text="*" Display="Dynamic" />
        </div>
        <div class="row">
            <asp:TextBox ID="txtName" runat="server" ToolTip="Your Name/Public Alias" MaxLength="150"/>
            <asp:RequiredFieldValidator ID="rfvName" ControlToValidate="txtName" runat="server" Text="*" Display="Dynamic" />
        </div>
        <div class="row">
            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="6" ToolTip="A more thorough of the add-on including installation instructions as needed"/>
        </div>
        <div class="row">
            <asp:TextBox ID="txtN2Version" runat="server" ToolTip="N2 Version used during development and testing (e.g. 1.4.3)" MaxLength="50"/>
        </div>
        <div class="row">
            <asp:TextBox ID="txtVersion" runat="server" ToolTip="Current Add-On Version (e.g. 1.0)"  MaxLength="50"/>
        </div>
        <div class="row">
            <asp:TextBox ID="txtHomepage" runat="server" ToolTip="Homepage URL (e.g. http://n2cms.com)" MaxLength="150"/>
        </div>
        <div class="row">
            <asp:TextBox ID="txtSource" runat="server" ToolTip="URL to Source Code (e.g. http://www.codeplex.com/n2)" MaxLength="150"/>
        </div>
        <div class="row">
            <label>Type of add-on</label>
            <asp:CheckBoxList id="cblCategory" runat="server" CssClass="cbl" RepeatDirection="Horizontal">
                <asp:ListItem Text="Library/Developer tool" Value="1" />
                <asp:ListItem Text="Theme/Layout" Value="2" />
                <asp:ListItem Text="Pages" Value="4" />
                <asp:ListItem Text="Parts" Value="8" />
            </asp:CheckBoxList>
        </div>
        <div class="row">
            <label>Depends on</label>
            <asp:CheckBoxList id="cblRequirements" runat="server" CssClass="cbl" RepeatDirection="Horizontal">
                <asp:ListItem Text="N2/N2 Edit" Value="1" Selected="True" />
                <asp:ListItem Text="N2 Templates" Value="2" Selected="True" />
                <asp:ListItem Text="ASP.NET 3.5" Value="4" />
                <asp:ListItem Text="Full Trust" Value="8" />
            </asp:CheckBoxList>
        </div>

    </fieldset>
    <asp:Button ID="btnSave" runat="server" OnClick="save_Click" Text="Save" CssClass="b" />
    <a href="<%= Url.Parse(CurrentPage.Url) %>">&laquo; Back</a>

    <script type="text/javascript">
        $("textarea,input[type='text']").addClass("tb").each(function() {
            if (!this.value) {
                this.value = this.title;
                $(this).addClass("unedited").focus(function() {
                    if (!this.originalValue) {
                        this.originalValue = this.value;
                        this.value = "";
                        $(this).removeClass("unedited");
                    }
                }).blur(function() {
                    if (!this.value) {
                        this.value = this.originalValue;
                        this.originalValue = null;
                        $(this).addClass("unedited");
                    }
                });
            }
        });
        $("#<%= btnSave.ClientID %>").click(function() {
            $("textarea,input[type='text']").each(function() {
                if (this.value == this.title)
                    this.value = "";
            });
        });
    </script>
</asp:Panel>