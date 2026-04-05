"%WIX%\bin\heat.exe" dir "components" -o TargetFiles.wxs -sreg -srd -v -ag -suid -cg TargetFiles -dr AssemblyFolder
"%WIX%\bin\TargetFilesFilter.exe" TargetFiles.wxs

pause

