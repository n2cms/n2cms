INSTALLATION
* Install the N2CMS.Search.Remote nuget package
* This is added to web.config:
	<n2><database>
		<search type="RemoteServer">
			<client sharedSecret="changemeHereAndOnServerConfig" />
* Run as administrator: RemoteSearchServer\GrantListenPortPermissionToServer.bat
* Run: RemoteSearchServer\N2.Search.Remote.Server.exe
* Go to the management UI and publish a page
* Search using N2's search API

CONFIGURE Shared Secret
* Exit the server
* Edit web.config on the website
* Change <client sharedSecret="..." to something really secret
* Edit RemoteSearchServer\N2.Search.Remote.Server.exe.config
* Change <server sharedSecret="..." to the same text
* Run: RemoteSearchServer\N2.Search.Remote.Server.exe

CONFIGURE Index Path
* Exit the server
* Edit RemoteSearchServer\N2.Search.Remote.Server.exe.config
* Change indexPath="C:\ProgramData\N2\SearchIndex\" to a path where the index should be stored, e.g. indexPath="D:\N2\SearchIndex\"
* Run: RemoteSearchServer\N2.Search.Remote.Server.exe
* Trigger re-index of all content

CONFIGURE Multiple Websites Sharing Same Server
* Edit web.config on the website
* Add instanceName attribute:
	<database>
		<search>
			<client instanceName="WebSite1" />
* Re-index all pages

REINDEXING pages
* In the mangement UI go to N2/Site Settings
* Press "Schedule index of all content"

MOVING Search Server
* XCOPY all files below RemoteSearchServer to a directory and server of choice
* Run: N2.Search.Remote.Server.exe
* Edit web.config on the website
* Change <client url="http://myserver:7850/" /> to any new server (localhost is default)

INSTALLING as Windows Service
* Run as administrator: InstallAsWindowsService.bat
* The service will start with windows

DEBUGGING
* Run: RemoteSearchServer\N2.Search.Remote.Server.exe
* type "debug" and press Enter: log output will now be displayed in the console window
* type "search <searchphrase>": search results will be displayed
* type "help": available server commands will be displayed
