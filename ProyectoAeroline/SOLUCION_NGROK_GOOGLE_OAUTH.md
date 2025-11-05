# 🔧 Solución: Error localhost con ngrok y Google OAuth

## 🐛 Problema

Cuando accedes desde un celular usando ngrok y intentas iniciar sesión con Google, obtienes el error:
```
ERR_CONNECTION_REFUSED
localhost rechazó la conexión.
```

**Causa:** ASP.NET Core no detecta automáticamente que está detrás de un proxy (ngrok) y sigue usando `localhost` en lugar de la URL pública de ngrok.

---

## ✅ Solución Implementada

Se agregó configuración en `Program.cs` para:
1. **Detectar headers de proxy** que ngrok envía (`X-Forwarded-Proto`, `X-Forwarded-Host`)
2. **Actualizar automáticamente** el `Scheme` y `Host` de las peticiones
3. **Usar la URL pública** de ngrok para todas las redirecciones

---

## 📋 Pasos para Configurar Correctamente

### **1. Obtener tu URL de ngrok**

Ejecuta ngrok:
```cmd
C:\ngrok\ngrok.exe http 5244
```

Obtendrás algo como:
```
Forwarding    https://abc123.ngrok-free.app -> http://localhost:5244
```

**Copia esta URL:** `https://abc123.ngrok-free.app`

---

### **2. Agregar URL en Google Cloud Console**

1. Ve a: **https://console.cloud.google.com/**
2. Selecciona tu proyecto
3. **APIs & Services** → **Credentials**
4. Haz click en tu **OAuth Client ID**
5. En **"Authorized redirect URIs"**, agrega:
   ```
   https://abc123.ngrok-free.app/signin-google
   ```
   *(Reemplaza `abc123` con tu URL real de ngrok)*
6. Click **Save**

⚠️ **IMPORTANTE:** Agrega la URL exacta de ngrok. Cada vez que reinicies ngrok y obtengas un link diferente, debes agregar ese nuevo link también.

---

### **3. Verificar Configuración de la App**

Asegúrate de que `appsettings.Development.json` tenga:
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

---

### **4. Reiniciar la Aplicación**

Después de los cambios en `Program.cs`, reinicia tu aplicación:
```cmd
dotnet run
```

---

## 🔍 Verificar que Funciona

1. **Abre ngrok:**
   ```cmd
   C:\ngrok\ngrok.exe http 5244
   ```

2. **Abre tu aplicación desde el celular:**
   ```
   https://abc123.ngrok-free.app
   ```

3. **Haz click en "Iniciar con Google"**

4. **Debería redirigir a:**
   ```
   https://accounts.google.com/o/oauth2/auth?...
   ```

5. **Después de autenticarte en Google, debería redirigir a:**
   ```
   https://abc123.ngrok-free.app/signin-google?code=...
   ```
   *(No `localhost`)*

---

## 🎯 ¿Qué Hace el Código?

El código agregado en `Program.cs`:

```csharp
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                      ForwardedHeaders.XForwardedProto |
                      ForwardedHeaders.XForwardedHost,
    RequireHeaderSymmetry = false
});

app.Use(async (context, next) =>
{
    // Si viene de ngrok, usar HTTPS y el host correcto
    if (context.Request.Headers.ContainsKey("X-Forwarded-Proto"))
    {
        var forwardedProto = context.Request.Headers["X-Forwarded-Proto"].ToString();
        if (forwardedProto == "https")
        {
            context.Request.Scheme = "https";
        }
    }
    
    if (context.Request.Headers.ContainsKey("X-Forwarded-Host"))
    {
        var forwardedHost = context.Request.Headers["X-Forwarded-Host"].ToString();
        if (!string.IsNullOrWhiteSpace(forwardedHost))
        {
            context.Request.Host = new HostString(forwardedHost);
        }
    }
    
    await next();
});
```

**Esto garantiza que:**
- ✅ La aplicación detecte que viene de ngrok
- ✅ Use `https://` en lugar de `http://`
- ✅ Use `abc123.ngrok-free.app` en lugar de `localhost:5244`
- ✅ Google OAuth redirija correctamente a la URL pública

---

## ⚠️ Problemas Comunes

### **Error: "redirect_uri_mismatch"**

**Causa:** La URL de ngrok no está autorizada en Google Console.

**Solución:**
1. Verifica la URL exacta en ngrok
2. Agrega `https://TU-URL-ngrok.app/signin-google` a Google Console
3. Espera 1-2 minutos para que se propague

### **Sigue redirigiendo a localhost**

**Causa:** La aplicación no se reinició después de los cambios.

**Solución:**
1. Detén la aplicación (Ctrl+C)
2. Reinicia: `dotnet run`
3. Prueba de nuevo desde el celular

### **ngrok muestra "ERR_NGROK_302"**

**Causa:** Google está redirigiendo, pero la URL no está autorizada.

**Solución:**
1. Verifica en Google Console que la URL esté exactamente igual
2. Asegúrate de incluir `https://` y `/signin-google` al final
3. No incluyas parámetros adicionales

---

## 📝 Resumen Rápido

1. ✅ **Ejecuta ngrok:** `C:\ngrok\ngrok.exe http 5244`
2. ✅ **Copia la URL:** `https://abc123.ngrok-free.app`
3. ✅ **Agrega en Google Console:** `https://abc123.ngrok-free.app/signin-google`
4. ✅ **Reinicia tu app:** `dotnet run`
5. ✅ **Prueba desde el celular:** `https://abc123.ngrok-free.app`

---

**¿Necesitas ayuda?** Revisa los logs de tu aplicación para ver qué URL está usando.

