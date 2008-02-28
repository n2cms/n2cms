@ECHO OFF
ECHO BUILDING...
MSBuild ..\Templates\N2.Templates-vs2008.sln > Temp\build.templates.log
ECHO DONE
pause