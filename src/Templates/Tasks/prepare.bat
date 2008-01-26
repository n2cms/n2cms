@echo off

echo Creating folders
mkdir .\Deploy

echo Copying edit interface
xcopy /s/Y ..\UI\*.dll .\Deploy\
xcopy /s/Y ..\UI\*.ascx .\Deploy\
xcopy /s/Y ..\UI\*.aspx .\Deploy\
xcopy /s/Y ..\UI\*.config .\Deploy\
xcopy /s/Y ..\UI\*.css .\Deploy\
xcopy /s/Y ..\UI\*.gif .\Deploy\
xcopy /s/Y ..\UI\*.htm .\Deploy\
xcopy /s/Y ..\UI\*.html .\Deploy\
xcopy /s/Y ..\UI\*.js .\Deploy\
xcopy /s/Y ..\UI\*.master .\Deploy\
xcopy /s/Y ..\UI\*.png .\Deploy\
xcopy /s/Y ..\UI\*.resx .\Deploy\
xcopy /s/Y ..\UI\*.txt .\Deploy\

Pause