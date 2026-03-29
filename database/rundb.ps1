$ErrorActionPreference = 'Stop'

$ContainerName    = 'northwind-postgres'
$Image            = 'postgres:17'
$PostgresUser     = 'postgres'
$PostgresPassword = 'postgres'
$PostgresDb       = 'northwind'
$HostPort         = '5440'

$ScriptDir  = Split-Path -Parent $MyInvocation.MyCommand.Path
$InitSql    = Join-Path $ScriptDir 'init.sql'
$SeedSql    = Join-Path $ScriptDir 'seed.sql'
$SeedBmpSql = Join-Path $ScriptDir 'seedbmp.sql'

$existing = docker ps -a --format '{{.Names}}' | Where-Object { $_ -eq $ContainerName }
if ($existing) {
    Write-Error "Error: container '$ContainerName' already exists.`nPlease delete it manually and run this script again."
    exit 1
}

docker run -d `
    --name $ContainerName `
    -e POSTGRES_USER=$PostgresUser `
    -e POSTGRES_PASSWORD=$PostgresPassword `
    -e POSTGRES_DB=$PostgresDb `
    -p "${HostPort}:5432" `
    $Image | Out-Null

Write-Host 'Waiting for PostgreSQL to be ready...'
do {
    Start-Sleep -Seconds 1
    $ready = docker exec $ContainerName pg_isready -U $PostgresUser -d $PostgresDb 2>&1
} until ($LASTEXITCODE -eq 0)

Write-Host 'Running init.sql...'
Get-Content $InitSql -Raw | docker exec -i $ContainerName psql -v ON_ERROR_STOP=1 -U $PostgresUser -d $PostgresDb
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host 'Running seed.sql...'
Get-Content $SeedSql -Raw | docker exec -i $ContainerName psql -v ON_ERROR_STOP=1 -U $PostgresUser -d $PostgresDb
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host 'Running seedbmp.sql...'
Get-Content $SeedBmpSql -Raw | docker exec -i $ContainerName psql -v ON_ERROR_STOP=1 -U $PostgresUser -d $PostgresDb
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Done. Database '$PostgresDb' is ready in container '$ContainerName'."
