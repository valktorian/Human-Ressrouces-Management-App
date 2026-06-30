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

function New-Base64Secret {
    param(
        [Parameter(Mandatory = $true)]
        [int]$ByteCount
    )

    $bytes = New-Object byte[] $ByteCount
    $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()

    try {
        $rng.GetBytes($bytes)
        return [Convert]::ToBase64String($bytes)
    }
    finally {
        $rng.Dispose()
    }
}

$jwtSecret = New-Base64Secret -ByteCount 64
$mediaInternalApiKey = New-Base64Secret -ByteCount 32

$content = @(
    "POSTGRES_USER=admin"
    "POSTGRES_PASSWORD=admin"
    "POSTGRES_DB=postgres"
    "MONGO_INITDB_ROOT_USERNAME=root"
    "MONGO_INITDB_ROOT_PASSWORD=root"
    "MONGO_EXPRESS_USER=admin"
    "MONGO_EXPRESS_PASSWORD=admin"
    "JWT_SECRET_KEY=$jwtSecret"
    "MEDIA_INTERNAL_API_KEY=$mediaInternalApiKey"
)

Set-Content -LiteralPath $resolvedOutput -Value $content -Encoding ascii
Write-Host "Generated local development env file at '$resolvedOutput'."
