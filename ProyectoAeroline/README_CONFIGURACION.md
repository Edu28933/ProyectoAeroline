# Configuración de Secretos para Desarrollo Local

Este proyecto requiere un archivo `appsettings.Development.json` que contiene secretos sensibles y **NO se sube a GitHub** por seguridad.

## Configuración Inicial

### 1. Copiar el archivo de ejemplo

Al clonar el repositorio en una nueva máquina, copia el archivo de ejemplo:

```bash
# En Windows PowerShell
Copy-Item appsettings.Development.json.example appsettings.Development.json

# En Linux/Mac
cp appsettings.Development.json.example appsettings.Development.json
```

### 2. Configurar los valores

Edita `appsettings.Development.json` y completa los siguientes valores:

#### Conexión a Base de Datos
```json
"ConnectionStrings": {
  "CadenaSQL": "Server=tcp:TU_SERVIDOR.database.windows.net,1433;Initial Catalog=TU_BASE_DE_DATOS;..."
}
```

#### Google OAuth
1. Ve a [Google Cloud Console](https://console.cloud.google.com/)
2. Crea un proyecto o selecciona uno existente
3. Habilita la API de Google+
4. Ve a "Credenciales" → "Crear credenciales" → "ID de cliente OAuth"
5. Tipo de aplicación: "Aplicación web"
6. **⚠️ IMPORTANTE**: Autoriza los siguientes URI de redirección (debes agregar TODOS los que vayas a usar):
   - `http://localhost:5244/signin-google` (HTTP - Puerto por defecto)
   - `https://localhost:7014/signin-google` (HTTPS - Puerto por defecto)
   - Si usas otro puerto en otra PC, agrega: `http://localhost:PUERTO/signin-google`
   - Si accedes desde otra máquina en la red: `http://IP_DE_LA_PC:PUERTO/signin-google`
   - Para producción: `https://tu-dominio.com/signin-google`
7. Copia el **Client ID** y **Client Secret** a `appsettings.Development.json`

**Nota**: Si el login con Google no funciona en una PC específica:
- Verifica qué puerto está usando la aplicación (mira en `launchSettings.json` o en la consola)
- Agrega ese URI completo a Google Cloud Console en "URI de redirección autorizados"

```json
"Authentication": {
  "Google": {
    "ClientId": "TU_CLIENT_ID.apps.googleusercontent.com",
    "ClientSecret": "TU_CLIENT_SECRET",
    "CallbackPath": "/signin-google"
  }
}
```

#### Configuración SMTP (Gmail)
1. Ve a tu cuenta de Google
2. Activa la verificación en dos pasos
3. Genera una "Contraseña de aplicación":
   - Ve a [Google Account](https://myaccount.google.com/)
   - Seguridad → Verificación en dos pasos
   - Contraseñas de aplicaciones → Generar nueva
4. Usa esa contraseña en `appsettings.Development.json`

```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "User": "TU_EMAIL@gmail.com",
  "Pass": "TU_CONTRASEÑA_DE_APLICACIÓN",
  "FromEmail": "TU_EMAIL@gmail.com",
  "FromName": "Proyecto Aerolínea"
}
```

## Verificación

Después de configurar `appsettings.Development.json`, ejecuta la aplicación:

```bash
dotnet run
```

La aplicación debería:
- ✅ Conectarse a la base de datos
- ✅ Permitir login con Google
- ✅ Enviar correos electrónicos

## Nota de Seguridad

⚠️ **NUNCA** subas `appsettings.Development.json` a GitHub. Este archivo está en `.gitignore` por seguridad.

Si accidentalmente subes secretos a GitHub:
1. Rotarlos inmediatamente (regenerar credenciales)
2. Eliminarlos del historial de Git usando `git filter-branch` o BFG Repo-Cleaner

