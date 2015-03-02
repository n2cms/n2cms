# N2CMS Developer Installation Instructions
(to be uploaded to wiki)
 
First, download the release and extract to a path such as C:\N2Packages (this example path is used below)

Next, decide whether you want to use the ZIP or NOZIP management packs. 

### Before doing anything listed on this page
*Back up your project before installing or uninstalling any N2CMS nuget packages*.

### Which management pack should I choose? 

You can choose whichever management pack is right for you. Consider the following benefits and drawbacks.

* The benefit of the ZIP management pack is that it is a single file for the N2 Management Interface. However, if you store your website in a source control depot (e.g. Git repo), you end up with a lot of bloat as the ~5 MB N2.zip file gets upgraded over time. You also need to upload the entire N2.zip when you update it. 

* The benefit of the NOZIP management pack is that you can take advantage of Web Deploy incremental uploads, as well as more efficient source control storage as the files are installed separately (not extracted). Additionally, the Zip Virtual Path Provider is not installed, which means that less memory is used by N2CMS. 


### Switching Management Packs
You can switch management packs (e.g. ZIP to NOZIP or visa-versa) at any time by first *uninstalling* any current management pack, and then installing the desired management pack. 

### Multiple Management Packs
You MUST NOT install multiple management packs on your N2 installation. This is not supported and you will likely break your N2 installation. The only supported recourse in this situation is to *start a new project*! Back up your N2 installation before installing any management packs.

### Upgrading from N2CMS 2.5 and earlier


```
Uninstall-Package n2cms.dinamico
Uninstall-Package n2cms
Uninstall-Package n2cms.management
```

## Installation Instructions 

### Example: Dinamico Template Pack

For the NOZIP option: 
```
Install-Package -Source C:\N2Packages -Name n2cms.dinamico
Install-Package -Source C:\N2Packages -Name n2cms.management.nozip
```

For the ZIP option: 
```
Install-Package -Source C:\N2Packages -Name n2cms.dinamico
Install-Package -Source C:\N2Packages -Name n2cms.management.zip
```

## Upgrading
You can upgrade just the management pack. The N2CMS libraries will be updated 
automatically if required. You can also upgrade the template packs. Run the relevant
Install-Package command as listed above. 

```
Install-Package -Source C:\N2Packages -Name n2cms.management.nozip
```
OR
```
Install-Package -Source C:\N2Packages -Name n2cms.management.zip
```
