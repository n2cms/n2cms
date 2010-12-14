using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using N2.Security.Items;
using N2.Security;

// General Information about an assembly is controlled through the following set of attributes. Change these attribute values to modify the information associated with an assembly.
[assembly: AssemblyTitle("N2.Security")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("N2.Security")]
[assembly: AssemblyCopyright("Copyright © 2006-2009 Crisitan Libardo")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible to COM components.  If you need to access a type in this assembly from COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6bbc6cf1-8e9f-480b-abc4-3ea87f3fcd83")]

[assembly: TypeForwardedTo(typeof(N2.Security.Details.EditableRolesAttribute))]
[assembly: TypeForwardedTo(typeof(User))]
[assembly: TypeForwardedTo(typeof(UserList))]
[assembly: TypeForwardedTo(typeof(ContentMembershipProvider))]
[assembly: TypeForwardedTo(typeof(ContentProfileProvider))]
[assembly: TypeForwardedTo(typeof(ContentRoleProvider))]
[assembly: TypeForwardedTo(typeof(ItemBridge))]
