<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Export.Default" meta:resourcekey="PageResource1" %>
<%@ Register Src="../AffectedItems.ascx" TagName="AffectedItems" TagPrefix="uc1" %>
<%@ Register TagPrefix="edit" Namespace="N2.Edit.Web.UI.Controls" Assembly="N2.Management" %>

<asp:Content ID="CH" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="<%=MapCssUrl("exportImport.css")%>" type="text/css" />
</asp:Content>
<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
    <edit:CancelLink ID="hlCancel" runat="server" meta:resourceKey="hlCancel">Cancel</edit:CancelLink>
</asp:Content>
<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
    <n2:tabpanel id="tpExport" runat="server" ToolTip="Export" meta:resourceKey="tpExport" >
		<div>
		    <asp:Button ID="btnExport" runat="server" CssClass="command" OnCommand="btnExport_Command" CausesValidation="false" meta:resourceKey="btnExport" Text="Export these items" />
		</div>
		<div>
		    <asp:CheckBox ID="chkDefinedDetails" runat="server" Text="Exclude computer generated data"  meta:resourceKey="chkDefinedDetails" />
		</div>
		<div>
		    <asp:CheckBox ID="chkAttachments" runat="server" Text="Don't export attachments"  meta:resourceKey="chkAttachments" />
		</div>
		<n2:h4 runat="server" Text="Exported items" meta:resourceKey="exportedItems" />
		<uc1:AffectedItems id="exportedItems" runat="server" />		
    </n2:tabpanel>
    <n2:tabpanel runat="server" ToolTip="Import" meta:resourceKey="tpImport">
        <asp:CustomValidator id="cvImport" runat="server" CssClass="validator" meta:resourceKey="cvImport" Display="Dynamic"/>
	    <asp:MultiView ID="uploadFlow" runat="server" ActiveViewIndex="0">
		    <asp:View ID="uploadView" runat="server">
			    <div class="upload">
				    <div class="cf">
				        <asp:FileUpload ID="fuImport" runat="server" />
				        <asp:RequiredFieldValidator ID="rfvUpload" ControlToValidate="fuImport" runat="server" ErrorMessage="*"  meta:resourceKey="rfvImport"/>
				    </div>
				    <div>
				        <asp:Button ID="btnVerify" runat="server" Text="Upload and examine" OnClick="btnVerify_Click" Display="Dynamic" meta:resourceKey="btnVerify"/>
				        <asp:Button ID="btnUploadImport" runat="server" Text="Import here" OnClick="btnUploadImport_Click"  meta:resourceKey="btnUploadImport"/>
				    </div>
			    </div>
		    </asp:View>
		    <asp:View ID="preView" runat="server">
		        <div>
		            <asp:CheckBox ID="chkSkipRoot" runat="server" Text="Skip imported root item" ToolTip="Checking this options cause the first level item not to be imported, and it's children to be added to the selected item's children" meta:resourceKey="chkSkipRoot" />
		        </div>
			    <asp:Button ID="btnImportUploaded" runat="server" Text="Import" OnClick="btnImportUploaded_Click"  meta:resourceKey="btnImportUploaded"/>
			    <n2:h4 runat="server" Text="Imported Items" meta:resourceKey="importedItems" />
			    <uc1:AffectedItems id="importedItems" runat="server" />
			    <n2:h4 runat="server" Text="Attachments" meta:resourceKey="attachments" />
			    <asp:Repeater ID="rptAttachments" runat="server">
			        <ItemTemplate>
			            <div class="file"><asp:Image runat="server" ImageUrl="../../Resources/icons/page_white.png" alt="file" /><%# Eval("Url") %> <span class="warning"><%# CheckExists((string)Eval("Url")) %></span></div>
			        </ItemTemplate>
			    </asp:Repeater>
		    </asp:View>
	    </asp:MultiView>
    </n2:tabpanel>
</asp:Content>
