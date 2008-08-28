<%@ Page Language="C#" MasterPageFile="~/DocSite.Master" AutoEventWireup="true" Codebehind="Default.aspx.cs" Inherits="N2.DocSite.Default" %>

<asp:Content ID="HelpFile" ContentPlaceHolderID="contentPlaceHolder" runat="server">
  <div id="docsite_content_iframe_container">
		<iframe runat="server" class="docsite_content_iframe" id="ContentFrame" width="100%" frameborder="0" scrolling="no"></iframe>
	</div>
	<asp:HiddenField runat="server" ID="ContentUrl" />
	<%-- The following variable is required by the script that is generated in the code-behind.
	     Since this script is not registered for async postback (with the ScriptManager) the 
	     variable will only be initialized once.  The generated script _is_ registered and, 
	     therefore, will always use the most updated value. --%>
	<script type="text/javascript">
	  var contentFrame_FirstLoad = true;
	</script>
</asp:Content>
