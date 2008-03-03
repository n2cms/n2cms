@echo off
ECHO DEPLOYING N2...

msbuild.bat /target:PrepareDependencies

ECHO DONE!
PAUSE