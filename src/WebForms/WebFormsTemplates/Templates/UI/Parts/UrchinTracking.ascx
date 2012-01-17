<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UrchinTracking.ascx.cs" Inherits="N2.Templates.UI.Parts.UrchinTracking" %>
<asp:PlaceHolder Visible="<%# Track %>" runat="server">
<script type="text/javascript">
	var _gaq = _gaq || [];
	_gaq.push(['_setAccount', '<%# CurrentItem.UACCT %>']);
	_gaq.push(['_trackPageview']);
	(function () {
		var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
		ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
		var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
	})();
</script>
</asp:PlaceHolder>