path "C:\Program Files (x86)\MSBuild\14.0\Bin\";%windir%\Microsoft.NET\Framework\v4.0.30319;%PATH%

@IF NOT EXIST MSBuild.exe @ECHO COULDN'T FIND MSBUILD (Is .NET 4 installed?)

msbuild n2.proj /maxcpucount %*