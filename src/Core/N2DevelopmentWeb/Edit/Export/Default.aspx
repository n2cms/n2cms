<%@ Page MasterPageFile="../Framed.master" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="N2.Edit.Export.Default" %>
<%@ Register Src="../AffectedItems.ascx" TagName="AffectedItems" TagPrefix="uc1" %>
<asp:Content ID="CH" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="Css/exportImport.css" type="text/css" />
</asp:Content>
<asp:Content ID="CT" ContentPlaceHolderID="Toolbar" runat="server">
    <asp:HyperLink ID="hlCancel" runat="server" CssClass="cancel command" AccessKey="C" meta:resourceKey="hlCancel"><img src="../img/ico/cancel.gif" /> cancel</asp:HyperLink>
</asp:Content>
<asp:Content ID="CC" ContentPlaceHolderID="Content" runat="server">
    <h1>Export & Import</h1>
    <n2:TabPanel id="tpExport" runat="server" ToolTip="Export">
		<div>
		    <asp:Button ID="btnExport" runat="server" CssClass="command" OnCommand="btnExport_Command" CausesValidation="false" meta:resourceKey="btnExport" Text="Export these items" />
		</div>
		<h4>Exported items</h4>
		<uc1:AffectedItems id="exportedItems" runat="server" />		
    </n2:TabPanel>
    <n2:TabPanel runat="server" ToolTip="Import">
        <asp:CustomValidator id="cvImport" runat="server" CssClass="validator" meta:resourceKey="cvImport" Display="Dynamic"/>
	    <asp:MultiView ID="uploadFlow" runat="server" ActiveViewIndex="0">
		    <asp:View ID="uploadView" runat="server">
			    <div class="upload">
				    <asp:FileUpload ID="fuImport" runat="server" />
				    <asp:RequiredFieldValidator ID="rfvUpload" ControlToValidate="fuImport" runat="server" ErrorMessage="*"  meta:resourceKey="rfvImport"/>
				    <asp:Button ID="btnVerify" runat="server" Text="Upload and examine" OnClick="btnVerify_Click" Display="Dynamic" meta:resourceKey="btnVerify"/>
				    <asp:Button ID="btnUploadImport" runat="server" Text="Import here" OnClick="btnUploadImport_Click"  meta:resourceKey="btnUploadImport"/>
			    </div>
		    </asp:View>
		    <asp:View ID="preView" runat="server">
		        <div>
		            <asp:CheckBox ID="chkSkipRoot" runat="server" Text="Skip imported root item" ToolTip="Checking this options cause the first level item not to be imported, and it's children to be added to the selected item's children" />
		        </div>
			    <asp:Button ID="btnImportUploaded" runat="server" Text="Import" OnClick="btnImportUploaded_Click"  meta:resourceKey="btnImportUploaded"/>
			    <h4>Imported Items</h4>
			    <uc1:AffectedItems id="importedItems" runat="server" />
			    <h4>Attachments</h4>
			    <asp:Repeater ID="rptAttachments" runat="server">
			        <ItemTemplate>
			            <div class="file"><asp:Image runat="server" ImageUrl="~/edit/img/ico/page_white.gif" alt="file" /><%# Eval("Url") %> <span class="warning"><%# CheckExists((string)Eval("Url")) %></span></div>
			        </ItemTemplate>
			    </asp:Repeater>
		    </asp:View>
	    </asp:MultiView>
    </n2:TabPanel>
</asp:Content>
