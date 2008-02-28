@echo off

@echo CREATING FOLDER STRUCTURE
@mkdir Temp
@mkdir ..\Output
@mkdir ..\Output\Core
@mkdir ..\Output\Core\Edit
@mkdir ..\Output\Core\Bin

@echo COPYING EDIT INTERFACE FILES
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.ascx ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.aspx ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.config ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.css ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.gif ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.png ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.jpg ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.htm ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.html ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.js ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.master ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.png ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.resx ..\Output\Core\Edit\ >> Temp\output.core.log
@xcopy /s/Y ..\Core\N2DevelopmentWeb\Edit\*.ashx ..\Output\Core\Edit\ >> Temp\output.core.log

@echo MOVING GLOBAL RESOURCES
@move ..\Output\Core\Edit\App_GlobalResources ..\Output\Core\ >> Temp\output.core.log
@move ..\Output\Core\Edit\App_GlobalResources\* ..\Output\Core\App_GlobalResources\ >> Temp\output.core.log

@echo COPYING BINARIES
@xcopy /Y "..\..\lib\Iesi.Collections.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\..\lib\Castle.Core.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\..\lib\Castle.DynamicProxy2.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\..\lib\Castle.DynamicProxy.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\..\lib\Castle.MicroKernel.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\..\lib\Castle.Windsor.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\..\lib\log4net.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\..\lib\NHibernate.Caches.SysCache.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\..\lib\NHibernate.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\Core\N2DevelopmentWeb\bin\N2.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\Core\N2DevelopmentWeb\bin\N2.Edit*.dll" ..\Output\Core\Bin >> Temp\output.core.log
@xcopy /Y "..\Core\N2DevelopmentWeb\bin\N2.Parts.dll" ..\Output\Core\Bin >> Temp\output.core.log

@echo DONE!
@Pause