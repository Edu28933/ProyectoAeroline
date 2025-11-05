# 🔐 Guía: Login con Google y Confirmación de Boletos

Esta guía explica cómo configurar el login con Google y cómo funciona el sistema de confirmación de boletos por email.

---

## 📧 PARTE 1: LOGIN CON GOOGLE - Configuración

### **¿Cómo funciona el login con Google?**

1. El usuario hace clic en "Iniciar con Google"
2. Se redirige a Google para autenticarse
3. Google redirige de vuelta a tu aplicación en: `https://tu-dominio.com/signin-google`
4. Tu aplicación valida los datos y crea/inicia sesión del usuario

### **⚠️ PROBLEMA: URLs de Redirección**

Google **solo permite** redirecciones a URLs que hayas **previamente autorizado** en Google Cloud Console.

**Esto significa que:**
- Si compartes el link con ngrok: `https://abc123.ngrok-free.app`
- Debes agregar: `https://abc123.ngrok-free.app/signin-google` a Google Console
- Si despliegas en Azure: `https://tuapp.azurewebsites.net`
- Debes agregar: `https://tuapp.azurewebsites.net/signin-google` a Google Console

---

## 🔧 PASOS PARA CONFIGURAR GOOGLE OAUTH

### **Paso 1: Ir a Google Cloud Console**

1. Ve a: **https://console.cloud.google.com/**
2. Selecciona tu proyecto (o crea uno nuevo)

### **Paso 2: Habilitar Google+ API**

1. Ve a **"APIs & Services"** → **"Library"**
2. Busca **"Google+ API"** o **"Google Identity"**
3. Click en **"Enable"**

### **Paso 3: Crear Credenciales OAuth**

1. Ve a **"APIs & Services"** → **"Credentials"**
2. Click en **"+ CREATE CREDENTIALS"** → **"OAuth client ID"**
3. Si te pide configurar la pantalla de consentimiento:
   - **Tipo de usuario:** Internal (si tienes Workspace) o External
   - Completa los datos básicos
   - Agrega tu email como desarrollador

4. **Crear OAuth Client ID:**
   - **Application type:** Web application
   - **Name:** ProyectoAeroline (o el que quieras)

### **Paso 4: Configurar URLs de Redirección Autorizadas**

**Aquí está la parte IMPORTANTE:**

En el campo **"Authorized redirect URIs"**, debes agregar **TODAS** las URLs desde donde la gente accederá:

#### **Para Desarrollo Local:**
```
http://localhost:5244/signin-google
https://localhost:7014/signin-google
```

#### **Para ngrok (Temporal):**
Cada vez que ejecutes ngrok, obtendrás un link diferente:
```
https://abc123.ngrok-free.app/signin-google
https://xyz789.ngrok-free.app/signin-google
```

**⚠️ IMPORTANTE:** Debes agregar cada link de ngrok que uses. Si reinicias ngrok y obtienes un link diferente, agrega ese nuevo link también.

#### **Para Producción (Azure, Railway, etc.):**
```
https://tuapp.azurewebsites.net/signin-google
https://tuapp.up.railway.app/signin-google
```

### **Paso 5: Copiar Credenciales**

Después de crear el OAuth Client ID:
1. **Client ID:** Cópialo (ej: `245203969003-xxxxx.apps.googleusercontent.com`)
2. **Client Secret:** Cópialo (ej: `GOCSPX-xxxxx`)

### **Paso 6: Configurar en tu Aplicación**

**Para Desarrollo (appsettings.Development.json):**
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "TU_CLIENT_ID_AQUI",
      "ClientSecret": "TU_CLIENT_SECRET_AQUI",
      "CallbackPath": "/signin-google"
    }
  }
}
```

**Para Producción:**
- En Azure: Variables de entorno en App Service
- En Railway: Variables de entorno en el dashboard
- En ngrok: Usa `appsettings.Development.json` (mismo que desarrollo local)

---

## 🎯 FLUJO COMPLETO DE LOGIN CON GOOGLE

```
1. Usuario hace clic en "Iniciar con Google"
   ↓
2. Aplicación redirige a: https://accounts.google.com/o/oauth2/auth
   ↓
3. Usuario ingresa credenciales en Google
   ↓
4. Google redirige a: https://tu-dominio.com/signin-google
   (Esta URL DEBE estar autorizada en Google Console)
   ↓
5. Tu aplicación recibe el código de autorización
   ↓
6. Tu aplicación intercambia el código por un token
   ↓
7. Tu aplicación obtiene datos del usuario (email, nombre)
   ↓
8. Si el usuario es NUEVO:
   - Se envía email de confirmación
   - Usuario debe hacer clic en el link del email
   - Se crea la cuenta con Rol 5 (Usuario)
   ↓
9. Si el usuario YA EXISTE:
   - Se inicia sesión automáticamente
   ↓
