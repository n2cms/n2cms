== Controllers ==

Controllers controlling content items that are routed through via the content route.

== Models ==

Models used by to dinamico controllers and views.

== Registrations ==

Registrations define what is editable in the management UI. These extend any declarations 
on the models themselves.

== Themes ==

This is the themes root folder. When no theme is selected or if a theme doesn't
define a certain view or css the resource is retrieved from /Dinamico/Themes/Default/...
When a theme is used and a resource exists in /Dinamico/Themes/[SelectedThemeName/... 
this is chosen before the default resource.

== Simple Parts & Pages ==

You can quickly add simple "template first" parts or pages by adding a single 
cshtml file to a directory:
* Parts: /Views/ContentParts/*.cshtml
* Pages: /View/ContentPages/*.cshtml
Look at /Dinamico/Themes/Default/Views/Content[Parts|Pages] for inspiration
