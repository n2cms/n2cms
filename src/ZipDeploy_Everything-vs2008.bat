@echo off
ECHO DEPLOYING AND ZIPPING N2 TO 'Deploy' FOLDER

msbuild.bat /target:ZipDeploy

ECHO DONE!
PAUSE