@echo off

@echo CREATING FOLDER STRUCTURE
@mkdir Temp
@mkdir ..\Output
@mkdir ..\Output\Templates
@mkdir ..\Output\Templates\Edit
@mkdir ..\Output\Templates\Bin

@echo COPYING TEMPLATE USER INTERFACE FILES
@xcopy /s/Y ..\Templates\UI\*.ascx ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.aspx ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.config ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.css ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.gif ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.png ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.htm ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.html ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.js ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.master ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.png ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.resx ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.mdf ..\Output\Templates\ >> Temp\deploy_templates.log
@xcopy /s/Y ..\Templates\UI\*.ldf ..\Output\Templates\ >> Temp\deploy_templates.log

@echo COPYING TEMPLATE BINARIES
@xcopy /Y "..\Templates\UI\bin\*.dll" ..\Output\Templates\Bin\ >> Temp\deploy_templates.log

@echo DONE!
@Pause