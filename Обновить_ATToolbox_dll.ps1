# После сборки проекта ATToolbox скопировать свежую DLL в папку ARM (откуда АРМ берёт ссылки и контент).
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Copy-Item -Path (Join-Path $root "bin\Debug\ATToolbox.dll") -Destination (Join-Path $root "ARM\ATToolbox.dll") -Force
Copy-Item -Path (Join-Path $root "bin\Debug\ATToolbox.pdb") -Destination (Join-Path $root "ARM\ATToolbox.pdb") -Force -ErrorAction SilentlyContinue
$res = Join-Path $root "bin\Debug\ru\ATToolbox.resources.dll"
if (Test-Path $res) {
  $destDir = Join-Path $root "ARM\ru"
  New-Item -ItemType Directory -Path $destDir -Force | Out-Null
  Copy-Item -Path $res -Destination (Join-Path $destDir "ATToolbox.resources.dll") -Force
}
Write-Host "Готово: ARM\ATToolbox.dll обновлён из bin\Debug."
