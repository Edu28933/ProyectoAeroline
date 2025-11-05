# 🌐 Guía: Cómo Compartir tu Aplicación con Otras Personas

## 📋 Opciones Disponibles

### **Opción 1: Túnel Temporal (ngrok) - ⚡ RÁPIDO para Pruebas**

**Ideal para:** Pruebas rápidas, demos, compartir con clientes temporalmente

#### **Paso 1: Descargar ngrok**

1. Ve a: https://ngrok.com/download
2. Descarga ngrok para Windows
3. Extrae el archivo `ngrok.exe` a una carpeta (ej: `C:\ngrok\`)

#### **Paso 2: Ejecutar tu aplicación**

```cmd
cd ruta\a\ProyectoAeroline
dotnet run
```

O si usas la carpeta `publish`:
```cmd
cd ruta\a\ProyectoAeroline\publish
dotnet ProyectoAeroline.dll
```

Tu aplicación estará corriendo en: `http://localhost:5000` (o el puerto que uses)

#### **Paso 3: Crear el túnel**

Abre **otra terminal** y ejecuta:

```cmd
cd C:\ngrok
ngrok http 5000
```

> **Nota:** Si tu aplicación corre en otro puerto (ej: 5001, 7014), cambia `5000` por ese puerto.

#### **Paso 4: Obtener el link público**

Ngrok mostrará algo como:

```
Forwarding    https://abc123.ngrok-free.app -> http://localhost:5000
```

**¡Ese link `https://abc123.ngrok-free.app` es el que puedes compartir!**

#### **Ventajas:**
- ✅ Muy rápido (2 minutos)
- ✅ HTTPS automático
- ✅ Gratis para uso básico
- ✅ No requiere configuración del servidor

#### **Desventajas:**
- ❌ El link cambia cada vez que reinicias ngrok (a menos que tengas cuenta paga)
- ❌ Limitado en tráfico (versión gratuita)
- ❌ Solo funciona mientras tu PC esté encendido y ngrok corriendo

---

### **Opción 2: Desplegar en Azure App Service - 🚀 PERMANENTE**

**Ideal para:** Despliegue permanente, producción

#### **Paso 1: Publicar la aplicación**

Ya lo hiciste anteriormente, pero si necesitas republicar:

```cmd
cd ProyectoAeroline
dotnet publish ProyectoAeroline.csproj --configuration Release --output ./publish
```

#### **Paso 2: Crear App Service en Azure**

1. **Inicia sesión en Azure Portal:** https://portal.azure.com

2. **Crear nuevo recurso:**
   - Busca "App Service" o "Web App"
   - Click en "Crear"

3. **Configurar:**
   - **Suscripción:** Tu suscripción
   - **Grupo de recursos:** Crear nuevo o usar existente
   - **Nombre:** `proyectoaeroline-[tunombre]` (debe ser único)
   - **Publicar:** Código
   - **Runtime stack:** .NET 8
   - **Sistema operativo:** Windows
   - **Plan:** Crear nuevo plan (ej: "Basic B1" - $13/mes aproximadamente)

4. **Click en "Revisar + crear"** y luego "Crear"

#### **Paso 3: Desplegar el código**

**Opción A: Desde Visual Studio**
1. Click derecho en el proyecto → "Publicar"
2. Selecciona "Azure" → "Azure App Service (Windows)"
3. Selecciona tu App Service
4. Click en "Publicar"

**Opción B: Desde la línea de comandos (Zip Deploy)**

1. **Instalar Azure CLI:**
   ```cmd
   winget install -e --id Microsoft.AzureCLI
   ```

2. **Login a Azure:**
   ```cmd
   az login
   ```

3. **Crear un archivo ZIP de la carpeta publish:**
   ```cmd
   cd ProyectoAeroline\publish
   tar -a -c -f ..\..\app.zip *
   ```

4. **Desplegar:**
   ```cmd
   az webapp deployment source config-zip --resource-group [TU_GRUPO_RECURSOS] --name [NOMBRE_APP_SERVICE] --src app.zip
   ```

#### **Paso 4: Configurar Variables de Entorno**

En Azure Portal:
1. Ve a tu App Service
2. **Configuración** → **Application settings**
3. Agrega estas variables:
   - `ConnectionStrings__CadenaSQL`: Tu cadena de conexión
   - `Authentication__Google__ClientId`: Tu Client ID de Google
   - `Authentication__Google__ClientSecret`: Tu Client Secret
   - `Smtp__Host`: smtp.gmail.com
   - `Smtp__User`: Tu email
   - `Smtp__Pass`: Tu contraseña de app
   - `Smtp__FromEmail`: Tu email
   - `ASPNETCORE_ENVIRONMENT`: Production

#### **Paso 5: Obtener el link**

Tu aplicación estará disponible en:
```
https://[nombre-app-service].azurewebsites.net
```

**Ejemplo:**
```
https://proyectoaeroline-eduardo.azurewebsites.net
```

#### **Ventajas:**
- ✅ Link permanente
- ✅ HTTPS incluido
- ✅ Escalable
- ✅ Integración con Azure SQL Database (ya lo tienes)

#### **Desventajas:**
- ❌ Requiere suscripción de Azure (hay nivel gratuito con límites)
- ❌ Configuración más compleja

---

### **Opción 3: Railway.app - 🎯 FÁCIL y Gratis (Temporal)**

**Ideal para:** Despliegue rápido sin configuración compleja

1. **Crear cuenta:** https://railway.app
2. **Nuevo proyecto** → **Deploy from GitHub** (si tienes el código en GitHub)
   O **Empty Project** → **Deploy from folder** → Sube la carpeta `publish`
3. **Configurar variables de entorno** en el dashboard
4. **Obtener link:** Railway te da un link tipo `https://tuproyecto.up.railway.app`

---

### **Opción 4: Configurar Servidor con IP Pública**

**Ideal para:** Si tienes un servidor dedicado o VPS

#### **Requisitos:**
- Servidor con IP pública
- Puerto 80/443 abierto en el firewall
- Dominio (opcional)

#### **Pasos:**
1. Copiar carpeta `publish` al servidor
2. Instalar .NET 8.0 Runtime
3. Configurar como servicio Windows (usando NSSM o Windows Service)
4. Configurar IIS o Nginx como reverse proxy
5. Configurar DNS apuntando a tu IP pública

---

## 🎯 Recomendación por Caso de Uso

| Caso | Recomendación |
|------|---------------|
| **Prueba rápida con cliente** | ngrok (Opción 1) |
| **Demo temporal** | Railway.app (Opción 3) |
| **Producción permanente** | Azure App Service (Opción 2) |
| **Servidor propio** | Configurar IP Pública (Opción 4) |

---

## ⚠️ IMPORTANTE: Seguridad

Antes de compartir tu aplicación, asegúrate de:

1. **✅ NO exponer datos sensibles:**
   - Mueve las credenciales de `appsettings.json` a variables de entorno
   - No subas `appsettings.Development.json` al servidor

2. **✅ Configurar HTTPS:**
   - Azure y Railway lo incluyen automáticamente
   - Con ngrok, usa el link HTTPS (no HTTP)

3. **✅ Configurar CORS (si aplica):**
   - Si tu frontend está en otro dominio

4. **✅ Revisar permisos:**
   - Asegúrate de que solo usuarios autorizados puedan acceder

---

## 📞 Soporte Rápido

**Para ngrok:**
- Documentación: https://ngrok.com/docs
- Problemas comunes: El puerto debe coincidir con el de tu app

**Para Azure:**
- Documentación: https://docs.microsoft.com/azure/app-service
- Soporte: Portal de Azure → "Help + support"

**Para Railway:**
- Documentación: https://docs.railway.app
- Soporte: Discord de Railway

---

**¿Necesitas ayuda con alguna opción específica?** 🚀

