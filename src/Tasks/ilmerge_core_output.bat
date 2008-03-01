@echo off

@echo Merging dll's
@mkdir Temp
@"..\..\lib\ILMerge.exe" /ndebug /out:"Temp\N2.Edit.dll" "..\Output\Core\Bin\N2.Edit.dll" /log "..\Output\Core\Bin\N2.Edit.Export.dll" "..\Output\Core\Bin\N2.Edit.Install.dll" "..\Output\Core\Bin\N2.Edit.LinkTracker.dll" "..\Output\Core\Bin\N2.Edit.Membership.dll" "..\Output\Core\Bin\N2.Edit.Trash.dll" "..\Output\Core\Bin\N2.Edit.Wizard.dll" "..\Output\Core\Bin\N2.Installation.dll" > Temp\merge.log
@del "..\Output\Core\Bin\N2.Edit*.dll" 
@move Temp\N2.Edit.dll ..\Output\Core\Bin\

@echo Done

@pause