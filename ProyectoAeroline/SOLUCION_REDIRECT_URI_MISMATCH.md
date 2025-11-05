# 🔴 Solución: Error 400 redirect_uri_mismatch

## 🐛 El Problema

Estás viendo este error en Google:
```
Error 400: redirect_uri_mismatch
Acceso bloqueado: La solicitud de ProyectoAeroline no es válida
```

**Esto significa que:** La URL que Google está intentando usar para redirigir después del login **NO está autorizada** en Google Cloud Console.

---

## ✅ Solución Paso a Paso

### **PASO 1: Obtener la URL Exacta de ngrok**

1. **Ejecuta ngrok:**
   ```cmd
   C:\ngrok\ngrok.exe http 5244
   ```

2. **Copia la URL EXACTA que muestra:**
   ```
   Forwarding    https://abc123.ngrok-free.app -> http://localhost:5244
   ```
   
   **Tu URL es:** `https://abc123.ngrok-free.app`
   
   *(Reemplaza `abc123` con tu URL real)*

---

### **PASO 2: Ir a Google Cloud Console**

1. **Abre:** https://console.cloud.google.com/
2. **Selecciona tu proyecto** (ProyectoAeroline o el que tengas)
3. **Ve a:** APIs & Services → **Credentials**
4. **Haz click en tu OAuth Client ID** (el que estás usando)

---

### **PASO 3: Agregar la URL de ngrok**

En la sección **"Authorized redirect URIs"**:

1. **Haz click en "+ ADD URI"**
2. **Pega esta URL EXACTA:**
   ```
   https://abc123.ngrok-free.app/signin-google
   ```
   
   ⚠️ **IMPORTANTE:**
   - Debe empezar con `https://` (no `http://`)
   - Debe terminar con `/signin-google` (exactamente así)
   - No debe tener espacios al inicio o final
   - Debe ser exactamente igual a la URL de ngrok + `/signin-google`

3. **Haz click en "SAVE"** (Guardar)

---

### **PASO 4: Verificar Todas las URLs Autorizadas**

Asegúrate de tener estas URLs autorizadas (depende de qué uses):

```
http://localhost:5244/signin-google
https://localhost:7014/signin-google
https://abc123.ngrok-free.app/signin-google  ← Esta es la que acabas de agregar
```

**Si tienes otras URLs de ngrok, agrégalas también.**

---

### **PASO 5: Esperar unos minutos**

Google puede tardar **1-2 minutos** en propagar los cambios. Espera un poco antes de probar de nuevo.

---

### **PASO 6: Reiniciar la aplicación**

1. **Detén tu aplicación** (Ctrl+C)
2. **Reinicia:**
   ```cmd
   dotnet run
   ```

---

### **PASO 7: Probar de nuevo**

1. **Abre ngrok** (si no está abierto):
   ```cmd
   C:\ngrok\ngrok.exe http 5244
   ```

2. **Desde el celular, abre:**
   ```
   https://abc123.ngrok-free.app
   ```

3. **Haz click en "Iniciar con Google"**

4. **Debería funcionar ahora** ✅

---

## 🔍 Verificar la URL que se Está Usando

Si el problema persiste, puedes ver qué URL está enviando tu aplicación:

### **Opción 1: Ver en los logs**

Cuando hagas click en "Iniciar con Google", mira la URL en la barra del navegador antes de redirigir a Google. Debería verse algo como:

```
https://accounts.google.com/o/oauth2/auth?redirect_uri=https%3A%2F%2Fabc123.ngrok-free.app%2Fsignin-google&...
```

La parte `redirect_uri=` muestra qué URL está usando. Debe coincidir EXACTAMENTE con la que agregaste en Google Console.

### **Opción 2: Verificar en Google Console**

1. Ve a Google Cloud Console
2. APIs & Services → Credentials
3. Click en tu OAuth Client ID
4. Revisa la lista de "Authorized redirect URIs"
5. Asegúrate de que la URL de ngrok esté ahí

---

## ⚠️ Problemas Comunes

### **Problema: "Sigue dando el mismo error"**

**Posibles causas:**
1. ❌ No esperaste suficiente tiempo (espera 2-3 minutos)
2. ❌ La URL no coincide exactamente (revisa mayúsculas, minúsculas, `/` final)
3. ❌ No guardaste los cambios en Google Console
4. ❌ Estás usando una URL de ngrok diferente (cada vez que reinicias ngrok, obtienes un link nuevo)

**Solución:**
- Verifica que la URL en Google Console sea **EXACTAMENTE** igual a la que muestra ngrok + `/signin-google`
- Si reiniciaste ngrok, agrega la **NUEVA** URL también

### **Problema: "Cada vez que reinicio ngrok, tengo que agregar una nueva URL"**

**Solución:**
- ✅ **Opción 1:** Usa una cuenta paga de ngrok para tener un dominio fijo
- ✅ **Opción 2:** Agrega todas las URLs de ngrok que uses (puedes tener múltiples)
- ✅ **Opción 3:** Despliega en Azure/Railway para tener una URL permanente

---

## 📋 Checklist de Verificación

Antes de probar de nuevo, verifica:

- [ ] ✅ ngrok está corriendo y muestra tu URL
- [ ] ✅ Copiaste la URL EXACTA de ngrok (sin espacios, completa)
- [ ] ✅ Agregaste `https://TU-URL-ngrok.app/signin-google` en Google Console
- [ ] ✅ Guardaste los cambios en Google Console
- [ ] ✅ Esperaste 1-2 minutos
- [ ] ✅ Reiniciaste tu aplicación (`dotnet run`)
- [ ] ✅ Estás accediendo desde el celular usando la URL de ngrok (no localhost)

---

## 🎯 Ejemplo Completo

**Si ngrok muestra:**
```
Forwarding    https://abc-123-xyz.ngrok-free.app -> http://localhost:5244
```

**Entonces en Google Console debes agregar:**
```
https://abc-123-xyz.ngrok-free.app/signin-google
```

**Y desde el celular debes abrir:**
```
https://abc-123-xyz.ngrok-free.app
```

---

**¿Sigue sin funcionar?** Comparte:
1. La URL exacta que muestra ngrok
2. La URL exacta que agregaste en Google Console
3. Cualquier mensaje de error adicional

