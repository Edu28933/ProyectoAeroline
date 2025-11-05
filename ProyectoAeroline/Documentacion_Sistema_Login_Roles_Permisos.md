# 📘 Documentación: Sistema de Login, Roles, Pantallas y Permisos

## 🎯 Resumen General

Este documento explica cómo funciona el sistema completo de autenticación, autorización y control de permisos en la aplicación. El sistema está diseñado para ser **robusto, seguro y escalable**.

---

## 🔐 1. SISTEMA DE LOGIN (AUTENTICACIÓN)

### 1.1. Flujo de Autenticación

El login se realiza a través del `AccountController` y puede ser de **dos formas**:

#### **A) Login Tradicional (Email/Contraseña)**

**Ubicación:** `Controllers/AccountController.cs` → Método `Login` (POST)

**Proceso:**

1. **Validación de credenciales:**
   - El usuario ingresa email/nombre y contraseña
   - Se llama a `LoginData.ValidarUsuarioAsync()` que:
     - Busca el usuario en la base de datos por `Correo` o `Nombre`
     - Verifica que el usuario esté `Activo`
     - Compara la contraseña (soporta texto plano para compatibilidad y hash BCrypt para seguridad)

2. **Creación de Claims (Identidad del usuario):**
   ```csharp
   var claims = new List<Claim>
   {
       new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
       new Claim(ClaimTypes.Name, nombre),
       new Claim("Nombre", nombre),
       new Claim(ClaimTypes.Email, correo),
       new Claim(ClaimTypes.Role, rolNombre),
       new Claim("IdRol", user.IdRol.ToString()) // ⭐ CLAVE para permisos
   };
   ```

3. **Creación de Cookie de Autenticación:**
   - Se crea una cookie de autenticación con duración de 8 horas
   - Se guarda información en la sesión:
     - `IdUsuario`
     - `Nombre`
     - `Correo`
     - `Rol`

4. **Redirección:**
   - Si hay un `returnUrl` válido, redirige ahí
   - Si no, redirige a `/Index` (dashboard)

#### **B) Login con Google OAuth**

**Ubicación:** `Controllers/AccountController.cs` → `ExternalLogin` y `ExternalLoginCallback`

**Proceso:**

1. **Inicio del flujo OAuth:**
   - El usuario hace clic en "Iniciar con Google"
   - Se redirige a Google para autenticación
   - Google devuelve información del usuario (email, nombre)

2. **Búsqueda/Creación de usuario:**
   - Si el usuario **ya existe** (buscado por email), se autentica automáticamente
   - Si el usuario **es nuevo**, se crea un registro con `IdRol = 5` (Usuario) por defecto

3. **Mismo proceso de Claims y Cookie:**
   - Se crean los mismos claims que en el login tradicional
   - Se guarda información en sesión

---

## 👥 2. SISTEMA DE ROLES

### 2.1. ¿Qué son los Roles?

Los **Roles** son grupos de usuarios que comparten el mismo nivel de acceso. Cada usuario tiene **un solo rol** asignado mediante el campo `IdRol` en la tabla `Usuarios`.

**Tabla en BD:** `Roles`
- `IdRol` (PK)
- `NombreRol` (ej: "SuperAdmin", "Admin", "Recepcion")
- Campos de auditoría: `FechaCreacion`, `UsuarioCreacion`, etc.

### 2.2. Roles Disponibles

Según la configuración actual del sistema:

| IdRol | NombreRol | Descripción |
|-------|-----------|-------------|
| 1 | **SuperAdmin** | Acceso completo a todo el sistema |
| 2 | **Admin** | Administrador operativo (sin acceso a configuración de roles) |
| 3 | **Recepcion** | Personal de recepción |
| 4 | **Auditoria** | Solo lectura de todas las pantallas |
| 5 | **Usuario** | Usuario básico (asignado automáticamente en registro con Google) |
| 6 | **Mantenimiento** | Personal de mantenimiento |
| 7 | **Consulta** | Solo consultas específicas |

### 2.3. Gestión de Roles

**Controlador:** `Controllers/RolesController.cs`

- **Listar:** Ver todos los roles
- **Guardar:** Crear un nuevo rol
- **Modificar:** Editar un rol existente
- **Eliminar:** Eliminación lógica (marca `FechaEliminacion`)

**Acceso:** Solo usuarios con rol **SuperAdmin** pueden gestionar roles.

---

## 🖥️ 3. SISTEMA DE PANTALLAS

### 3.1. ¿Qué son las Pantallas?

