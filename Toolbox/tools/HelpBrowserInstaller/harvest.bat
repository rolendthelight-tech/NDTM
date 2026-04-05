"%WIX%\bin\heat.exe" dir "..\ATHelpBrowser\bin\Release" -var var.ATHelpBrowser.TargetDir -o TargetFiles.wxs -sreg -srd -v -ag -suid -cg TargetFiles -dr INSTALLLOCATION
"%WIX%\bin\TargetFilesFilter.exe" TargetFiles.wxs vshost\.exe v9\.2\.xml TestData Logs

pause

