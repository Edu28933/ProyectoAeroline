@echo off
REM Script para compartir la aplicación usando ngrok
REM Asegúrate de tener ngrok instalado en C:\ngrok\ngrok.exe

echo ========================================
echo   Compartir Aplicacion con ngrok
echo ========================================
echo.

REM Verificar que ngrok existe
if not exist "C:\ngrok\ngrok.exe" (
    echo [ERROR] ngrok no encontrado en C:\ngrok\ngrok.exe
    echo.
    echo Por favor:
    echo 1. Descarga ngrok desde: https://ngrok.com/download
    echo 2. Extrae ngrok.exe a C:\ngrok\
    echo 3. Ejecuta este script nuevamente
    echo.
    pause
    exit /b 1
)

echo Tu aplicacion debe estar corriendo en uno de estos puertos:
echo   - http://localhost:5244
echo   - https://localhost:7014
echo.
set /p PORT="Ingresa el puerto (5244 o 7014): "

if "%PORT%"=="" (
    set PORT=5244
)

echo.
echo Creando tunel publico en http://localhost:%PORT% ...
echo.
echo IMPORTANTE:
echo - Manten esta ventana abierta mientras quieras compartir el link
echo - El link se mostrara abajo
echo - Presiona Ctrl+C para cerrar el tunel
echo.

C:\ngrok\ngrok.exe http %PORT%

pause

