<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="System.Security" %>
<%@ Import Namespace="System.Web.Compilation" %>

<script runat="server">
    private static readonly DateTime _utilityDate = new DateTime(2010, 2, 2);
    private static readonly string _utilityVersion = "v4";
    
    private static readonly Dictionary<string, string> _mvcFriendlyVersionNames = new Dictionary<string, string>() {
        { "1.0.30218.0", "ASP.NET MVC 1 Preview 2" },
        { "1.0.30508.0", "ASP.NET MVC 1 Preview 3" },
        { "1.0.30714.0", "ASP.NET MVC 1 Preview 4" },
        { "1.0.30826.0", "ASP.NET MVC 1 Preview 5" },
        { "1.0.31003.0", "ASP.NET MVC 1 Beta" },
        { "1.0.40112.0", "ASP.NET MVC 1 RC 1" },
        { "1.0.40128.0", "ASP.NET MVC 1 RC 1 Refresh" },
        { "1.0.40216.0", "ASP.NET MVC 1 RC 2" },
        { "1.0.40310.0", "ASP.NET MVC 1 RTM" },
        { "2.0.40724.0", "ASP.NET MVC 2 Preview 1" },
        { "2.0.41001.0", "ASP.NET MVC 2 Preview 2" },
        { "2.0.41110.0", "ASP.NET MVC 2 Beta" },
        { "2.0.41211.0", "ASP.NET MVC 2 RC 1" },
        { "2.0.50129.0", "ASP.NET MVC 2 RC 2" }
    };

    private static readonly Dictionary<string, string> _mvcFuturesDownloadUrls = new Dictionary<string, string>() {
        { "1.0.40310.0", "http://aspnet.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=24471#DownloadId=61773" }, // MVC 1 RTM
        { "2.0.40724.0", "http://aspnet.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=30886#DownloadId=77345" }, // MVC 2 Preview 1
        { "2.0.41001.0", "http://aspnet.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=33836#DownloadId=85929" }, // MVC 2 Preview 2
        { "2.0.41110.0", "http://aspnet.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=36054#DownloadId=93343" }, // MVC 2 Beta
        { "2.0.41211.0", "http://aspnet.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=37423#DownloadId=97581" }, // MVC 2 RC 1
        { "2.0.50129.0", "http://go.microsoft.com/fwlink/?LinkID=183045" } // MVC 2 RC 2
    };
    
    // Diagnostics routines
    private class DiagnosticsResults {
        public DiagnosticsResults() {
            EnvironmentInformation = GetEnvironmentInformation();
            AllAssemblies = BuildManager.GetReferencedAssemblies().OfType<Assembly>().Concat(AppDomain.CurrentDomain.GetAssemblies()).Distinct().OrderBy(o => o.FullName).ToArray();
            MvcAssemblies = AllAssemblies.Where(IsMvcAssembly).Select<Assembly, MvcAssemblyInformation>(GetMvcAssemblyInformation).ToArray();
            MvcFuturesAssemblies = AllAssemblies.Where(IsMvcFuturesAssembly).Select<Assembly, MvcAssemblyInformation>(GetMvcAssemblyInformation).ToArray();

            IsFuturesConflict = (MvcAssemblies.Length == 1 && MvcFuturesAssemblies.Length == 1 && MvcAssemblies[0].VersionActual != MvcFuturesAssemblies[0].VersionActual);
            IsError = (MvcAssemblies.Length != 1) || (MvcFuturesAssemblies.Length > 1) || IsFuturesConflict;
        }

        private static EnvironmentInformation GetEnvironmentInformation() {
            string iisVersion = HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"];
            if (String.IsNullOrEmpty(iisVersion)) {
                iisVersion = "{ not available }";
            }

            string processName = "{ not available }";
            try {
                // late binding so that LinkDemands are not triggered
                object currentProcess = typeof(Process).GetMethod("GetCurrentProcess", Type.EmptyTypes).Invoke(null, null);
                object processModule = typeof(Process).GetProperty("MainModule").GetValue(currentProcess, null);
                processName = (string)typeof(ProcessModule).GetProperty("ModuleName").GetValue(processModule, null);
            }
            catch { } // swallow exceptions

            return new EnvironmentInformation() {
                OperatingSystem = Environment.OSVersion,
                NetFrameworkVersion = Environment.Version,
                NetFrameworkBitness = IntPtr.Size * 8,
                ServerSoftware = iisVersion,
                WorkerProcess = processName,
                IsIntegrated = HttpRuntime.UsingIntegratedPipeline
            };
        }

        private static MvcAssemblyInformation GetMvcAssemblyInformation(Assembly assembly) {
            AssemblyFileVersionAttribute fileVersionAttr = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true /* inherit */).OfType<AssemblyFileVersionAttribute>().FirstOrDefault();

            string actualVersion = (fileVersionAttr != null) ? fileVersionAttr.Version : "Unknown version";
            string friendlyVersion;
            _mvcFriendlyVersionNames.TryGetValue(actualVersion, out friendlyVersion);
            if (String.IsNullOrEmpty(friendlyVersion)) {
                friendlyVersion = "Unknown version";
            }

            string codeBase = "{ not available }";
            try {
                codeBase = assembly.CodeBase;
            }
            catch (SecurityException) {
                // can't read code base in medium trust, so just skip
            }

            string deployment = (assembly.GlobalAssemblyCache) ? "GAC" : "bin";

            string futuresDownloadLocation;
            _mvcFuturesDownloadUrls.TryGetValue(actualVersion, out futuresDownloadLocation);
            if (String.IsNullOrEmpty(futuresDownloadLocation)) {
                futuresDownloadLocation = "http://aspnet.codeplex.com/";
            }

            return new MvcAssemblyInformation() {
                VersionActual = actualVersion,
                VersionFriendly = friendlyVersion,
                FullName = assembly.FullName,
                CodeBase = codeBase,
                Deployment = deployment,
                FuturesDownloadLocation = futuresDownloadLocation
            };
        }

        private static bool IsMvcAssembly(Assembly assembly) {
            return (String.Equals(assembly.ManifestModule.Name, "System.Web.Mvc.dll", StringComparison.OrdinalIgnoreCase)
                || (assembly.GetType("System.Web.Mvc.Controller", false /* throwOnError */) != null));
        }

        private static bool IsMvcFuturesAssembly(Assembly assembly) {
            return (String.Equals(assembly.ManifestModule.Name, "Microsoft.Web.Mvc.dll", StringComparison.OrdinalIgnoreCase));
        }

        public readonly EnvironmentInformation EnvironmentInformation;
        public readonly MvcAssemblyInformation[] MvcAssemblies;
        public readonly MvcAssemblyInformation[] MvcFuturesAssemblies;
        public readonly Assembly[] AllAssemblies;
        public readonly bool IsError;
        public readonly bool IsFuturesConflict;
    }

    private class EnvironmentInformation {
        public OperatingSystem OperatingSystem;
        public Version NetFrameworkVersion;
        public int NetFrameworkBitness;
        public string ServerSoftware;
        public string WorkerProcess;
        public bool IsIntegrated;
    }

    private class MvcAssemblyInformation {
        public string VersionFriendly;
        public string VersionActual;
        public string FullName;
        public string CodeBase;
        public string Deployment;
        public string FuturesDownloadLocation;
    } 

    private static string AE(object input) {
        return HttpUtility.HtmlAttributeEncode(Convert.ToString(input, CultureInfo.InvariantCulture));
    }

    private static string E(object input) {
        return HttpUtility.HtmlEncode(Convert.ToString(input, CultureInfo.InvariantCulture));
    }
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ASP.NET MVC Diagnostics Utility</title>
    <style type="text/css">
        .error 
        {
            font-weight: bold;
            color: Red;
        }
        
        .box 
        {
            border-width: thin;
            border-style: solid;
            padding: .2em 1em .2em 1em;
            background-color: #dddddd;
        }
        
        .errorInset
        {
            padding: 1em;
            background-color: #ffbbbb;
        }
        
        body 
        {
            font-family: Calibri, Helvetica;
        }
    </style>
