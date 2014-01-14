using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using N2.Linq;
using N2.Web.Mvc;
using N2.Web.Mvc.Html;

// General Information about an assembly is controlled through the following set of attributes. Change these attribute values to modify the information associated with an assembly.
[assembly: AssemblyTitle("N2.Extensions")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("N2.Extensions")]
[assembly: AssemblyCopyright("Copyright © 2006-2009 Crisitan Libardo")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible to COM components.  If you need to access a type in this assembly from  COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8857e546-b0bb-43b8-9c6d-5fd31b9d82e2")]

[assembly: InternalsVisibleTo("N2.Extensions.Tests")]

// N2.Linq
//[assembly: TypeForwardedTo(typeof(ContentQueryable<>))]
//[assembly: TypeForwardedTo(typeof(ContentQueryProvider))]
[assembly: TypeForwardedTo(typeof(EngineExtensions))]
[assembly: TypeForwardedTo(typeof(QueryableExtensions))]

// N2.Web.Mvc
[assembly: TypeForwardedTo(typeof(ActionResolver))]
[assembly: TypeForwardedTo(typeof(ContentController))]
[assembly: TypeForwardedTo(typeof(ContentController<>))]
[assembly: TypeForwardedTo(typeof(ContentOutputCacheAttribute))]
[assembly: TypeForwardedTo(typeof(ContentRoute))]
[assembly: TypeForwardedTo(typeof(ContentRoute<>))]
[assembly: TypeForwardedTo(typeof(ControllerMapper))]
[assembly: TypeForwardedTo(typeof(IControllerMapper))]
[assembly: TypeForwardedTo(typeof(RequestContextExtensions))]
[assembly: TypeForwardedTo(typeof(RouteExtensions))]
[assembly: TypeForwardedTo(typeof(Traversal))]
[assembly: TypeForwardedTo(typeof(ViewPageResult))]

[assembly: TypeForwardedTo(typeof(DroppableZoneExtensions))]
[assembly: TypeForwardedTo(typeof(DroppableZoneHelper))]
[assembly: TypeForwardedTo(typeof(ZoneExtensions))]
[assembly: TypeForwardedTo(typeof(ZoneHelper))]

[assembly: TypeForwardedTo(typeof(ControllerFactoryConfigurator))]
[assembly: TypeForwardedTo(typeof(ServiceLocatingControllerFactory))]
