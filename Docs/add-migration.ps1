param([string]$MigrationName)
dotnet ef migrations add $MigrationName -p "$PSScriptRoot\..\TesteVsoft.Infrastructure" -s "$PSScriptRoot\..\TesteVsoft.MigrationService"