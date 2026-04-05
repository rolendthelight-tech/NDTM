rem test
FOR %%d IN (bin obj) DO (
FOR /F "tokens=*" %%f IN ('DIR /A:D /B /S *%%d') DO (
	IF "%%d"=="%%~nxf" (
		RMDIR /S /Q "%%f"
)))

pause