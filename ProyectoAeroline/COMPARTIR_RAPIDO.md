# 🚀 Cómo Compartir tu Aplicación - Guía Rápida

## ⚡ Método Rápido: ngrok (2 minutos)

### **Paso 1: Descargar ngrok**
1. Ve a: **https://ngrok.com/download**
2. Descarga **ngrok para Windows**
3. Extrae `ngrok.exe` a: `C:\ngrok\`

### **Paso 2: Ejecutar tu aplicación**
Abre una terminal y ejecuta:
```cmd
cd ProyectoAeroline
dotnet run
```
O si usas la carpeta publish:
```cmd
cd ProyectoAeroline\publish
dotnet ProyectoAeroline.dll
```

Tu aplicación estará en: **http://localhost:5244** o **https://localhost:7014**

### **Paso 3: Crear túnel público**

**Opción A: Usar el script automatizado**
```cmd
compartir-ngrok.bat
```

**Opción B: Manualmente**
```cmd
C:\ngrok\ngrok.exe http 5244
```
(Si tu app corre en 7014, usa ese puerto)

### **Paso 4: Obtener el link**

Ngrok mostrará algo como:
```
Forwarding    https://abc123.ngrok-free.app -> http://localhost:5244
```

**¡Comparte este link:** `https://abc123.ngrok-free.app`

---

## ⚠️ IMPORTANTE

1. **Mantén ambas ventanas abiertas:**
   - La terminal con `dotnet run`
   - La terminal con `ngrok`

2. **El link cambia cada vez** que reinicias ngrok (a menos que tengas cuenta paga)

3. **Solo funciona mientras tu PC esté encendido** y ambas aplicaciones corran

---

## 🔒 Para Producción Permanente

Si necesitas un link permanente, considera:
- **Azure App Service** (ver `GUIA_COMPARTIR_APLICACION.md`)
- **Railway.app** (fácil y gratis para empezar)
- **Heroku** (también tiene nivel gratis)

---

**¿Problemas?** 
- Verifica que el puerto coincida
- Asegúrate de que tu aplicación esté corriendo
- Revisa el firewall de Windows

