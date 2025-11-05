# Script de publicación para ProyectoAeroline
# Este script genera los archivos compilados listos para desplegar

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Publicando ProyectoAeroline" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Cambiar al directorio del proyecto
$proyectoPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $proyectoPath

# Verificar que existe el archivo .csproj
$csprojFile = Join-Path $proyectoPath "ProyectoAeroline.csproj"
if (-not (Test-Path $csprojFile)) {
    Write-Host "❌ Error: No se encontró ProyectoAeroline.csproj" -ForegroundColor Red
    exit 1
}

# Carpeta de publicación
$publishFolder = Join-Path $proyectoPath "publish"
Write-Host "📦 Carpeta de publicación: $publishFolder" -ForegroundColor Yellow

# Limpiar publicación anterior si existe
if (Test-Path $publishFolder) {
    Write-Host "🗑️  Eliminando publicación anterior..." -ForegroundColor Yellow
    Remove-Item -Path $publishFolder -Recurse -Force
}

# Publicar el proyecto
Write-Host ""
Write-Host "🚀 Iniciando publicación..." -ForegroundColor Green
Write-Host ""

try {
    dotnet publish "$csprojFile" `
        --configuration Release `
        --output "$publishFolder" `
        --self-contained false `
        --runtime win-x64

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Publicación completada exitosamente!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📁 Archivos publicados en: $publishFolder" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "📋 Próximos pasos:" -ForegroundColor Yellow
        Write-Host "   1. Copia la carpeta 'publish' al servidor destino" -ForegroundColor White
        Write-Host "   2. Asegúrate de tener .NET 8.0 Runtime instalado en el servidor" -ForegroundColor White
        Write-Host "   3. Configura la cadena de conexión en appsettings.json" -ForegroundColor White
        Write-Host "   4. Ejecuta: dotnet ProyectoAeroline.dll" -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host "❌ Error durante la publicación. Código de salida: $LASTEXITCODE" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ Error durante la publicación: $_" -ForegroundColor Red
    exit 1
}