10. Redirige a /Index (dashboard)
```

---

## ✈️ PARTE 2: CONFIRMACIÓN DE BOLETOS POR EMAIL

### **¿Cómo funciona la confirmación de boletos?**

1. Un usuario con permisos genera un boleto
2. Hace clic en "Enviar Boleto" en la vista de `VerPDF`
3. Se ingresa un email (o se usa automáticamente si es perfil de empleado)
4. Se genera un PDF del boleto
5. Se envía un email con:
   - El PDF adjunto
   - Un botón "Confirmar Boleto" con un link especial

6. El pasajero hace clic en "Confirmar Boleto"
7. Se valida el token y se cambia el estado del boleto de "Pendiente" a "Confirmado"

---

## 🔗 FLUJO DETALLADO DE CONFIRMACIÓN DE BOLETOS

### **Paso 1: Generar el Boleto**

**Ubicación:** `Controllers/BoletosController.cs` → Método `EnviarBoleto`

```csharp
// Se genera un token único
var confirmacionToken = Guid.NewGuid().ToString("N");

// Se guarda en sesión (temporalmente)
HttpContext.Session.SetString($"BoletoToken_{idBoleto}", confirmacionToken);

// Se crea la URL de confirmación
var baseUrl = $"{Request.Scheme}://{Request.Host}";
var confirmarUrl = $"{baseUrl}/Boletos/ConfirmarBoleto?idBoleto={idBoleto}&token={confirmacionToken}";
```

**URL generada será:**
```
https://tu-dominio.com/Boletos/ConfirmarBoleto?idBoleto=123&token=abc123def456...
```

### **Paso 2: Enviar el Email**

El email contiene:
- **PDF adjunto** con los datos del boleto
- **Botón "Confirmar Boleto"** que lleva a la URL de confirmación
- **Mensaje** explicando que debe confirmar el boleto

**Código del botón en el email:**
```html
<a href="https://tu-dominio.com/Boletos/ConfirmarBoleto?idBoleto=123&token=abc123...">
    Confirmar Boleto
</a>
```

### **Paso 3: Confirmar el Boleto**

**Ubicación:** `Controllers/BoletosController.cs` → Método `ConfirmarBoleto`

**Flujo de validación:**

1. **Validar token:**
   ```csharp
   if (string.IsNullOrWhiteSpace(token) || token.Length < 10)
   {
       // Token inválido
   }
   ```

2. **Validar que el boleto existe:**
   ```csharp
   var boleto = _BoletosData.MtdBuscarBoleto(idBoleto);
   if (boleto == null) { /* Error */ }
   ```

3. **Validar que está en estado "Pendiente":**
   ```csharp
   if (boleto.Estado != "Pendiente")
   {
       // Ya está confirmado/cancelado
   }
   ```

4. **Confirmar el boleto:**
   ```csharp
   var confirmado = _BoletosData.MtdConfirmarBoleto(idBoleto);
   // Cambia el estado a "Confirmado"
   ```

5. **Mostrar resultado:**
   - Vista: `Views/Boletos/ConfirmarBoletoResult.cshtml`
   - Esta vista es **pública** (no requiere login)
   - Solo muestra el resultado, NO da acceso al sistema

---

## ⚙️ CONFIGURACIÓN PARA COMPARTIR EL LINK

### **Escenario 1: Usando ngrok**

#### **1. Ejecutar tu aplicación:**
```cmd
cd ProyectoAeroline
dotnet run
```
Aplicación corriendo en: `http://localhost:5244`

#### **2. Ejecutar ngrok:**
```cmd
C:\ngrok\ngrok.exe http 5244
```
Obtienes: `https://abc123.ngrok-free.app`

#### **3. Configurar Google Console:**

Agregar a "Authorized redirect URIs":
```
https://abc123.ngrok-free.app/signin-google
```

