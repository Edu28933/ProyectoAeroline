# Documentación: Lógica de Roles y Permisos

## Resumen de Roles

El sistema cuenta con **5 roles** diferentes, cada uno con permisos específicos según su función en la organización.

---

## 1. SuperAdmin (IdRol: 1)

### Descripción
Rol con **acceso completo** a todas las funcionalidades del sistema. Tiene control total sobre la configuración, gestión de usuarios, roles, permisos y todas las operaciones del negocio.

### Permisos
- **Todas las pantallas**: Ver, Crear, Editar, Eliminar
- **Pantallas incluidas**:
  - Empleados
  - Usuarios
  - Roles
  - Pantallas
  - RolPantallaOpcion (Permisos)
  - Aeropuertos
  - Aviones
  - Mantenimientos
  - Aerolineas
  - Vuelos
  - Horarios
  - Escalas
  - Pasajeros
  - Boletos
  - Equipaje
  - Servicios
  - Reservas
  - Facturacion
  - Historiales

### Uso típico
- Administrador principal del sistema
- Responsable de configurar roles y permisos
- Gestión completa de usuarios y seguridad

---

## 2. Admin (IdRol: 2)

### Descripción
Administrador operativo con acceso a la gestión diaria del negocio, **excepto** la configuración de roles y permisos del sistema.

### Permisos
- **Ver, Crear, Editar, Eliminar** en:
  - Empleados
  - Usuarios
  - Aeropuertos
  - Aviones
  - Mantenimientos
  - Aerolineas
  - Vuelos
  - Horarios
  - Escalas
  - Pasajeros
  - Boletos
  - Equipaje
  - Servicios
  - Reservas
  - Facturacion
  - Historiales

- **NO tiene acceso** a:
  - Roles (no puede gestionar roles)
  - Pantallas (no puede gestionar pantallas)
  - RolPantallaOpcion (no puede gestionar permisos)

### Uso típico
- Gerente operativo
- Supervisor de operaciones
- Responsable de la gestión diaria del negocio

---

## 3. Recepcion (IdRol: 3)

### Descripción
Personal de recepción con permisos para consultar información y gestionar reservas y boletos.

### Permisos

#### Solo Ver (consultar):
- Vuelos
- Horarios
- Usuarios (solo para ver)

#### Ver, Crear, Editar (gestión):
- Pasajeros
- Boletos
- Reservas

#### Sin Eliminar:
- No tiene permiso de eliminar en ninguna pantalla

### Uso típico
- Personal de mostrador
- Agentes de ventas
- Personal de atención al cliente
- Recepcionistas

---

## 4. Auditoria (IdRol: 4)

### Descripción
Rol de solo lectura para auditoría y revisión de información. **No puede realizar modificaciones** en el sistema.

### Permisos
- **Solo Ver** en **TODAS** las pantallas del sistema
- **NO tiene** permisos de Crear, Editar o Eliminar

### Pantallas accesibles (solo lectura):
- Empleados
- Usuarios
- Roles
- Pantallas
- RolPantallaOpcion
- Aeropuertos
- Aviones
- Mantenimientos
- Aerolineas
- Vuelos
- Horarios
- Escalas
- Pasajeros
- Boletos
- Equipaje
- Servicios
- Reservas
- Facturacion
- Historiales

### Uso típico
- Auditors internos
- Personal de control de calidad
- Supervisores que solo necesitan consultar
- Personal de cumplimiento

---

## 5. Usuario (IdRol: 5)

### Descripción
Rol para usuarios finales (clientes) que pueden comprar boletos y gestionar su propio perfil.

### Permisos

#### Boletos:
- **Ver**: Puede ver sus boletos
- **Crear**: Puede comprar boletos (crear reservas)
- **Editar**: Puede modificar sus reservas/boletos
- **NO Eliminar**: No puede eliminar boletos

#### Usuarios:
- **Ver**: Puede ver su propio perfil
- **Editar**: Puede modificar su propio perfil
- **NO Crear**: No puede crear otros usuarios
- **NO Eliminar**: No puede eliminar usuarios

### Pantallas NO accesibles:
- El usuario solo puede acceder a:
  - Boletos (comprar y gestionar)
  - Usuarios (solo su perfil)

### Uso típico
- Clientes finales
- Pasajeros que se registran con Google
- Usuarios que solo necesitan comprar boletos

---

## Asignación Automática de Roles

### Registro con Google
Cuando un usuario se registra usando **Google OAuth**, se le asigna **automáticamente** el **Rol 5 (Usuario)**.

Esto se configura en:
- **Stored Procedure**: `usp_GoogleLogin_Confirmar` (línea 135) → `@IdRol = 5`
- **Código C#**: `UsuariosData.ConfirmarGoogleLoginAsync` → devuelve `IdRol = 5` por defecto

### Registro Normal
Los usuarios que se registran normalmente a través del formulario de registro también reciben el **Rol 5 (Usuario)** por defecto.

---

## Notas Importantes

1. **SuperAdmin siempre tiene acceso total**: El sistema verifica si `IdRol = 1` y otorga automáticamente todos los permisos sin consultar la base de datos.

2. **Permisos verificados en dos niveles**:
   - **Servidor (Controller)**: El atributo `[RequirePermission]` bloquea el acceso si no hay permiso
   - **Cliente (Vista)**: Los botones y enlaces del sidebar se ocultan según los permisos

3. **Nombres de pantallas**: Los nombres deben coincidir exactamente con los nombres de los controladores (sin "Controller"):
   - `UsuariosController` → Pantalla: `"Usuarios"`
   - `BoletosController` → Pantalla: `"Boletos"`

4. **Eliminación lógica**: Los permisos se eliminan lógicamente (con `FechaEliminacion`) en lugar de eliminarse físicamente, manteniendo el historial.

---

## Tabla Resumen de Permisos

| Rol | Ver | Crear | Editar | Eliminar | Pantallas Especiales |
|-----|-----|-------|--------|----------|---------------------|
| **SuperAdmin (1)** | Todas | Todas | Todas | Todas | Acceso completo |
| **Admin (2)** | Todas excepto Roles/Pantallas/Permisos | Todas excepto Roles/Pantallas/Permisos | Todas excepto Roles/Pantallas/Permisos | Todas excepto Roles/Pantallas/Permisos | Sin acceso a configuración |
| **Recepcion (3)** | Vuelos, Horarios, Pasajeros, Boletos, Reservas, Usuarios | Pasajeros, Boletos, Reservas | Pasajeros, Boletos, Reservas | Ninguna | Solo gestión de reservas |
| **Auditoria (4)** | Todas | Ninguna | Ninguna | Ninguna | Solo lectura |
| **Usuario (5)** | Boletos, Usuarios (solo propio) | Boletos | Boletos, Usuarios (solo propio) | Ninguna | Solo comprar y perfil |

---

## Modificaciones Futuras

Si necesitas agregar nuevos permisos o modificar los existentes:

1. Ejecuta el script `Datos_Iniciales_Roles_Pantallas_Permisos.sql`
2. O modifica directamente los permisos en la tabla `RolPantallaOpcion` usando la interfaz web (si eres SuperAdmin)

---

**Última actualización**: 2025-01-11

