<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Diagnose.aspx.cs" Inherits="N2.Edit.Install.Diagnose" %>
<%@ Import Namespace="System.Collections.Generic"%>
<%@ Import Namespace="N2.Web"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Diagnose N2</title>
    <link rel="stylesheet" href="../Resources/Css/All.css" type="text/css" />
    <style type="text/css">
        label{font-weight:bold;margin:5px 10px 0 0;}
        input{vertical-align:middle;margin-bottom:5px;}
        ul,li{margin-top:0;margin-bottom:0;}
        textarea{height:55px;width:70%;background-color:#FFA07A;border:none;font-size:11px}
        .t {font-size:.8em; width:100%;}
        .t thead td{ font-weight:bold; background-color:#eee;}
		.t th h2 { margin:0; padding:.1em; background-color:#ccc; width:auto;}
        .t td{vertical-align:top;text-align:left;border:solid 1px #eee; padding:1px; font-size:inherit; }
        .t input { font-size:inherit; }
        .EnabledFalse { color:#999; }
        .IsDefinedFalse { color:Red; }
        a { color:Blue; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>N2 CMS diagnostic page</h1>

			<table class="t">
				<tbody>
					<tr><th colspan="2"><h2>Database</h2></th></tr>
<% try { %>
					<tr><th>Connection provider</th><td><%= System.Configuration.ConfigurationManager.ConnectionStrings[N2.Context.Current.Resolve<N2.Configuration.DatabaseSection>().ConnectionStringName].ProviderName %></td></tr>
<% } catch (Exception ex) { Response.Write("<tr><td>" + ex + "</td></tr>"); } %>
					<tr><th>Connection</th><td><asp:Label ID="lblDbConnection" runat="server" /></td></tr>
					<tr><th>Root item</th><td><asp:Label ID="lblRootNode" runat="server" /></td></tr>
					<tr><th>Start page</th><td><asp:Label ID="lblStartNode" runat="server" /></td></tr>
					<tr><th rowspan="<%= recentChanges.Length + 1 %>">Recent changes </th><td><asp:Label ID="lblChanges" runat="server" />
						<% foreach (string change in recentChanges){ %>
							<%= change %> </td></tr><tr><td>
						<% } %>
					</td></tr>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>Assemblies</h2></th></tr>
					<tr><th>N2 version</th><td><asp:Label ID="lblN2Version" runat="server" /></td></tr>
					<tr><th>N2.Management version</th><td><asp:Label ID="lblEditVersion" runat="server" /></td></tr>
<% try { %>
					<tr><th>Engine type</th><td><%= N2.Context.Current.GetType() %></td></tr>
					<tr><th>IoC Container type</th><td><%= N2.Context.Current.Container.GetType() %></td></tr>
					<tr><th>Url parser type</th><td><%= N2.Context.Current.Resolve<N2.Web.IUrlParser>().GetType() %></td></tr>
<% } catch (Exception ex) { Response.Write("<tr><th>Error</th><td>" + ex.ToString() + "</td>"); } %>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>Server</h2></th></tr>
					<tr><th>Trust Level</th><td><%= GetTrustLevel() %></td></tr>
					<tr><th>Application Path</th><td><%= Request.ApplicationPath %></td></tr>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>Sites</h2></th></tr>
<% try { %>
					<tr><th>Default (fallback) site</th><td><%= host.DefaultSite %></td></tr>
					<tr><th>Current site (<%= Request.Url.Authority %>)</th><td><%= host.CurrentSite %></td></tr>
					<tr>
						<th rowspan="<%= host.Sites.Count %>">All sites</th>
				<% foreach (Site site in host.Sites){ %>
						<td><%= site %></td></tr><tr>
				<% } %>
					</tr>
<% } catch (Exception ex) { Response.Write(ex.ToString()); } %>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>Errors</h2></th></tr>
					<tr><th>Last error</th><td><asp:Label ID="lblError" runat="server" /></td></tr>
				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>MVC</h2></th></tr>
<% try {%>
					<tr><th rowspan="3">Versions</th><td><%= System.Reflection.Assembly.LoadWithPartialName("System.Web.Mvc").FullName %></td></tr>
					<tr><td><%= System.Reflection.Assembly.LoadWithPartialName("System.Web.Abstractions").FullName %></td></tr>
					<tr><td><%= System.Reflection.Assembly.LoadWithPartialName("System.Web.Routing").FullName %></td></tr>
<% } catch (Exception ex) { Response.Write("<tr><td>" + ex + "</td></tr>"); } %>
<%--<% try {
	IEnumerable routes = Type.GetType("System.Web.Routing.RouteTable").GetProperty("Routes", System.Reflection.BindingFlags.Static).GetValue(null, null) as IEnumerable;
	foreach (object route in routes) {%>
					<tr><th></th><td><%= route %></td></tr>
<% } } catch (Exception ex) { Response.Write("<tr><td>" + ex + "</td></tr>"); } %>
--%>				</tbody>
				<tbody>
					<tr><th colspan="2"><h2>Operations <span style="font-size:.9em">(prefer the <a href=".">installation wizard</a> before using this)</span></h2></th></tr>
					<tr><th>Restart web application</th><td><asp:Button ID="btnRestart" runat="server" OnClick="btnRestart_Click" Text="RESTART" OnClientClick="return confirm('restart site?');" /></td></tr>
					<tr><th>Drop tables clearing all content data in database</th><td>
				<asp:Button ID="btnClearTables" runat="server" OnClick="btnClearTables_Click" Text="DROP" OnClientClick="return confirm('drop all tables?');" />
				<asp:Label runat="server" ID="lblClearTablesResult" /></td></tr>
					<tr><th>Create database schema (this drops any existing tables)</th><td>
				<asp:Button ID="btnAddSchema" runat="server" OnClick="btnAddSchema_Click" Text="CREATE" OnClientClick="return confirm('drop and recreate all tables?');" />
				<asp:Label runat="server" ID="lblAddSchemaResult" /></td></tr>
					<tr><th>Insert root node</th><td>
				<asp:Button ID="btnInsert" runat="server" OnClick="btnInsert_Click" Text="Select type..." />
				<asp:DropDownList ID="ddlTypes" runat="server" AutoPostBack="True" Visible="False">
				</asp:DropDownList><asp:Button runat="server" ID="btnInsertRootNode" Text="OK" Visible="false" OnClick="btnInsertRootNode_Click" />
				<asp:Label ID="lblInsert" runat="server"></asp:Label></td></tr>
				</tbody>
			</table>

            <i>These settings are generated at application start from attributes in the project source code.</i>
            <asp:Repeater ID="rptDefinitions" runat="server">
                <HeaderTemplate>
					<table class="t">
						<thead>
							<tr><th colspan="8"><h2>Definitions</h2></th></tr>
							<tr>
								<td colspan="2">Definition</td>
								<td colspan="2">Zones</td>
								<td colspan="2">Details</td>
								<td rowspan="2">Templates</td>
								<td rowspan="2">Templates (2)</td>
							</tr>
							<tr>
								<td>Type</td>
								<td>Allowed children</td>
								<td>Available</td>
								<td>Allowed in</td>
								<td>Editables</td>
								<td>Displayables</td>
							</tr>
						</thead>
						<tbody>
				</HeaderTemplate>
                <ItemTemplate>
                    <tr class="<%# Eval("Enabled", "Enabled{0}") %> <%# Eval("IsDefined", "IsDefined{0}") %>"><td>
                        <b><%# Eval("Title") %></b><br /><span style="color:gray"><%# Eval("ItemType") %></span><br/><%# Eval("Discriminator") %>
                    </td><td>
                        <!-- Child definitions -->
                        <asp:Repeater ID="Repeater1" runat="server" DataSource='<%# AllowedChildren(Container.DataItem) %>'>
                            <ItemTemplate><%# Eval("Title")%></ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;
                    </td><td>
                        <!-- Available zones -->
                        <asp:Repeater ID="Repeater2" runat="server" DataSource='<%# Eval("AvailableZones") %>'>
                            <ItemTemplate><%# Eval("ZoneName") %>&nbsp;(<%# Eval("Title") %>)</ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;
                    </td><td>
						<b><%# Eval("AllowedIn")%>: </b><br />
                        <!-- Allowed in zone -->
                        <asp:Repeater ID="Repeater3" runat="server" DataSource='<%# Eval("AllowedZoneNames") %>'>
                            <ItemTemplate><%# Container.DataItem %></ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;
                    </td><td>
                        <!-- Editable attributes -->
                        <asp:Repeater ID="Repeater4" runat="server" DataSource='<%# Eval("Editables") %>'>
                            <ItemTemplate><%# Eval("Title")%>&nbsp;(<%# Eval("Name")%>)</ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;
                    </td><td>
                        <!-- Displayable attributes -->
                        <asp:Repeater ID="Repeater5" runat="server" DataSource='<%# Eval("Displayables") %>'>
                            <ItemTemplate><%# ((N2.Details.IDisplayable)Container.DataItem).Name %></ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;
                    </td><td>
                        <asp:Repeater ID="Repeater6" runat="server" DataSource='<%# PathDictionary.GetFinders((Type)Eval("ItemType")) %>'>
                            <ItemTemplate><%# Container.DataItem is TemplateAttribute ? ("/" + Eval("Action") + "&nbsp;->&nbsp;" + Eval("TemplateUrl")) : ("(" + Container.DataItem.GetType().Name + ")")%><br></ItemTemplate>
                        </asp:Repeater>&nbsp;
                    </td><td>
                        <asp:Repeater ID="Repeater7" runat="server" DataSource='<%# N2.Context.Current.Definitions.GetTemplates((Type)Eval("ItemType")) %>'>
                            <ItemTemplate>
								<%# Eval("Title") %>
							</ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
                        </asp:Repeater>&nbsp;
                    </td></tr>
                </ItemTemplate>
                <FooterTemplate>
						</tbody>
					</table>
				</FooterTemplate>
            </asp:Repeater>
            <asp:Label ID="lblDefinitions" runat="server" />
            
            <h2>Assemblies</h2>
            <asp:Repeater ID="rptAssembly" runat="server">
                <HeaderTemplate><table class="t"><thead><tr><td>Assembly Name</td><td>Version</td><td>Culture</td><td>Public Key</td><td>References N2</td><td>Definitions</td></tr></thead><tbody></HeaderTemplate>
                <ItemTemplate><tr>
                <asp:Repeater runat="server" DataSource="<%# ((System.Reflection.Assembly)Container.DataItem).FullName.Split(',') %>">
					<ItemTemplate>
						<td><%# Container.DataItem %></td>
					</ItemTemplate>
                </asp:Repeater>
                <td><%# Array.Find(((System.Reflection.Assembly)Container.DataItem).GetReferencedAssemblies(), delegate(System.Reflection.AssemblyName an) { return an.Name.StartsWith("N2"); }) != null %></td>
				<td><%# GetDefinitions((System.Reflection.Assembly)Container.DataItem) %></td>
                </tr></ItemTemplate>
                <FooterTemplate></tbody></table></FooterTemplate>
            </asp:Repeater>
            <asp:Label ID="lblAssemblies" runat="server" />

			<% try { %>
			<table class="t"><thead><tr><td>NH Cache Region</td><td>Cache</td></tr></thead>
			<tbody>
			<% foreach (KeyValuePair<string, NHibernate.Cache.ICache> kvp in ((NHibernate.Impl.SessionFactoryImpl)N2.Context.Current.Resolve<N2.Persistence.NH.IConfigurationBuilder>().BuildSessionFactory()).GetAllSecondLevelCacheRegions()) { %>
				<tr><td><%= kvp.Key%></td><td><%= kvp.Value%></td></tr>
			<% } %>
			</tbody></table>
			<% } catch (Exception ex) { %>
			<pre><%= ex %></pre>
			<% } %>

			<% try { %>

			<% if (Request["showcache"] != null) { %>
			<table id="runtimecache" class="t"><thead><tr><td>Runtime Cache Key</td><td>Value</td></tr></thead>
			<tbody>
			<% foreach (System.Collections.DictionaryEntry c in Context.Cache) { if(Request["cachefilter"] != null && !c.Key.ToString().Contains(Request["cachefilter"])) continue; %>
				<tr><td><%= c.Key %></td><td><%= c.Value is System.Collections.DictionaryEntry ? ("<b>" + ((System.Collections.DictionaryEntry)c.Value).Key + "</b>: " + ((System.Collections.DictionaryEntry)c.Value).Value) : c.Value%></td></tr>
			<% } %>
			</tbody></table>
			<% } else { %>
			<table class="t"><thead><td colspan="2">Runtime Cache</td></thead><tbody><tr><th>Count</th><td><%= Context.Cache.Count %> <a href="?showcache=true#runtimecache">(Show)</a></td></tr></tbody></table>
			<% } %>

			<% } catch (Exception ex) { %>
			<pre><%= ex %></pre>
			<% } %>
        </div>
    </form>
</body>
</html>