#### **4. Actualizar appsettings.Development.json:**

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "TU_CLIENT_ID",
      "ClientSecret": "TU_CLIENT_SECRET",
      "CallbackPath": "/signin-google"
    }
  }
}
```

#### **5. Compartir el link:**

```
https://abc123.ngrok-free.app/Account/Login
```

O directamente:
```
https://abc123.ngrok-free.app
```

**⚠️ IMPORTANTE:** 
- Cada vez que reinicies ngrok, obtienes un link diferente
- Debes agregar el NUEVO link a Google Console
- O mejor aún: usa una cuenta de ngrok paga para tener un dominio fijo

---

### **Escenario 2: Desplegado en Azure**

#### **1. Crear App Service en Azure**
(Sigue la guía `INSTRUCCIONES_DESPLIEGUE.md`)

#### **2. Obtener el link:**
```
https://tuapp.azurewebsites.net
```

#### **3. Configurar Google Console:**

Agregar a "Authorized redirect URIs":
```
https://tuapp.azurewebsites.net/signin-google
```

#### **4. Configurar Variables de Entorno en Azure:**

En Azure Portal → Tu App Service → **Configuration** → **Application settings**:

```
Authentication__Google__ClientId = TU_CLIENT_ID
Authentication__Google__ClientSecret = TU_CLIENT_SECRET
```

#### **5. Compartir el link:**

```
https://tuapp.azurewebsites.net
```

---

## 📧 CONFIGURACIÓN DE EMAIL PARA BOLETOS

### **Requisitos:**

1. **Gmail con Contraseña de Aplicación:**
   - Ve a tu cuenta de Google
   - **Seguridad** → **Verificación en 2 pasos** (debe estar activada)
   - **Contraseñas de aplicaciones**
   - Genera una contraseña para "Mail"
   - Úsala en `Smtp__Pass`

2. **Configurar en appsettings.json:**

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "tu-email@gmail.com",
    "Pass": "tu-contraseña-de-app",
    "FromEmail": "tu-email@gmail.com",
    "FromName": "Proyecto Aerolínea"
  }
}
```

### **Flujo de Envío de Boleto:**

```
1. Usuario genera boleto → Vista VerPDF
   ↓
2. Hace clic en "Enviar Boleto"
   ↓
3. Se ingresa email (o se usa automático si es empleado)
   ↓
4. Se genera PDF usando QuestPDF
   ↓
5. Se genera token de confirmación (GUID)
   ↓
6. Se crea URL: https://tu-dominio.com/Boletos/ConfirmarBoleto?idBoleto=X&token=Y
   ↓
7. Se envía email con:
   - PDF adjunto
   - Botón con la URL de confirmación
   ↓
8. Pasajero recibe email
   ↓
9. Pasajero hace clic en "Confirmar Boleto"
   ↓
10. Se valida token y se cambia estado a "Confirmado"
```

---

## 🔒 SEGURIDAD IMPORTANTE

### **Para Google OAuth:**

1. ✅ **NUNCA compartas tu Client Secret**
2. ✅ **Usa variables de entorno** en producción (no en archivos)
3. ✅ **Agrega solo URLs que controlas** en Google Console
4. ✅ **Revisa regularmente** las URLs autorizadas en Google Console

### **Para Confirmación de Boletos:**

1. ✅ **El token está en sesión** (temporal, se pierde al reiniciar)
   - **Para producción:** Considera guardarlo en base de datos con expiración

2. ✅ **La página de confirmación es pública** (`[AllowAnonymous]`)
   - Esto está bien porque solo confirma el boleto
   - NO da acceso al sistema

3. ✅ **Validaciones de seguridad:**
   - Token no vacío y mínimo 10 caracteres
   - Boleto debe existir
   - Boleto debe estar en estado "Pendiente"

---

## 📝 RESUMEN RÁPIDO

### **Para Login con Google:**

1. **Configurar Google Console:**
   - Agregar URL: `https://tu-dominio.com/signin-google`
   - Copiar Client ID y Client Secret

2. **Configurar aplicación:**
   - Agregar credenciales en `appsettings.json` o variables de entorno
   - El callback path es: `/signin-google`

3. **Compartir link:**
   - `https://tu-dominio.com` o `https://tu-dominio.com/Account/Login`

### **Para Envío de Boletos:**

1. **Configurar SMTP:**
   - Gmail con contraseña de aplicación
   - Configurar en `appsettings.json`

2. **El flujo es automático:**
   - Click en "Enviar Boleto" → Ingresa email → Envía
   - Email contiene PDF y botón de confirmación
   - Botón lleva a: `/Boletos/ConfirmarBoleto?idBoleto=X&token=Y`

3. **Confirmación:**
   - Página pública que solo cambia el estado del boleto
   - No requiere login

---

## ⚠️ PROBLEMAS COMUNES

### **Error: "redirect_uri_mismatch"**

**Causa:** La URL de redirección no está autorizada en Google Console.

**Solución:**
1. Verifica la URL exacta que está usando (mira en la barra de direcciones cuando falla)
2. Agrega esa URL exacta a Google Console
3. Espera unos minutos para que se propague

### **Error: "Token inválido" al confirmar boleto**

**Causa:** 
- El token está en sesión y se perdió al reiniciar el servidor
- O el token fue usado ya

**Solución (para producción):**
- Guardar tokens en base de datos con expiración (1 hora, por ejemplo)
- Validar que no haya sido usado antes

### **Error: "No se puede conectar a SMTP"**

**Causa:**
- Contraseña incorrecta
- Verificación en 2 pasos no activada
- Puerto bloqueado por firewall

**Solución:**
- Usa "Contraseña de aplicación" de Google, no tu contraseña normal
- Verifica que el puerto 587 no esté bloqueado

---

**¿Necesitas ayuda con algún paso específico?** 🚀

