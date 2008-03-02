@echo off
ECHO DEPLOYING N2...

msbuild.bat /target:Deploy

ECHO DONE!
PAUSE