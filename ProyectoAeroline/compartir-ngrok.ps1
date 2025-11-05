# Script PowerShell para compartir la aplicación usando ngrok

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Compartir Aplicacion con ngrok" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que ngrok existe
$ngrokPath = "C:\ngrok\ngrok.exe"
if (-not (Test-Path $ngrokPath)) {
    Write-Host "❌ ngrok no encontrado en $ngrokPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor:" -ForegroundColor Yellow
    Write-Host "1. Descarga ngrok desde: https://ngrok.com/download" -ForegroundColor White
    Write-Host "2. Extrae ngrok.exe a C:\ngrok\" -ForegroundColor White
    Write-Host "3. Ejecuta este script nuevamente" -ForegroundColor White
    Write-Host ""
    pause
    exit 1
}

Write-Host "Tu aplicacion debe estar corriendo en uno de estos puertos:" -ForegroundColor Yellow
Write-Host "  - http://localhost:5244" -ForegroundColor White
Write-Host "  - https://localhost:7014" -ForegroundColor White
Write-Host ""

$port = Read-Host "Ingresa el puerto (5244 o 7014)"

if ([string]::IsNullOrWhiteSpace($port)) {
    $port = "5244"
}

Write-Host ""
Write-Host "🚀 Creando túnel público en http://localhost:$port ..." -ForegroundColor Green
Write-Host ""
Write-Host "⚠️  IMPORTANTE:" -ForegroundColor Yellow
Write-Host "   - Mantén esta ventana abierta mientras quieras compartir el link" -ForegroundColor White
Write-Host "   - El link se mostrará abajo (https://xxxx.ngrok-free.app)" -ForegroundColor White
Write-Host "   - Presiona Ctrl+C para cerrar el túnel" -ForegroundColor White
Write-Host ""

& $ngrokPath http $port

