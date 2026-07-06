$ErrorActionPreference = "Stop"

function Stop-PortProcess {
    param(
        [Parameter(Mandatory = $true)]
        [int]$Port
    )

    $matches = netstat -ano | Select-String ":$Port"
    if (-not $matches) {
        Write-Host "No hay proceso escuchando en el puerto $Port." -ForegroundColor Yellow
        return
    }

    $line = $matches | Select-Object -Last 1
    $parts = ($line.ToString() -split "\s+") | Where-Object { $_ }
    $procId = $parts[-1]

    if (-not $procId) {
        Write-Host "No fue posible resolver el PID del puerto $Port." -ForegroundColor Yellow
        return
    }

    cmd /c taskkill /PID $procId /F | Out-Null
    Write-Host "Se detuvo el proceso del puerto $Port (PID $procId)." -ForegroundColor Green
}

Stop-PortProcess -Port 8000
Stop-PortProcess -Port 5083
