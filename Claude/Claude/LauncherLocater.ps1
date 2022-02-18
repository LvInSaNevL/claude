Write-Output "this is a test"

$out = Get-ChildItem -Path C:\ -Include "steam.exe" -File -Recurse -ErrorAction SilentlyContinue | % { $_.FullName }

Write-Output $out