<%@ Page Title="" Language="C#" MasterPageFile="~/Templates/UI/Layouts/Top+SubMenu.Master" AutoEventWireup="true" Inherits="N2.Templates.Web.UI.TemplatePage" %>

<script runat="server">

	protected override void OnInit(EventArgs e)
	{
		Zone1.ZoneName = "AutoZone2";
		Zone2.ZoneName = "AutoZone2";
		base.OnInit(e);
	}
	protected override void OnLoad(EventArgs e)
	{
		Zone3.ZoneName = "AutoZone2";
		Zone4.ZoneName = "AutoZone2";
		base.OnLoad(e);
	}
	protected override void OnPreRender(EventArgs e)
	{
		Zone5.ZoneName = "AutoZone2";
		Zone6.ZoneName = "AutoZone2";
		base.OnPreRender(e);
	}
		
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
	<fieldset>
		<legend>Never changed</legend>
		<n2:Zone ID="Zone0" ZoneName="AutoZone1" runat="server">
			<HeaderTemplate><fieldset></HeaderTemplate>
			<SeparatorTemplate></fieldset><fieldset></SeparatorTemplate>
			<FooterTemplate></fieldset></FooterTemplate>
		</n2:Zone>
	</fieldset>
	
	<fieldset>
		<legend>OnBuild</legend>
		<n2:Zone ID="Zone7" ZoneName='<%$ Code: "AutoZone2" %>' runat="server">
			<HeaderTemplate><fieldset></HeaderTemplate>
			<SeparatorTemplate></fieldset><fieldset></SeparatorTemplate>
			<FooterTemplate></fieldset></FooterTemplate>
		</n2:Zone>
	</fieldset>
	
	<fieldset>
		<legend>OnInit</legend>
		<em>Changed</em>
		<n2:Zone ID="Zone1" ZoneName="AutoZone1" runat="server">
			<HeaderTemplate><fieldset></HeaderTemplate>
			<SeparatorTemplate></fieldset><fieldset></SeparatorTemplate>
			<FooterTemplate></fieldset></FooterTemplate>
		</n2:Zone>
		<em>Set</em>
		<n2:Zone ID="Zone2" runat="server">
			<HeaderTemplate><fieldset></HeaderTemplate>
			<SeparatorTemplate></fieldset><fieldset></SeparatorTemplate>
			<FooterTemplate></fieldset></FooterTemplate>
		</n2:Zone>
	</fieldset>
	
	<fieldset>
		<legend>OnLoad</legend>
		<em>Changed</em>
		<n2:Zone ID="Zone3" ZoneName="AutoZone1" runat="server">
			<HeaderTemplate><fieldset></HeaderTemplate>
			<SeparatorTemplate></fieldset><fieldset></SeparatorTemplate>
			<FooterTemplate></fieldset></FooterTemplate>
		</n2:Zone>
		<em>Set</em>
		<n2:Zone ID="Zone4" runat="server">
			<HeaderTemplate><fieldset></HeaderTemplate>
			<SeparatorTemplate></fieldset><fieldset></SeparatorTemplate>
			<FooterTemplate></fieldset></FooterTemplate>
		</n2:Zone>
	</fieldset>
	
	<fieldset>
		<legend>OnPreRender</legend>
		<em>Changed</em>
		<n2:Zone ID="Zone5" ZoneName="AutoZone1" runat="server">
			<HeaderTemplate><fieldset></HeaderTemplate>
			<SeparatorTemplate></fieldset><fieldset></SeparatorTemplate>
			<FooterTemplate></fieldset></FooterTemplate>
		</n2:Zone>
		<em>Set</em>
		<n2:Zone ID="Zone6" runat="server">
			<HeaderTemplate><fieldset></HeaderTemplate>
			<SeparatorTemplate></fieldset><fieldset></SeparatorTemplate>
			<FooterTemplate></fieldset></FooterTemplate>
		</n2:Zone>
	</fieldset>
</asp:Content>
