<%@ Page Language="C#" MasterPageFile="../../Content/Framed.Master" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="N2.Management.Files.FileSystem.Upload" %>
<asp:Content ID="ct" ContentPlaceHolderID="Toolbar" runat="server">
	<asp:HyperLink ID="hlCancel" runat="server" Text="cancel" CssClass="command cancel" meta:resourceKey="hlCancel" />
</asp:Content>
<asp:Content ID="cc" ContentPlaceHolderID="Content" runat="server">
	<%
		string Movie = "FlashFileUpload.swf";
		//fileTypes=&fileTypeDescription=&totalUploadSize=
		string FlashVars = "completeFunction=onUploadComplete()"
			+ "&fileSizeLimit=" + maxFileSize 
			+ "&uploadPage=" + Server.UrlEncode("UploadFile.ashx?selected=" + Selection.SelectedItem.Path + "&ticket=" + FormsAuthentication.Encrypt(new FormsAuthenticationTicket("SecureUpload-" + Guid.NewGuid(), false, 60)));
	 %>
	<script type="text/javascript">
		function onUploadComplete() {
			<%= GetRefreshScript(Selection.SelectedItem, N2.Edit.ToolbarArea.Navigation) %>;
		}
	</script>
	<object id="flashUploader" classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000"
		codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,0,0"
		width="575" height="375" id="fileUpload" align="middle">
		<div style="padding:10px;">
			<asp:FileUpload runat="server" ID="fuAlternative" />
			<asp:Button runat="server" ID="btnAlternative" OnCommand="btnAlternative_Command" Text="Upload" meta:resourceKey="btnAlternative" />
		</div>
		<param name="allowScriptAccess" value="sameDomain" />
		<param name="movie" value="<%= Movie %>" />
		<param name="quality" value="high" />
		<param name="wmode" value="opaque">
		<param name=FlashVars value='<%= FlashVars %>'>
		<embed src="<%= Movie %>" flashvars='<%= FlashVars %>'
			quality="high" wmode="transparent" width="575" height="375" 
			name="fileUpload" align="middle" allowScriptAccess="sameDomain" 
			type="application/x-shockwave-flash" 
			pluginspage="http://www.macromedia.com/go/getflashplayer" />
	</object>
</asp:Content>