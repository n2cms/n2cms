// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project. 
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc. 
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File". 
// You do not need to add suppressions to this file manually. 

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Web.Mvc.TempDataDictionary.#System.Collections.Generic.ICollection`1<System.Collections.Generic.KeyValuePair`2<System.String,System.Object>>.Contains(System.Collections.Generic.KeyValuePair`2<System.String,System.Object>)",
    Justification = "There are no defined scenarios for wanting to derive from this class, but we don't want to prevent it either.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Web.Mvc.TempDataDictionary.#System.Collections.Generic.ICollection`1<System.Collections.Generic.KeyValuePair`2<System.String,System.Object>>.CopyTo(System.Collections.Generic.KeyValuePair`2<System.String,System.Object>[],System.Int32)",
    Justification = "There are no defined scenarios for wanting to derive from this class, but we don't want to prevent it either.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "System.Web.Mvc.TempDataDictionary.#System.Collections.Generic.ICollection`1<System.Collections.Generic.KeyValuePair`2<System.String,System.Object>>.IsReadOnly",
    Justification = "There are no defined scenarios for wanting to derive from this class, but we don't want to prevent it either.")]