Las **Pantallas** representan las diferentes secciones/módulos de la aplicación. Cada pantalla corresponde a un controlador MVC.

**Tabla en BD:** `Pantallas`
- `IdPantalla` (PK)
- `NombrePantalla` (ej: "Usuarios", "Empleados", "Boletos")
- Campos de auditoría

### 3.2. Relación con Controladores

**IMPORTANTE:** El `NombrePantalla` debe coincidir **exactamente** con el nombre del controlador (sin "Controller"):

```
UsuariosController  → Pantalla: "Usuarios"
EmpleadosController → Pantalla: "Empleados"
BoletosController   → Pantalla: "Boletos"
```

### 3.3. Gestión de Pantallas

**Controlador:** `Controllers/PantallasController.cs`

- **Listar:** Ver todas las pantallas del sistema
- **Guardar:** Agregar una nueva pantalla
- **Modificar:** Editar una pantalla
- **Eliminar:** Eliminación lógica

**Acceso:** Solo usuarios con rol **SuperAdmin** pueden gestionar pantallas.

---

## 🔑 4. SISTEMA DE PERMISOS (RolPantallaOpcion)

### 4.1. ¿Qué son los Permisos?

Los **Permisos** definen qué operaciones puede realizar un **rol** específico en una **pantalla** específica.

**Tabla en BD:** `RolPantallaOpcion`
- `IdRolPantallaOpcion` (PK)
- `IdRol` (FK a Roles)
- `IdPantalla` (FK a Pantallas)
- `Ver` (bit) - Puede ver/consultar
- `Crear` (bit) - Puede crear registros
- `Editar` (bit) - Puede modificar registros
- `Eliminar` (bit) - Puede eliminar registros
- `Estado` ("Activo" o "Inactivo")

### 4.2. Operaciones Disponibles

Cada permiso tiene **4 operaciones** posibles:

| Operación | Descripción | Ejemplo |
|-----------|-------------|---------|
| **Ver** | Puede ver/consultar la pantalla | Ver lista de usuarios |
| **Crear** | Puede crear nuevos registros | Agregar un nuevo usuario |
| **Editar** | Puede modificar registros existentes | Modificar datos de un usuario |
| **Eliminar** | Puede eliminar registros | Eliminar un usuario |

### 4.3. Gestión de Permisos

**Controlador:** `Controllers/RolPantallaOpcionController.cs`

- **Listar:** Ver todos los permisos configurados
- **Guardar:** Asignar permisos a un rol para una pantalla
- **Modificar:** Actualizar permisos
- **Eliminar:** Eliminación lógica

**Acceso:** Solo usuarios con rol **SuperAdmin** pueden gestionar permisos.

---

## 🛡️ 5. VERIFICACIÓN DE PERMISOS

### 5.1. Servicio de Permisos (`PermisosService`)

**Ubicación:** `Services/PermisosService.cs`

Este servicio es el **corazón** del sistema de permisos. Se encarga de verificar si un usuario tiene un permiso específico.

#### **Método Principal: `TienePermiso`**

```csharp
bool TienePermiso(ClaimsPrincipal user, string nombrePantalla, string operacion)
```

**Flujo de verificación:**

1. **Verificación de autenticación:**
   - Si el usuario no está autenticado → `false`

2. **Extracción del IdRol:**
   - Obtiene `IdRol` del claim `"IdRol"` en el `ClaimsPrincipal`
   - Si no existe, intenta mapear desde `ClaimTypes.Role` (nombre del rol)

3. **Regla especial: SuperAdmin:**
   ```csharp
   if (idRol == 1) // SuperAdmin
   {
       return true; // ⭐ Acceso total sin verificar BD
   }
   ```

4. **Consulta a la base de datos:**
   - Obtiene todos los permisos activos del sistema
   - Busca el permiso específico que coincida con:
     - `IdRol` del usuario
     - `NombrePantalla` (ej: "Usuarios")
     - `Estado = "Activo"`

5. **Verificación de la operación:**
   - Compara la operación solicitada ("Ver", "Crear", "Editar", "Eliminar")
   - Devuelve el valor del bit correspondiente (`Ver`, `Crear`, `Editar`, o `Eliminar`)

### 5.2. Atributo de Autorización (`RequirePermissionAttribute`)

**Ubicación:** `Attributes/RequirePermissionAttribute.cs`

Este atributo se coloca en las acciones de los controladores para **bloquear el acceso** si el usuario no tiene el permiso requerido.

