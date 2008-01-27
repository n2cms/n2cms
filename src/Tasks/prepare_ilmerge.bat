@echo off

echo Merging dll's

"..\..\lib\ILMerge.exe" /ndebug /out:"N2.Edit.dll" "Deploy\Bin\N2.Edit.dll" /log "Deploy\Bin\N2.Edit.Export.dll" "Deploy\Bin\N2.Edit.Install.dll" "Deploy\Bin\N2.Edit.LinkTracker.dll" "Deploy\Bin\N2.Edit.Membership.dll" "Deploy\Bin\N2.Edit.Security.dll" "Deploy\Bin\N2.Edit.Trash.dll" "Deploy\Bin\N2.Edit.Versions.dll" "Deploy\Bin\N2.Edit.Wizard.dll" "Deploy\Bin\N2.Installation.dll"
del "Deploy\Bin\N2.Edit*.dll" 
move N2.Edit.dll Deploy\Bin\

echo Done

pause