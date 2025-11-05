@echo off
REM Script de publicación para ProyectoAeroline (Windows Batch)

echo ========================================
echo   Publicando ProyectoAeroline
echo ========================================
echo.

REM Cambiar al directorio del script
cd /d "%~dp0"

REM Verificar que existe el archivo .csproj
if not exist "ProyectoAeroline.csproj" (
    echo [ERROR] No se encontró ProyectoAeroline.csproj
    pause
    exit /b 1
)

REM Carpeta de publicación
set PUBLISH_FOLDER=publish

REM Limpiar publicación anterior si existe
if exist "%PUBLISH_FOLDER%" (
    echo Eliminando publicación anterior...
    rmdir /s /q "%PUBLISH_FOLDER%"
)

echo.
echo Iniciando publicación...
echo.

REM Publicar el proyecto
dotnet publish ProyectoAeroline.csproj ^
    --configuration Release ^
    --output "%PUBLISH_FOLDER%" ^
    --self-contained false ^
    --runtime win-x64

if %ERRORLEVEL% EQU 0 (
    echo.
    echo [OK] Publicación completada exitosamente!
    echo.
    echo Archivos publicados en: %PUBLISH_FOLDER%
    echo.
    echo Próximos pasos:
    echo   1. Copia la carpeta 'publish' al servidor destino
    echo   2. Asegúrate de tener .NET 8.0 Runtime instalado en el servidor
    echo   3. Configura la cadena de conexión en appsettings.json
    echo   4. Ejecuta: dotnet ProyectoAeroline.dll
    echo.
) else (
    echo.
    echo [ERROR] Error durante la publicación. Código: %ERRORLEVEL%
    echo.
    pause
    exit /b 1
)

pause

