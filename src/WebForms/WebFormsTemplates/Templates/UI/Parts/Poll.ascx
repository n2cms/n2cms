<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Poll.ascx.cs" Inherits="N2.Templates.UI.Parts.Poll" %>
<n2:EditableDisplay runat="server" PropertyName="Title" />
<n2:Box runat="server">
    <asp:PlaceHolder ID="phQuestion" runat="server" />
    <asp:CustomValidator ValidationGroup="Poll" ID="cvAlternative" ErrorMessage="Select an alternative." runat="server" Enabled="false" Display="dynamic" OnServerValidate="cbAlternative_ServerValidate" meta:resourcekey="cvAlternative"/>
	<div class="buttons">
		<asp:Button ValidationGroup="Poll" ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" meta:resourcekey="btnSubmit" />
		<asp:HyperLink ID="hlDisplay" runat="server" Text="Display results" meta:resourcekey="hlDisplay" NavigateUrl='<%$ Code: GetShowUrl() %>' />
    </div>
</n2:Box>