#### **Ejemplo de uso:**

```csharp
[Authorize] // Primero verifica que esté autenticado
[RequirePermission("Usuarios", "Ver")] // Luego verifica el permiso
public IActionResult Listar()
{
    // Solo usuarios con permiso "Ver" en "Usuarios" pueden acceder
    return View();
}
```

#### **Flujo del atributo:**

1. **Intercepta la petición** antes de ejecutar la acción
2. **Obtiene el servicio de permisos** desde el contenedor de dependencias
3. **Llama a `PermisosService.TienePermiso()`**
4. **Si NO tiene permiso:**
   - Redirige a `Home/AccesoDenegado`
   - Muestra mensaje de error en `TempData`
5. **Si tiene permiso:**
   - Permite que la acción se ejecute normalmente

### 5.3. Helper de Vistas (`PermisosHelper`)

**Ubicación:** `Helpers/PermisosHelper.cs`

Este helper permite verificar permisos **en las vistas Razor** para mostrar u ocultar botones/enlaces.

#### **Ejemplo de uso en vista:**

```razor
@using ProyectoAeroline.Helpers

@if (PermisosHelper.PuedeCrear(ViewContext, "Usuarios"))
{
    <a href="/Usuarios/Guardar" class="btn btn-primary">Agregar Usuario</a>
}

@if (PermisosHelper.PuedeEliminar(ViewContext, "Usuarios"))
{
    <button onclick="eliminar(@item.IdUsuario)">Eliminar</button>
}
```

#### **Métodos disponibles:**

- `TienePermiso(viewContext, nombrePantalla, operacion)` - Verificación genérica
- `PuedeVer(viewContext, nombrePantalla)` - Verificación de "Ver"
- `PuedeCrear(viewContext, nombrePantalla)` - Verificación de "Crear"
- `PuedeEditar(viewContext, nombrePantalla)` - Verificación de "Editar"
- `PuedeEliminar(viewContext, nombrePantalla)` - Verificación de "Eliminar"

---

## 🔄 6. FLUJO COMPLETO DE UNA PETICIÓN

### Ejemplo: Usuario intenta acceder a `/Usuarios/Listar`

1. **Middleware de Autenticación (`UseAuthentication`):**
   - Lee la cookie de autenticación
   - Crea el `ClaimsPrincipal` con los claims del usuario

2. **Middleware de Autorización (`UseAuthorization`):**
   - Verifica que el usuario esté autenticado (`[Authorize]`)

3. **Filtro de Autorización (`RequirePermissionAttribute`):**
   - Intercepta la petición antes de ejecutar `Listar()`
   - Llama a `PermisosService.TienePermiso(user, "Usuarios", "Ver")`

4. **Verificación en PermisosService:**
   - Si `IdRol == 1` → `return true` (SuperAdmin tiene acceso total)
   - Si no, consulta la BD:
     ```sql
     SELECT Ver FROM RolPantallaOpcion
     WHERE IdRol = @IdRol
       AND NombrePantalla = 'Usuarios'
       AND Estado = 'Activo'
     ```
   - Devuelve `true` si `Ver = 1`, `false` si no

5. **Resultado:**
   - **Si tiene permiso:** Se ejecuta `Listar()` y muestra la vista
   - **Si NO tiene permiso:** Redirige a `Home/AccesoDenegado` con mensaje de error

---

## 📊 7. ESTRUCTURA DE BASE DE DATOS

### 7.1. Tablas Involucradas

```
┌─────────────┐
│   Usuarios  │
│─────────────│
│ IdUsuario   │──┐
│ IdRol       │  │ FK
│ Nombre      │  │
│ Correo      │  │
│ Contraseña  │  │
│ Estado      │  │
└─────────────┘  │
                 │
┌─────────────┐  │
│    Roles    │◄─┘
│─────────────│
│ IdRol       │──┐
│ NombreRol   │  │
└─────────────┘  │
                 │
┌─────────────┐  │
│  Pantallas  │  │
│─────────────│  │
│ IdPantalla  │  │
│ NombrePantalla│ │
└─────────────┘  │
                 │
┌─────────────────────┐
│ RolPantallaOpcion  │◄──┘
│────────────────────│
│ IdRolPantallaOpcion│
│ IdRol              │──┐ FK a Roles
│ IdPantalla         │──┘ FK a Pantallas
│ Ver                │
│ Crear              │
│ Editar             │
│ Eliminar           │
│ Estado             │
└────────────────────┘
```

