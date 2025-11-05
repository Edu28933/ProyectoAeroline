# 🚀 Guía Rápida: Login Google y Boletos

## 📋 RESUMEN

### **Login con Google:**
1. Configurar Google Console → Agregar URL de redirección
2. Copiar Client ID y Secret → Poner en `appsettings.json`
3. Compartir link → Personas pueden iniciar sesión

### **Envío de Boletos:**
1. Generar boleto → Click "Enviar Boleto"
2. Email se envía con PDF + botón "Confirmar"
3. Pasajero hace clic → Boleto cambia a "Confirmado"

---

## 🔐 PASO A PASO: LOGIN CON GOOGLE

### **PASO 1: Ir a Google Cloud Console**

👉 **https://console.cloud.google.com/**

1. Inicia sesión con tu cuenta de Google
2. Selecciona o crea un proyecto

### **PASO 2: Habilitar API**

1. **Menú** → **APIs & Services** → **Library**
2. Busca **"Google+ API"** → Click **Enable**

### **PASO 3: Crear OAuth Client ID**

1. **APIs & Services** → **Credentials**
2. **+ CREATE CREDENTIALS** → **OAuth client ID**

### **PASO 4: Configurar URLs de Redirección**

**⚠️ MUY IMPORTANTE: Agregar TODAS las URLs desde donde accederán:**

#### **Para ngrok (Pruebas rápidas):**
```
https://abc123.ngrok-free.app/signin-google
```
*(Reemplaza `abc123` con tu link de ngrok)*

#### **Para Azure (Producción):**
```
https://tuapp.azurewebsites.net/signin-google
```

#### **Para localhost (Desarrollo):**
```
http://localhost:5244/signin-google
https://localhost:7014/signin-google
```

**Cómo agregar:**
1. En "Authorized redirect URIs"
2. Click **"+ ADD URI"** por cada URL
3. Pega la URL completa
4. Click **Save**

### **PASO 5: Copiar Credenciales**

Después de crear, verás:
- **Client ID:** `245203969003-xxxxx.apps.googleusercontent.com`
- **Client Secret:** `GOCSPX-xxxxx`

### **PASO 6: Configurar en la App**

Edita `appsettings.Development.json`:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "245203969003-xxxxx.apps.googleusercontent.com",
      "ClientSecret": "GOCSPX-xxxxx",
      "CallbackPath": "/signin-google"
    }
  }
}
```

### **PASO 7: Reiniciar la aplicación**

```cmd
dotnet run
```

### **PASO 8: Probar**

1. Ve a: `https://tu-link.ngrok-free.app/Account/Login`
2. Click en **"Iniciar con Google"**
3. Debería funcionar ✅

---

## ✈️ PASO A PASO: ENVÍO Y CONFIRMACIÓN DE BOLETOS

### **PASO 1: Configurar Email (Gmail)**

1. **Activar verificación en 2 pasos:**
   - Ve a: https://myaccount.google.com/security
   - Activa "Verificación en 2 pasos"

2. **Generar contraseña de aplicación:**
   - https://myaccount.google.com/apppasswords
   - **Seleccionar app:** Mail
   - **Seleccionar dispositivo:** Otro (nombre personalizado: "ProyectoAeroline")
   - Click **Generar**
   - Copia la contraseña (ej: `abcd efgh ijkl mnop`)

3. **Configurar en appsettings.Development.json:**
```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "tu-email@gmail.com",
    "Pass": "abcd efgh ijkl mnop",
    "FromEmail": "tu-email@gmail.com",
    "FromName": "Proyecto Aerolínea"
  }
}
```

### **PASO 2: Generar y Enviar Boleto**

1. **Ir a Boletos** → **Listar**
2. **Click en "Generar Boleto"** de un boleto
3. En la vista `VerPDF`, **click en "Enviar Boleto"**
4. **Ingresa el email** del pasajero
5. **Click en "Enviar"**

**Lo que sucede:**
- ✅ Se genera PDF del boleto
- ✅ Se crea token de confirmación único
- ✅ Se envía email con:
  - PDF adjunto
  - Botón "Confirmar Boleto" con link especial

### **PASO 3: El Pasajero Recibe el Email**

El email contiene:
```
🎫 Boleto de Vuelo

Estimado/a [Nombre],

Le enviamos su boleto adjunto.

[Botón: ✅ Confirmar Boleto]

ID de Boleto: #123
Vuelo: VUELO-001
Origen: Ciudad A
Destino: Ciudad B
```

### **PASO 4: Confirmar el Boleto**

**El pasajero hace clic en "Confirmar Boleto"**

