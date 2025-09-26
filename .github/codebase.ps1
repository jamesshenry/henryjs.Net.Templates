$repoRoot = Resolve-Path "$PSScriptRoot/.."

Write-Host $repoRoot

$sourceDirectory = Join-Path $repoRoot 'templates'
Write-Host $sourceDirectory

$outputFile = "$repoRoot/.github/instructions/codebase.txt"
Write-Host $outputFile

# Build directory tree
$directoryTree = Get-ChildItem -Path $sourceDirectory -Recurse -File |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } | ForEach-Object {
        $indent = '  ' * ($_.FullName.Split('\').Length - $sourceDirectory.Split('\').Length)
        "$indent- $($_.Name)"
    } | Out-String

$contextBlock = "$directoryTree`n# --- Start of Code Files ---`n`n"
Set-Content -Path $outputFile -Value $contextBlock

# Extension -> language mapping
$languageMap = @{
    '.cs'     = 'csharp'
    '.ps1'    = 'powershell'
    '.json'   = 'json'
    '.xml'    = 'xml'
    '.yml'    = 'yaml'
    '.yaml'   = 'yaml'
    '.md'     = 'markdown'
    '.sh'     = 'bash'
    '.ts'     = 'typescript'
    '.js'     = 'javascript'
    '.csproj' = 'xml'
}

# Grab all files except bin/obj
$allFiles = Get-ChildItem -Path $sourceDirectory -Recurse -File -Exclude *.ico, *.png |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

foreach ($file in $allFiles) {
    $relativePath = $file.FullName.Substring($PWD.Path.Length + 1)

    # Determine language based on extension (default: text)
    $ext = $file.Extension.ToLower()
    $lang = if ($languageMap.ContainsKey($ext)) { $languageMap[$ext] } else { 'text' }

    $filePathHeader = @"
// File: $relativePath
"@

    $codeBlockStart = "``````$lang"
    $codeBlockEnd = "```````n"

    $fileContent = Get-Content -Path $file.FullName | Out-String
    $formattedContent = @"
$filePathHeader

$codeBlockStart
$fileContent
$codeBlockEnd
"@
    Add-Content -Path $outputFile -Value $formattedContent
}
