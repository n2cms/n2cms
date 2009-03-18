@ECHO Assuming msbuild location: %windir%\Microsoft.NET\Framework\v3.5\MSBuild.exe

%windir%\Microsoft.NET\Framework\v3.5\MSBuild.exe msbuild.proj /maxcpucount /p:Configuration=Release;Platform="Any CPU" %*