
What's NEW


* FIXED A BUG
* REWRITTEN DOCuMENTATION.... I hope now it's understandable  (see below for an excerpt of the new documentation)




/// <author> Michele Scaramal</author>
/// <summary>
/// Module to handle language internationalization 
/// 
/// BASIC FUNCTIONALITY:
/// Each time a request is received by the server this module will decide which language has to be served and will expose this 
/// information in the public property requestlanguage. Aspx pages then can know which language was requested by the client
/// reading that property.
/// 
/// How the property is set:
/// If we are in edit mode (we can recognize this reading the appSettings collection for the attribute edit) we read the information in a parameter in the request or if this information doesn't
/// exist we expose the default language value
/// If we are in browsing mode we expose the value read on the client's language cookie  if a language cookie is provided, otherwise we expose the default language.
///
/// 
/// 
/// HOW TO SET UP THE MODULE
/// This module is to be added to the web config
///   <httpModules>
///     <add name="LanguageSwitchingModule" type="LanguageSwitcherModule" />
///   </httpModules>
///  to allow the LanguageSwitcherAttribute to manage the request language.
/// </summary>
///  You will need to set also this app setting on the web config of your edit directory
///  to allow the module to discover if the content is requested for editing purposes 
///  rather than for showing purposes
///  <appSettings>
///        <add key="editmode" value="true" />
///  </appSettings>
/// 
/// 
/// 
/// At this time the browsing mode language management is handled by cookies but if you want to use sessions or something 
/// else you just need to subclass this class and override
/// 
/// private string getPersistentLanguage()
/// private void setPersistentLanguage(string language)
/// private boolean existPersistentLanguage()
/// 
/// see   region to handle persistent information about languages
/// 
