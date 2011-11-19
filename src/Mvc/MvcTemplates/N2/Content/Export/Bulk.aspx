<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Bulk.aspx.cs" Inherits="N2.Management.Content.Export.Bulk" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>
    
<asp:Content ID="CH" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
    <edit:CancelLink ID="hlCancel" runat="server">Close</edit:CancelLink>
</asp:Content>
<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
    <n2:tabpanel id="tpUpload" runat="server" ToolTip="Upload">
        <div>
            <asp:Label ID=lblImportFile" AssociatedControlID="fuImportFile" Text="Import file" />
            <asp:FileUpload ID="fuImportFile" runat="server" />
            <asp:RequiredFieldValidator ID="rfvImportFile" Text="Import file is required" runat="server" ControlToValidate="fuImportFile" />
        </div>
        <div>
            <asp:Button ID="btnUpload" runat="server" Text="Upload and continue" OnCommand="UploadCommand" />
        </div>
    </n2:tabpanel>
    <n2:tabpanel id="tpImport" runat="server" ToolTip="Import" enabled="false">
        <asp:DataGrid ID="dgrTEst" runat="server" />
    </n2:tabpanel>

</asp:Content>