### 7.2. Stored Procedures Principales

- `sp_RolPantallaOpcionesSeleccionar` - Obtiene todos los permisos activos
- `sp_RolPantallaOpcionAgregar` - Agrega un nuevo permiso
- `sp_RolPantallaOpcionEditar` - Actualiza un permiso
- `sp_RolPantallaOpcionEliminar` - Elimina lógicamente un permiso

---

## ⚙️ 8. CONFIGURACIÓN EN `Program.cs`

### 8.1. Registro de Servicios

```csharp
// Servicio de permisos
builder.Services.AddScoped<IPermisosService, PermisosService>();
builder.Services.AddScoped<RolPantallaOpcionData>();
```

### 8.2. Configuración de Autenticación

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    })
    .AddGoogle(options => {
        // Configuración de Google OAuth
    });
```

### 8.3. Política de Autorización Global

```csharp
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
```

Esto significa que **por defecto**, todas las rutas requieren autenticación (a menos que se marquen con `[AllowAnonymous]`).

---

## 🔒 9. SEGURIDAD: DOBLE CAPA DE VERIFICACIÓN

El sistema implementa **dos capas** de verificación de permisos:

### **Capa 1: Servidor (Backend) - OBLIGATORIA**

- **Atributo `[RequirePermission]`** en controladores
- **Bloquea el acceso** si no hay permiso
- **No se puede evitar** modificando el código del cliente

**Ejemplo:**
```csharp
[RequirePermission("Usuarios", "Eliminar")]
public IActionResult Eliminar(int id)
{
    // Si el usuario NO tiene permiso, nunca llegará aquí
    // Será redirigido a AccesoDenegado
}
```

### **Capa 2: Cliente (Frontend) - UX**

- **Helper `PermisosHelper`** en vistas
- **Oculta botones/enlaces** si no hay permiso
- **Mejora la experiencia de usuario**
- **NO es obligatoria** (la seguridad real está en la capa 1)

**Ejemplo:**
```razor
@if (PermisosHelper.PuedeEliminar(ViewContext, "Usuarios"))
{
    <button>Eliminar</button>
}
```

---

## 📝 10. REGLAS ESPECIALES

### 10.1. SuperAdmin (IdRol = 1)

- **Tiene acceso total** sin verificar la base de datos
- **Bypass automático** en `PermisosService.TienePermiso()`
- **No se guardan permisos específicos** para SuperAdmin en `RolPantallaOpcion`
- **Siempre retorna `true`** independientemente de la pantalla u operación

### 10.2. Eliminación Lógica

- Todos los registros (Roles, Pantallas, Permisos) se eliminan **lógicamente**
- Se marca `FechaEliminacion` en lugar de hacer `DELETE`
- **Ventaja:** Mantiene historial y permite recuperación

### 10.3. Nombres de Pantallas

- **DEBEN coincidir exactamente** con el nombre del controlador (sin "Controller")
- **Case-insensitive** en las comparaciones (se usa `StringComparison.OrdinalIgnoreCase`)
- **Ejemplo:**
  - Controlador: `UsuariosController`
  - Pantalla en BD: `"Usuarios"` ✅ Correcto
  - Pantalla en BD: `"usuarios"` ✅ También correcto (case-insensitive)
  - Pantalla en BD: `"Usuario"` ❌ Incorrecto (falta la 's')

---

## 🎯 11. EJEMPLOS PRÁCTICOS

### Ejemplo 1: Usuario con rol "Recepcion" intenta ver usuarios

1. Usuario hace login → `IdRol = 3` guardado en claims
2. Intenta acceder a `/Usuarios/Listar`
3. `RequirePermissionAttribute` verifica: `TienePermiso(user, "Usuarios", "Ver")`
4. `PermisosService` consulta BD:
   ```sql
   SELECT Ver FROM RolPantallaOpcion
   WHERE IdRol = 3 AND NombrePantalla = 'Usuarios' AND Estado = 'Activo'
   ```
5. Si `Ver = 1` → Permite acceso
6. Si `Ver = 0` → Redirige a `AccesoDenegado`

### Ejemplo 2: SuperAdmin intenta eliminar un usuario

1. Usuario hace login → `IdRol = 1` (SuperAdmin)
2. Intenta acceder a `/Usuarios/Eliminar/5`
3. `RequirePermissionAttribute` verifica: `TienePermiso(user, "Usuarios", "Eliminar")`
4. `PermisosService` detecta `IdRol == 1`
5. **Retorna `true` inmediatamente** sin consultar BD
6. Permite acceso

### Ejemplo 3: Vista muestra/oculta botones según permisos

```razor
<!-- Botón "Agregar" solo visible si tiene permiso "Crear" -->
@if (PermisosHelper.PuedeCrear(ViewContext, "Usuarios"))
{
    <a href="/Usuarios/Guardar" class="btn btn-primary">➕ Agregar</a>
}

