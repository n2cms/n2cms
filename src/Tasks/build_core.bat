@echo off
mkdir Temp
copy ..\..\lib\NHibernate.Caches.SysCache.dll ..\Core\N2DevelopmentWeb\bin
echo BUILDING...
MSBuild ..\Core\N2-vs2008.sln > Temp\build.core.log
echo DONE
pause