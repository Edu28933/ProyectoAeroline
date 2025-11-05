# 📦 Guía de Despliegue - ProyectoAeroline

Esta guía te ayudará a publicar y desplegar el proyecto en cualquier servidor sin necesidad de compilar.

---

## 🚀 Opción 1: Publicación Automática (Recomendado)

### En Windows:

**Usando PowerShell:**
```powershell
.\publicar.ps1
```

**Usando Batch:**
```cmd
publicar.bat
```

### En Linux/Mac:

```bash
dotnet publish ProyectoAeroline.csproj --configuration Release --output ./publish --self-contained false --runtime linux-x64
```

---

## 📋 Opción 2: Publicación Manual

### Paso 1: Abrir Terminal/CMD

Navega hasta la carpeta del proyecto:
```cmd
cd ruta\a\ProyectoAeroline
```

### Paso 2: Ejecutar Comando de Publicación

**Para Windows:**
```cmd
dotnet publish ProyectoAeroline.csproj --configuration Release --output ./publish --self-contained false --runtime win-x64
```

**Para Linux:**
```bash
dotnet publish ProyectoAeroline.csproj --configuration Release --output ./publish --self-contained false --runtime linux-x64
```

**Para macOS:**
```bash
dotnet publish ProyectoAeroline.csproj --configuration Release --output ./publish --self-contained false --runtime osx-x64
```

**Para múltiples plataformas (portable):**
```cmd
dotnet publish ProyectoAeroline.csproj --configuration Release --output ./publish
```

---

## 📁 Estructura de la Carpeta `publish`

Después de la publicación, encontrarás:

```
publish/
├── ProyectoAeroline.dll          (Aplicación compilada)
├── ProyectoAeroline.exe          (Ejecutable en Windows)
├── appsettings.json              (Configuración)
├── appsettings.Development.json  (Configuración de desarrollo)
├── wwwroot/                      (Archivos estáticos: CSS, JS, imágenes)
├── Views/                        (Vistas Razor)
├── Pages/                        (Páginas Razor)
└── [dependencias .dll]           (Librerías necesarias)
```

---

## 🖥️ Desplegar en el Servidor

### Requisitos del Servidor

1. **.NET 8.0 Runtime** instalado
   - Descargar desde: https://dotnet.microsoft.com/download/dotnet/8.0
   - Seleccionar "Runtime" (no SDK)

2. **Base de datos SQL Server** accesible
   - Azure SQL Database
   - SQL Server Express/LocalDB
   - SQL Server Standard/Enterprise

### Pasos de Despliegue

#### 1. Copiar Archivos

Copia toda la carpeta `publish` al servidor destino. Puedes usar:
- **FTP/SFTP**
- **Red compartida**
- **USB/Disco externo**
- **Azure Blob Storage / AWS S3**

#### 2. Configurar `appsettings.json`

Edita el archivo `appsettings.json` en el servidor con tus datos reales:

```json
{
  "ConnectionStrings": {
    "CadenaSQL": "Server=TU_SERVIDOR;Database=AerolineaPruebaDB;User Id=TU_USUARIO;Password=TU_CONTRASEÑA;Encrypt=True;"
  },
  "Authentication": {
    "Google": {
      "ClientId": "TU_CLIENT_ID",
      "ClientSecret": "TU_CLIENT_SECRET",
      "CallbackPath": "/signin-google"
    }
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "TU_EMAIL",
    "Pass": "TU_CONTRASEÑA_APP",
    "FromEmail": "TU_EMAIL",
    "FromName": "Proyecto Aerolínea"
  }
}
```

#### 3. Ejecutar la Aplicación

**En Windows:**
```cmd
cd ruta\a\publish
dotnet ProyectoAeroline.dll
```

O directamente:
```cmd
ProyectoAeroline.exe
```

**En Linux:**
```bash
cd /ruta/a/publish
dotnet ProyectoAeroline.dll
```

#### 4. Configurar como Servicio (Opcional - Producción)

**Windows (como Servicio):**

Usar NSSM (Non-Sucking Service Manager) o crear un servicio Windows:

```cmd
# Con NSSM
nssm install ProyectoAeroline "C:\Program Files\dotnet\dotnet.exe" "C:\ruta\a\publish\ProyectoAeroline.dll"
nssm start ProyectoAeroline
```

**Linux (con systemd):**

Crear archivo `/etc/systemd/system/proyectoaeroline.service`:

