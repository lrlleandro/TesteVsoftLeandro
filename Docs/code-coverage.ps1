$solutionPath = "../"
$resultsDir = "./TestResults/Coverage"
$reportDir = "./TestResults/Report"
$reportPattern = "**/code-coverage.xml"

if (Test-Path $resultsDir) { Remove-Item $resultsDir -Recurse -Force }
if (Test-Path $reportDir) { Remove-Item $reportDir -Recurse -Force }

New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null

dotnet test $solutionPath `
    --collect:"XPlat Code Coverage" `
    --results-directory:$resultsDir `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura `
    --verbosity normal

$coverageFiles = Get-ChildItem -Path $resultsDir -Recurse -Filter "coverage.cobertura.xml" | Select-Object -ExpandProperty FullName

if (-not $coverageFiles) {
    Write-Host "Nenhum arquivo de cobertura encontrado." -ForegroundColor Yellow
}

$reportsArg = ($coverageFiles -join ";")

reportgenerator `
    -reports:$reportsArg `
    -targetdir:$reportDir `
    -reporttypes:Html

Write-Host "`Relatorio consolidado gerado em: $reportDir" -ForegroundColor Green
