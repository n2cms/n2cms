[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{3F230DAB-AAB4-41DE-A550-98E3ADAB35E2}
AppName=N2CMS NuGet Packages
AppVersion=2.6.3.3
AppPublisher=Benjamin Herila
AppPublisherURL=http://n2cms.com
AppSupportURL=http://n2cms.com
AppUpdatesURL=http://n2cms.com
DefaultDirName=c:\n2cms
DefaultGroupName=N2CMS NuGet Packages
DisableProgramGroupPage=yes
LicenseFile=..\docs\License.txt
InfoAfterFile=..\docs\n2_readme.txt
OutputBaseFilename=n2setup
SetupIconFile=..\src\Mvc\Dinamico\favicon.ico
SolidCompression=yes
WizardSmallImageFile=..\docs\n2logo.bmp
DisableWelcomePage=True
DisableReadyPage=True
Uninstallable=yes
OutputDir=..
CompressionThreads=2
LZMANumBlockThreads=8
InternalCompressLevel=ultra
PrivilegesRequired=lowest
Compression=lzma2/max
LZMAUseSeparateProcess=yes
MinVersion=0,6.0.6001sp2
DisableFinishedPage=True
UninstallDisplayIcon={app}\favicon.ico
UninstallDisplayName=N2CMS NuGet Packages
CreateUninstallRegKey=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\src\Mvc\Dinamico\favicon.ico"; DestDir: "{app}";
Source: "..\output\*"; DestDir: "{app}"; Flags: recursesubdirs;

[ThirdParty]
UseRelativePaths=True