```ini
[Unit]
Description=ProyectoAeroline Web Application
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /ruta/a/publish/ProyectoAeroline.dll
Restart=always
RestartSec=10
SyslogIdentifier=proyectoaeroline
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

Luego:
```bash
sudo systemctl enable proyectoaeroline
sudo systemctl start proyectoaeroline
sudo systemctl status proyectoaeroline
```

---

## 🌐 Configurar como Aplicación Web (IIS / Apache / Nginx)

### IIS (Windows)

1. Instalar **ASP.NET Core Hosting Bundle**: https://dotnet.microsoft.com/download/dotnet/8.0
2. Crear un nuevo **Application Pool** (modo "No Managed Code")
3. Crear un **Site** en IIS apuntando a la carpeta `publish`
4. Configurar el puerto (ej: 80, 443, 5000)

### Apache (Linux)

1. Instalar `mod_proxy` y `mod_proxy_http`
2. Configurar virtual host en `/etc/apache2/sites-available/`:

```apache
<VirtualHost *:80>
    ServerName tudominio.com
    
    ProxyPreserveHost On
    ProxyPass / http://localhost:5000/
    ProxyPassReverse / http://localhost:5000/
    
    ErrorLog ${APACHE_LOG_DIR}/proyectoaeroline_error.log
    CustomLog ${APACHE_LOG_DIR}/proyectoaeroline_access.log combined
</VirtualHost>
```

### Nginx (Linux)

1. Instalar Nginx
2. Configurar en `/etc/nginx/sites-available/proyectoaeroline`:

```nginx
server {
    listen 80;
    server_name tudominio.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Luego:
```bash
sudo ln -s /etc/nginx/sites-available/proyectoaeroline /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

---

## 🔒 Variables de Entorno (Recomendado para Producción)

En lugar de poner datos sensibles en `appsettings.json`, usa variables de entorno:

### Windows:
```cmd
set ConnectionStrings__CadenaSQL="TU_CADENA_AQUI"
set Authentication__Google__ClientId="TU_CLIENT_ID"
set Authentication__Google__ClientSecret="TU_CLIENT_SECRET"
```

### Linux:
```bash
export ConnectionStrings__CadenaSQL="TU_CADENA_AQUI"
export Authentication__Google__ClientId="TU_CLIENT_ID"
export Authentication__Google__ClientSecret="TU_CLIENT_SECRET"
```

O crea un archivo `.env` y cárgalo antes de ejecutar.

---

## 🐳 Despliegue con Docker (Opcional)

### Crear Dockerfile

Crea un archivo `Dockerfile` en la raíz del proyecto:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProyectoAeroline.csproj", "./"]
RUN dotnet restore "ProyectoAeroline.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "ProyectoAeroline.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProyectoAeroline.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProyectoAeroline.dll"]
```

### Construir y Ejecutar

```bash
docker build -t proyectoaeroline .
docker run -d -p 8080:80 --name aerolinea proyectoaeroline
```

---

## ✅ Verificación Post-Despliegue

1. **Verificar que la aplicación está corriendo:**
   - Abre el navegador en: `http://localhost:5000` (o el puerto configurado)
   - Deberías ver la página de login

2. **Verificar logs:**
   - Windows: Revisa la consola donde ejecutaste `dotnet ProyectoAeroline.dll`
   - Linux: `journalctl -u proyectoaeroline -f` (si usas systemd)

3. **Probar login:**
   - Intentar iniciar sesión con un usuario válido
   - Verificar que se conecta correctamente a la base de datos

---

## 🔧 Solución de Problemas

### Error: "No se puede encontrar el runtime de .NET"

**Solución:** Instala .NET 8.0 Runtime en el servidor.

### Error: "No se puede conectar a la base de datos"

**Solución:**
- Verifica la cadena de conexión en `appsettings.json`
- Asegúrate de que el servidor SQL esté accesible desde el servidor de la aplicación
- Verifica firewall y reglas de red

### Error: "Puerto en uso"

**Solución:**
- Cambia el puerto en `launchSettings.json` o usa variables de entorno
- Ejecuta: `export ASPNETCORE_URLS="http://localhost:5001"` (Linux) o `set ASPNETCORE_URLS=http://localhost:5001` (Windows)

### Error: "No se pueden cargar las vistas"

**Solución:**
- Asegúrate de copiar TODA la carpeta `publish` incluyendo `Views` y `Pages`
- Verifica permisos de archivos en el servidor

---

## 📞 Soporte

Si tienes problemas durante el despliegue:
1. Revisa los logs de la aplicación
2. Verifica la configuración de `appsettings.json`
3. Asegúrate de que todas las dependencias estén instaladas
4. Consulta la documentación oficial de .NET: https://docs.microsoft.com/dotnet/core/deployment/

---

**Última actualización:** 2025-11-02

