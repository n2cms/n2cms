To try to fix the mess that moving to MVC 5 has created for some people, I have created a few branches in the n2cms/n2cms repository: 

__master__ : fixes which work against the 2.x and 3.x releases go here

__master-2.x__: master gets merged in here periodically, in preparation for 2.x releases (MVC4).

__master-3.x__: master gets merged in here periodically, in preparation for 3.x releases (MVC5); fixes that _only work with .NET 4.5_ or are otherwise _breaking changes_ can also go in here. Eventually, some (or all) of these fixes will make it back into master. 

This way, people can continue using MVC4. The MVC4 packages will be released under the 2.x version numbering. When you use Nuget, you will need to specify the -Version parameter to make sure you don't upgrade your site to 3.x. If you upgrade your site to 3.x, MVC4 will no longer work and .NET 4.5 is required. Please note, .NET 4.0 and earlier versions are no longer supported on MVC5. 

Eventually, we will not support 2.x anymore, so please try to upgrade to .NET 4.5 and N2CMS 3.x whenever you can. 

Thanks!
