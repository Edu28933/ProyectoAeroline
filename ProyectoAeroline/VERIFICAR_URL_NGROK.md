# 🔍 Verificar URL de ngrok y Google Console

## 📋 Checklist de Diagnóstico

Sigue estos pasos para identificar el problema:

### **1. Verificar URL de ngrok**

Ejecuta ngrok y copia la URL EXACTA:
```cmd
C:\ngrok\ngrok.exe http 5244
```

**URL que debes copiar:**
```
https://abc123.ngrok-free.app
```
*(Sin el `/` al final, sin espacios)*

---

### **2. Verificar en Google Cloud Console**

1. Ve a: **https://console.cloud.google.com/**
2. Selecciona tu proyecto
3. **APIs & Services** → **Credentials**
4. Click en tu **OAuth Client ID**
5. Busca la sección **"Authorized redirect URIs"**

**Debe tener EXACTAMENTE:**
```
https://abc123.ngrok-free.app/signin-google
```

⚠️ **Verifica:**
- [ ] Empieza con `https://` (no `http://`)
- [ ] Termina con `/signin-google` (con la barra `/`)
- [ ] No tiene espacios
- [ ] Coincide EXACTAMENTE con tu URL de ngrok + `/signin-google`

---

### **3. Verificar Pantalla de Consentimiento OAuth**

A veces el problema no es la URL, sino la configuración de la pantalla de consentimiento:

1. En Google Cloud Console, ve a: **APIs & Services** → **OAuth consent screen**
2. Verifica:
   - [ ] **User Type:** External (o Internal si tienes Workspace)
   - [ ] **App name:** ProyectoAeroline
   - [ ] **User support email:** Tu email
   - [ ] **Developer contact information:** Tu email
3. **Guarda los cambios**

---

### **4. Verificar que la App está Corriendo con ngrok**

1. **En tu PC:**
   - ngrok debe estar corriendo
   - Tu aplicación debe estar corriendo (`dotnet run`)
   - Ambos deben estar en la misma ventana/pantalla

2. **Desde el celular:**
   - Abre el navegador
   - Ve a la URL de ngrok: `https://abc123.ngrok-free.app`
   - Deberías ver tu aplicación (no un error)

---

### **5. Verificar la URL que se Envía a Google**

Cuando hagas click en "Iniciar con Google", **ANTES de redirigir a Google**, mira la URL en la barra del navegador. Debería verse algo como:

```
https://abc123.ngrok-free.app/Account/ExternalLogin?provider=Google&returnUrl=/
```

Luego, cuando Google muestra el error, **mira la URL completa del error**. Google te dirá qué URL está esperando vs qué URL recibió.

---

### **6. Probar con una Nueva URL de ngrok**

Si reiniciaste ngrok y obtuviste una URL nueva:

1. **Copia la nueva URL:**
   ```
   https://xyz789.ngrok-free.app
   ```

2. **Agrega en Google Console:**
   ```
   https://xyz789.ngrok-free.app/signin-google
   ```

3. **Espera 2-3 minutos**

4. **Prueba de nuevo**

---

### **7. Verificar Logs de la Aplicación**

Los logs ahora mostrarán qué URL está usando. Mira la consola donde ejecutaste `dotnet run` cuando hagas click en "Iniciar con Google". Deberías ver:

```
Google OAuth - Scheme: https, Host: abc123.ngrok-free.app, RedirectUrl: https://abc123.ngrok-free.app/Account/ExternalLoginCallback
```

Esta URL debe coincidir EXACTAMENTE con la que agregaste en Google Console.

---

## 🎯 Pasos Inmediatos

1. **Ejecuta ngrok y copia la URL exacta**
2. **Ve a Google Console** → Credentials → Tu OAuth Client ID
3. **Verifica que la URL esté agregada EXACTAMENTE así:**
   ```
   https://TU-URL-ngrok.app/signin-google
   ```
4. **Si no está, agrégala y guarda**
5. **Espera 3 minutos**
6. **Reinicia tu aplicación** (`dotnet run`)
7. **Prueba de nuevo**

---

## ⚠️ Si Sigue Sin Funcionar

Comparte esta información:

1. **La URL exacta que muestra ngrok:**
   ```
   https://???.ngrok-free.app
   ```

2. **Las URLs exactas que tienes en Google Console** (en "Authorized redirect URIs")

3. **Si reiniciaste ngrok** después de agregar la URL en Google Console

4. **El mensaje de error completo** que muestra Google

5. **Los logs de tu aplicación** cuando haces click en "Iniciar con Google"

---

## 🔄 Alternativa Rápida

Si necesitas una solución inmediata:

1. **Usa localhost desde tu PC** (no desde el celular):
   ```
   http://localhost:5244
   ```

2. **O despliega en Azure/Railway** para tener una URL permanente que no cambie

