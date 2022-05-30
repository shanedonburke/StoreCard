:: Compiles every shader file using the fxc program, which is expected to exist in the specified directory
for %%f in (Effects\Shaders\*.fx) do (
  echo Compiling %%~nf.fx
  "%ProgramFiles(x86)%\Windows Kits\10\bin\10.0.22000.0\x86\fxc.exe" /O0 /Fc /Zi /T ps_3_0 /Fo Effects\bin\%%~nf.ps Effects\Shaders\%%~nf.fx
)
:: Delete intermediary files we don't need
del Effects\Shaders\*.cod
echo Finished compiling shaders.
