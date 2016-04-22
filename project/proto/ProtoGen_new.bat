mkdir output
del output\*.* /q
for %%i in (*.proto) do ProtoGen.exe -i:%%i -o:output\%%~ni.cs -ns:Proto_JavaNet

COPY CoreOnly\ios\protobuf-net.dll output\

C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe /out:output\ProtoModel.dll /r:output\protobuf-net.dll /t:library output\*.cs

Precompile\precompile.exe output\ProtoModel.dll -o:output\ProtoSerializer.dll -t:ProtoSerializer

COPY output\*.dll ..\client\Assets\Protobuf\
COPY output\*.dll ..\tools\ActionEditor\Proto\

pause
