[CmdletBinding()]
param(
    [string]$OutputPath = ".env",
    [switch]$Force
)

$resolvedOutput = Join-Path -Path (Get-Location) -ChildPath $OutputPath

if ((Test-Path -LiteralPath $resolvedOutput) -and -not $Force) {
    Write-Error "The file '$resolvedOutput' already exists. Re-run with -Force to overwrite it."
    exit 1
}

$content = @(
    "POSTGRES_USER=admin"
    "POSTGRES_PASSWORD=admin"
    "POSTGRES_DB=postgres"
    "MONGO_INITDB_ROOT_USERNAME=root"
    "MONGO_INITDB_ROOT_PASSWORD=root"
    "MONGO_EXPRESS_USER=admin"
    "MONGO_EXPRESS_PASSWORD=admin"
    "JWT_SECRET_KEY=WorkForceHub-Local-Development-Secret-Key-2026"
    "MEDIA_INTERNAL_API_KEY=dev-media-service-key"
)

Set-Content -LiteralPath $resolvedOutput -Value $content -Encoding ascii
Write-Host "Generated local development env file at '$resolvedOutput'."
