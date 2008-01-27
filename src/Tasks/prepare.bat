@echo off

echo Creating folders
mkdir .\Deploy
mkdir .\Deploy\Edit
mkdir .\Deploy\Bin

@echo Deleting bin & obj folders
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Export\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Install\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\LinkTracker\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Membership\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Security\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Trash\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Versions\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Wizard\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\obj
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Export\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Install\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\LinkTracker\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Membership\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Security\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Trash\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Versions\bin
rmdir /S/Q ..\Core\N2DevelopmentWeb\Edit\Wizard\bin

echo Copying edit interface
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.ascx .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.aspx .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.config .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.css .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.gif .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.htm .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.html .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.js .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.master .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.png .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.resx .\Deploy\Edit\
xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.txt .\Deploy\Edit\
move .\Deploy\Edit\App_GlobalResources .\Deploy\
move .\Deploy\Edit\App_GlobalResources\* .\Deploy\App_GlobalResources\

echo Copying dlls
xcopy /Y "..\..\lib\Iesi.Collections.dll" .\Deploy\Bin
xcopy /Y "..\..\lib\Castle.Core.dll" .\Deploy\Bin
xcopy /Y "..\..\lib\Castle.DynamicProxy2.dll" .\Deploy\Bin
xcopy /Y "..\..\lib\Castle.DynamicProxy.dll" .\Deploy\Bin
xcopy /Y "..\..\lib\Castle.MicroKernel.dll" .\Deploy\Bin
xcopy /Y "..\..\lib\Castle.Windsor.dll" .\Deploy\Bin
xcopy /Y "..\..\lib\log4net.dll" .\Deploy\Bin
xcopy /Y "..\..\lib\NHibernate.Caches.SysCache.dll" .\Deploy\Bin
xcopy /Y "..\..\lib\NHibernate.dll" .\Deploy\Bin
xcopy /Y "..\Core\N2DevelopmentWeb\bin\N2.dll" .\Deploy\Bin
xcopy /Y "..\Core\N2DevelopmentWeb\bin\N2.Edit*.dll" .\Deploy\Bin
xcopy /Y "..\Core\N2DevelopmentWeb\bin\N2.Parts.dll" .\Deploy\Bin

echo Copying documentation

echo Done!
Pause