</head>

<%
    DiagnosticsResults results = new DiagnosticsResults();
    
    Action<MvcAssemblyInformation> outputAssemblyInfo = assemblyInfo => {
        %>
        <p>
            <b>Assembly version:</b> <%= E(assemblyInfo.VersionFriendly) %> (<%= E(assemblyInfo.VersionActual) %>)<br />
            <b>Full name:</b> <%= E(assemblyInfo.FullName) %><br />
            <b>Code base:</b> <%= E(assemblyInfo.CodeBase) %><br />
            <b>Deployment:</b> <%= E(assemblyInfo.Deployment) %>-deployed
        </p>
        <%
    };
            %>

<body>
    <h1>Microsoft ASP.NET MVC Diagnostics Information</h1>
    <p>
        This page is designed to help diagnose common errors related to mismatched or conflicting ASP.NET MVC binaries.
        If a known issue is identified, it will be displayed below in <span class="error">red</span> text.
    </p>
    <p>
        For questions or problems with ASP.NET MVC or this utility, please visit the ASP.NET MVC forums at <a href="http://forums.asp.net/1146.aspx">http://forums.asp.net/1146.aspx</a>.
    </p>
    <% if (results.IsError) { %><p class="error">Errors were found.  Please see below for more information.</p><% } %>
    <h2>Environment Information</h2>
    <div class="box">
        <p>
            <b>Operating system:</b> <%= E(results.EnvironmentInformation.OperatingSystem) %><br />
            <b>.NET Framework version:</b> <%= E(results.EnvironmentInformation.NetFrameworkVersion) %> (<%= E(results.EnvironmentInformation.NetFrameworkBitness) %>-bit)<br />
            <b>Web server:</b> <%= E(results.EnvironmentInformation.ServerSoftware) %><br />
            <b>Integrated pipeline:</b> <%= E(results.EnvironmentInformation.IsIntegrated) %><br />
            <b>Worker process:</b> <%= E(results.EnvironmentInformation.WorkerProcess) %>
        </p>
    </div>
    <h2>ASP.NET MVC Assembly Information (System.Web.Mvc.dll)</h2>
    <div class="box">
        <% if (results.MvcAssemblies.Length == 0) { %><p class="error">An ASP.NET MVC assembly has not been loaded into this application.</p><% } %>
        <% if (results.MvcAssemblies.Length > 1) { %><p class="error">Multiple ASP.NET MVC assemblies have been loaded into this application.</p><% } %>
        <% foreach (MvcAssemblyInformation info in results.MvcAssemblies) { outputAssemblyInfo(info); } %>
    </div>
    <h2>ASP.NET MVC Futures Assembly Information (Microsoft.Web.Mvc.dll)</h2>
    <div class="box">
        <% if (results.MvcFuturesAssemblies.Length == 0) { %>
            <p>
                An ASP.NET MVC Futures assembly has not been loaded into this application.
                <% if (results.MvcAssemblies.Length == 1) { %>If desired, you can download <%= E(results.MvcAssemblies[0].VersionFriendly) %> Futures from <a href="<%= AE(results.MvcAssemblies[0].FuturesDownloadLocation) %>"><%= E(results.MvcAssemblies[0].FuturesDownloadLocation) %></a>.<% } %>
            </p>
        <% } %>
        <% if (results.MvcFuturesAssemblies.Length > 1) { %><p class="error">Multiple ASP.NET MVC Futures assemblies have been loaded for this application.</p><% } %>
        <% if (results.IsFuturesConflict) { %>
            <div>
                <p class="error">Mismatched versions of ASP.NET MVC and ASP.NET MVC Futures are loaded.</p>
                <p class="errorInset">
                    Loaded version of ASP.NET MVC is: <%= E(results.MvcAssemblies[0].VersionFriendly) %> (<%= E(results.MvcAssemblies[0].VersionActual) %>)<br />
                    Loaded version of ASP.NET MVC Futures is: <%= E(results.MvcFuturesAssemblies[0].VersionFriendly) %> (<%= E(results.MvcFuturesAssemblies[0].VersionActual) %>)<br />
                    Download <%= E(results.MvcAssemblies[0].VersionFriendly) %> Futures from <a href="<%= AE(results.MvcAssemblies[0].FuturesDownloadLocation) %>"><%= E(results.MvcAssemblies[0].FuturesDownloadLocation) %></a>.
                </p>
            </div>
        <% } %>
        <% foreach (MvcAssemblyInformation info in results.MvcFuturesAssemblies) { outputAssemblyInfo(info); } %>
    </div>
    <h2>All Loaded Assemblies</h2>
    <div class="box">
        <p><%= E(results.AllAssemblies.Length) %> assemblies are loaded.</p>
        <ul>
            <% foreach (Assembly assembly in results.AllAssemblies) { %><li><%= E(assembly) %></li><% } %>
        </ul>
    </div>
    <p>
        <b>Diagnostics version:</b> <%= E(_utilityDate) %> <%= E(_utilityVersion) %><br />
        <b>Report generated on:</b> <%= E(DateTime.Now) %>
    </p>
</body>
</html>