**URL generada:**
```
https://tu-dominio.com/Boletos/ConfirmarBoleto?idBoleto=123&token=abc123def456...
```

**Lo que sucede:**
1. ✅ Se valida el token
2. ✅ Se verifica que el boleto existe y está "Pendiente"
3. ✅ Se cambia el estado a "Confirmado"
4. ✅ Se muestra página de éxito (pública, sin acceso al sistema)

---

## 🔗 EJEMPLO COMPLETO CON NGROK

### **Escenario: Quieres compartir tu app con un cliente**

#### **1. Ejecutar aplicación:**
```cmd
cd ProyectoAeroline
dotnet run
```
*(Corre en `http://localhost:5244`)*

#### **2. Ejecutar ngrok:**
```cmd
C:\ngrok\ngrok.exe http 5244
```

**Obtienes:**
```
Forwarding    https://abc123.ngrok-free.app -> http://localhost:5244
```

#### **3. Configurar Google Console:**

Ve a Google Cloud Console → Credentials → Tu OAuth Client ID

**Agregar URI:**
```
https://abc123.ngrok-free.app/signin-google
```

**Click Save**

#### **4. Compartir el link:**

```
https://abc123.ngrok-free.app
```

O directamente a login:
```
https://abc123.ngrok-free.app/Account/Login
```

#### **5. El cliente puede:**

1. **Acceder al link**
2. **Click en "Iniciar con Google"**
3. **Ingresar sus credenciales de Google**
4. **Si es usuario nuevo:** Recibe email de confirmación
5. **Click en email:** Se crea cuenta y puede entrar
6. **Si ya existe:** Entra automáticamente

---

## 📧 EJEMPLO COMPLETO: ENVÍO DE BOLETO

### **Escenario: Enviar boleto a un pasajero**

#### **1. Preparar el boleto:**
1. Ir a **Boletos** → **Listar**
2. Buscar un boleto en estado "Pendiente"
3. Click en **"Generar Boleto"**

#### **2. Enviar por email:**
1. En la vista `VerPDF`, click en **"Enviar Boleto"**
2. Ingresar email: `pasajero@email.com`
3. Click en **"Enviar"**

#### **3. El pasajero recibe:**

**Email con:**
- 📎 **Adjunto:** `Boleto_123.pdf`
- 🔘 **Botón:** "✅ Confirmar Boleto"
- 📝 **Datos:** ID, Vuelo, Origen, Destino

#### **4. El pasajero hace clic:**

**El link es:**
```
https://abc123.ngrok-free.app/Boletos/ConfirmarBoleto?idBoleto=123&token=abc123def456...
```

**Página que ve:**
- ✅ "Boleto Confirmado"
- 📋 Datos del boleto
- ℹ️ Mensaje de confirmación
- 🔒 Nota de seguridad (página pública, sin acceso al sistema)

#### **5. Resultado:**

El boleto cambia de:
- **Estado:** "Pendiente" → **"Confirmado"**
- Se puede verificar en **Boletos** → **Listar**

---

## ⚠️ IMPORTANTE: URL BASE

**El sistema usa automáticamente la URL base de tu aplicación:**

```csharp
var baseUrl = $"{Request.Scheme}://{Request.Host}";
var confirmarUrl = $"{baseUrl}/Boletos/ConfirmarBoleto?idBoleto={idBoleto}&token={token}";
```

**Esto significa:**
- ✅ Si usas ngrok: `https://abc123.ngrok-free.app`
- ✅ Si usas Azure: `https://tuapp.azurewebsites.net`
- ✅ Si usas localhost: `http://localhost:5244`

**El link de confirmación se genera automáticamente con la URL correcta.**

---

## 🔒 SEGURIDAD

### **Para Google OAuth:**
- ✅ **NO compartas** tu Client Secret
- ✅ Solo agrega URLs que **controlas**
- ✅ Revisa periódicamente las URLs autorizadas

### **Para Boletos:**
- ✅ El token está en **sesión** (temporal)
- ✅ La página de confirmación es **pública** pero **solo confirma**, no da acceso
- ✅ Validaciones: Token válido, boleto existe, estado "Pendiente"

---

## 📞 RESUMEN DE LINKS IMPORTANTES

- **Google Cloud Console:** https://console.cloud.google.com/
- **Configurar OAuth:** APIs & Services → Credentials → OAuth Client ID
- **Contraseñas de App (Gmail):** https://myaccount.google.com/apppasswords
- **Verificación en 2 pasos:** https://myaccount.google.com/security

---

**¿Listo para compartir tu aplicación?** 🚀

