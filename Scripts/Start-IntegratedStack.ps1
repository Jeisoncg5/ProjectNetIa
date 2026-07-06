$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$smartInventoryRoot = Join-Path (Split-Path -Parent $projectRoot) "smartinventory-ai-core-main"
$dotnetUrl = "http://localhost:5083/"
$fastApiUrl = "http://127.0.0.1:8000/"
$fastApiPort = 8000

function Test-HealthEndpoint {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Url,
        [int]$Attempts = 20,
        [int]$DelaySeconds = 2
    )

    for ($attempt = 1; $attempt -le $Attempts; $attempt++) {
        try {
            Invoke-WebRequest -UseBasicParsing -Uri $Url | Out-Null
            return $true
        } catch {
            Start-Sleep -Seconds $DelaySeconds
        }
    }

    return $false
}

function Get-PortListenerPid {
    param(
        [Parameter(Mandatory = $true)]
        [int]$Port
    )

    $matches = netstat -ano | Select-String ":$Port"
    if (-not $matches) {
        return $null
    }

    $line = $matches | Select-Object -Last 1
    $parts = ($line.ToString() -split "\s+") | Where-Object { $_ }
    if ($parts.Count -eq 0) {
        return $null
    }

    return $parts[-1]
}

if (-not (Test-Path -LiteralPath (Join-Path $projectRoot "Api\Api.csproj"))) {
    throw "No se encontro Api\Api.csproj. Ejecuta este script desde el repositorio ProjectNetIa."
}

if (-not (Test-Path -LiteralPath $smartInventoryRoot)) {
    throw "No se encontro el workspace smartinventory-ai-core-main en $smartInventoryRoot."
}

$smartInventoryVenv = Join-Path $smartInventoryRoot ".venv\Scripts\python.exe"
$smartInventoryEnv = Join-Path $smartInventoryRoot ".env"

if (-not (Test-Path -LiteralPath $smartInventoryVenv)) {
    throw "No se encontro el entorno virtual de SmartInventory en $smartInventoryVenv."
}

if (-not (Test-Path -LiteralPath $smartInventoryEnv)) {
    throw "No se encontro el archivo .env de SmartInventory en $smartInventoryEnv."
}

$dotnetRunning = $false
$fastApiRunning = $false

try {
    Invoke-WebRequest -UseBasicParsing -Uri $dotnetUrl | Out-Null
    $dotnetRunning = $true
} catch {
}

if (-not $dotnetRunning) {
    Write-Host "Iniciando backend .NET..." -ForegroundColor Cyan
    Start-Process -FilePath "dotnet" `
        -ArgumentList "run", "--project", "Api" `
        -WorkingDirectory $projectRoot `
        -WindowStyle Hidden | Out-Null
} else {
    Write-Host "Backend .NET ya estaba en ejecucion." -ForegroundColor Yellow
}

$fastApiPid = Get-PortListenerPid -Port $fastApiPort
if ($fastApiPid) {
    $fastApiRunning = $true
}

if (-not $fastApiRunning) {
    Write-Host "Iniciando FastAPI..." -ForegroundColor Cyan
    Start-Process -FilePath $smartInventoryVenv `
        -ArgumentList "-m", "uvicorn", "main:app", "--host", "127.0.0.1", "--port", "8000" `
        -WorkingDirectory $smartInventoryRoot `
        -WindowStyle Hidden | Out-Null
} else {
    Write-Host "FastAPI ya estaba en ejecucion en el puerto 8000." -ForegroundColor Yellow
}

Write-Host "Esperando salud de servicios..." -ForegroundColor Cyan

$dotnetHealthy = Test-HealthEndpoint -Url $dotnetUrl
$fastApiHealthy = Test-HealthEndpoint -Url $fastApiUrl

if (-not $dotnetHealthy) {
    throw "El backend .NET no respondio en $dotnetUrl."
}

if (-not $fastApiHealthy) {
    throw "FastAPI no respondio en $fastApiUrl."
}

Write-Host "Servicios listos." -ForegroundColor Green
Write-Host "  .NET API : $dotnetUrl" -ForegroundColor Yellow
Write-Host "  FastAPI  : $fastApiUrl" -ForegroundColor Yellow
Write-Host "  Chat .NET: http://localhost:5083/api/chat/message" -ForegroundColor Yellow