<!-- Botón "Eliminar" solo visible si tiene permiso "Eliminar" -->
@if (PermisosHelper.PuedeEliminar(ViewContext, "Usuarios"))
{
    <button onclick="eliminar(@item.IdUsuario)">🗑️ Eliminar</button>
}
```

---

## ✅ 12. CHECKLIST PARA AGREGAR UNA NUEVA PANTALLA

Si agregas un nuevo controlador (ej: `ReservasController`), debes:

1. ✅ Crear el controlador con acciones (`Listar`, `Guardar`, `Modificar`, `Eliminar`)
2. ✅ Agregar `[Authorize]` a nivel de clase
3. ✅ Agregar `[RequirePermission("Reservas", "Ver")]` a `Listar`
4. ✅ Agregar `[RequirePermission("Reservas", "Crear")]` a `Guardar` (GET y POST)
5. ✅ Agregar `[RequirePermission("Reservas", "Editar")]` a `Modificar` (GET y POST)
6. ✅ Agregar `[RequirePermission("Reservas", "Eliminar")]` a `Eliminar` (GET y POST)
7. ✅ Crear la pantalla en BD con `NombrePantalla = "Reservas"`
8. ✅ Configurar permisos en `RolPantallaOpcion` para cada rol
9. ✅ Usar `PermisosHelper` en las vistas para mostrar/ocultar botones
10. ✅ Agregar el enlace en el sidebar con verificación de permisos

---

## 📚 13. ARCHIVOS CLAVE DEL SISTEMA

| Archivo | Descripción |
|--------|-------------|
| `Controllers/AccountController.cs` | Maneja login/logout y OAuth de Google |
| `Data/LoginData.cs` | Valida credenciales de usuario |
| `Services/PermisosService.cs` | Verifica permisos de usuarios |
| `Attributes/RequirePermissionAttribute.cs` | Filtro de autorización para acciones |
| `Helpers/PermisosHelper.cs` | Helper para verificar permisos en vistas |
| `Controllers/RolesController.cs` | Gestión de roles |
| `Controllers/PantallasController.cs` | Gestión de pantallas |
| `Controllers/RolPantallaOpcionController.cs` | Gestión de permisos |
| `Data/RolPantallaOpcionData.cs` | Acceso a datos de permisos |
| `Program.cs` | Configuración de autenticación y servicios |

---

## 🔍 14. DEBUGGING Y TROUBLESHOOTING

### Problema: Usuario puede acceder sin permiso

**Solución:**
1. Verificar que el controlador tenga `[Authorize]`
2. Verificar que la acción tenga `[RequirePermission]`
3. Verificar que el `NombrePantalla` coincida exactamente
4. Verificar que el permiso esté `Activo` en la BD
5. Verificar que el `IdRol` esté correcto en los claims

### Problema: Botones aparecen pero no funcionan

**Solución:**
- La capa de seguridad del servidor (`[RequirePermission]`) está funcionando correctamente
- El problema está solo en la vista (capa de UX)
- Agregar `PermisosHelper` en la vista para ocultar botones

### Problema: SuperAdmin no tiene acceso

**Solución:**
1. Verificar que `IdRol == 1` en los claims
2. Verificar que el claim `"IdRol"` esté presente después del login
3. Verificar que `PermisosService.TienePermiso()` esté detectando `IdRol == 1`

---

## 🎓 15. CONCLUSIÓN

El sistema de login, roles, pantallas y permisos está diseñado con:

- ✅ **Seguridad robusta** (doble capa de verificación)
- ✅ **Escalabilidad** (fácil agregar nuevos roles, pantallas y permisos)
- ✅ **Mantenibilidad** (código organizado y documentado)
- ✅ **UX mejorada** (oculta elementos según permisos)
- ✅ **Flexibilidad** (permisos granulares por operación)

Este sistema permite un control total sobre quién puede hacer qué en la aplicación, protegiendo tanto el backend como mejorando la experiencia del usuario en el frontend.

---

**Fecha de creación:** 2025-11-02  
**Última actualización:** 2025-11-02  
**Versión del sistema:** 1.0

