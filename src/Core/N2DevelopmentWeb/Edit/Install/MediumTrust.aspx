<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MediumTrust.aspx.cs" Inherits="N2.Edit.Install.MediumTrust" %>

<!-- This is a generated excerpt of the web.config file in a medium trust 
     environment. Please review the sections carefully as you insert them 
     into the existing web.config of your application. -->

<configuration xmlns="http://schemas.microsoft.com/.NetConfiguration/v2.0">
	<configSections>
		<sectionGroup name="n2" type="N2.Configuration.N2ConfigurationSectionHandler, N2">
			<section name="mediumTrust" type="N2.MediumTrust.Configuration.MediumTrustSectionHandler, N2" requirePermission="false"/>
		</sectionGroup>
	</configSections>
	<n2>
		<mediumTrust rootItemID="<%= RootItemID %>" startPageID="<%= StartPageID %>">
		
			<!-- These assemblies are checked for plugins and other things -->
			<assemblies>
				<% foreach(string assemblyName in Assemblies) { %><add assembly="<%= assemblyName %>"/>
				<% } %>
			</assemblies>
			
			<!-- NHibernate is the object relational mapper used for N2 -->
			<nhProperties>
				<% foreach (System.Collections.Generic.KeyValuePair<string, string> property in NhProperties)
				   {%><add key="<%= property.Key %>" value="<%= property.Value %>"/>
				<% } %>
			</nhProperties>
		</mediumTrust>
	</n2>

	<system.web>
		<!-- Your hosting provider may have set this option in machine.config 
		     in which case you may remove this configuration -->
		<trust level="Medium"/>

		<!-- Replace the default n2.initializer	with this one to start the 
		     application with the medium trust engine. -->
		<httpModules>
			<add name="n2.mediumTrustInitializer" type="N2.MediumTrust.Web.InitializerModule, N2"/>
		</httpModules>
	</system.web>
</configuration>