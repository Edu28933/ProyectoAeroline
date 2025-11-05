-- =============================================
-- Script: Datos Iniciales - Roles, Pantallas y Permisos
-- Descripción: 
--   - Borra datos existentes de Roles, Pantallas y RolPantallaOpcion
--   - Crea 5 roles (SuperAdmin, Admin, Recepcion, Auditoria, Usuario)
--   - Crea todas las pantallas del sistema
--   - Configura permisos para cada rol
-- Fecha: 2025-01-11
-- =============================================

USE [AerolineaPruebaDB]
GO

SET NOCOUNT ON;

BEGIN TRANSACTION;

-- =============================================
-- 1. ELIMINAR DATOS EXISTENTES (lógica)
-- =============================================

-- Eliminar permisos (RolPantallaOpcion) primero debido a foreign keys
UPDATE [dbo].[RolPantallaOpcion]
SET [FechaEliminacion] = GETDATE()
WHERE [FechaEliminacion] IS NULL;

-- Eliminar pantallas (lógica)
UPDATE [dbo].[Pantallas]
SET [FechaEliminacion] = GETDATE()
WHERE [FechaEliminacion] IS NULL;

-- Eliminar roles (lógica) - EXCEPTO si quieres mantener datos históricos
-- Si quieres hacer DELETE físico, descomenta las siguientes líneas:
-- DELETE FROM [dbo].[RolPantallaOpcion];
-- DELETE FROM [dbo].[Pantallas];
-- DELETE FROM [dbo].[Roles];

-- Para este script, solo eliminaremos lógicamente y luego insertaremos nuevos

-- =============================================
-- 2. INSERTAR ROLES
-- =============================================

-- Limpiar roles existentes (físico) - descomenta si quieres borrar físicamente
-- DELETE FROM [dbo].[Roles] WHERE [IdRol] IN (1, 2, 3, 4, 5);

-- Insertar o actualizar roles
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [IdRol] = 1)
BEGIN
    SET IDENTITY_INSERT [dbo].[Roles] ON;
    INSERT INTO [dbo].[Roles] ([IdRol], [NombreRol], [FechaCreacion], [UsuarioCreacion])
    VALUES (1, 'SuperAdmin', GETDATE(), 'Sistema');
    SET IDENTITY_INSERT [dbo].[Roles] OFF;
END
ELSE
BEGIN
    UPDATE [dbo].[Roles] 
    SET [NombreRol] = 'SuperAdmin', 
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema',
        [FechaEliminacion] = NULL
    WHERE [IdRol] = 1;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [IdRol] = 2)
BEGIN
    SET IDENTITY_INSERT [dbo].[Roles] ON;
    INSERT INTO [dbo].[Roles] ([IdRol], [NombreRol], [FechaCreacion], [UsuarioCreacion])
    VALUES (2, 'Admin', GETDATE(), 'Sistema');
    SET IDENTITY_INSERT [dbo].[Roles] OFF;
END
ELSE
BEGIN
    UPDATE [dbo].[Roles] 
    SET [NombreRol] = 'Admin', 
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema',
        [FechaEliminacion] = NULL
    WHERE [IdRol] = 2;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [IdRol] = 3)
BEGIN
    SET IDENTITY_INSERT [dbo].[Roles] ON;
    INSERT INTO [dbo].[Roles] ([IdRol], [NombreRol], [FechaCreacion], [UsuarioCreacion])
    VALUES (3, 'Recepcion', GETDATE(), 'Sistema');
    SET IDENTITY_INSERT [dbo].[Roles] OFF;
END
ELSE
BEGIN
    UPDATE [dbo].[Roles] 
    SET [NombreRol] = 'Recepcion', 
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema',
        [FechaEliminacion] = NULL
    WHERE [IdRol] = 3;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [IdRol] = 4)
BEGIN
    SET IDENTITY_INSERT [dbo].[Roles] ON;
    INSERT INTO [dbo].[Roles] ([IdRol], [NombreRol], [FechaCreacion], [UsuarioCreacion])
    VALUES (4, 'Auditoria', GETDATE(), 'Sistema');
    SET IDENTITY_INSERT [dbo].[Roles] OFF;
END
ELSE
BEGIN
    UPDATE [dbo].[Roles] 
    SET [NombreRol] = 'Auditoria', 
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema',
        [FechaEliminacion] = NULL
    WHERE [IdRol] = 4;
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [IdRol] = 5)
BEGIN
    SET IDENTITY_INSERT [dbo].[Roles] ON;
    INSERT INTO [dbo].[Roles] ([IdRol], [NombreRol], [FechaCreacion], [UsuarioCreacion])
    VALUES (5, 'Usuario', GETDATE(), 'Sistema');
    SET IDENTITY_INSERT [dbo].[Roles] OFF;
END
ELSE
BEGIN
    UPDATE [dbo].[Roles] 
    SET [NombreRol] = 'Usuario', 
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema',
        [FechaEliminacion] = NULL
    WHERE [IdRol] = 5;
END

PRINT 'Roles insertados/actualizados correctamente';

-- =============================================
-- 3. VERIFICAR QUE LA TABLA Opciones TENGA LOS VALORES NECESARIOS
-- =============================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Ver')
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [FechaCreacion])
    VALUES ('Ver', GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Crear')
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [FechaCreacion])
    VALUES ('Crear', GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Editar')
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [FechaCreacion])
    VALUES ('Editar', GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Eliminar')
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [FechaCreacion])
    VALUES ('Eliminar', GETDATE());
END

-- Obtener IDs de opciones
DECLARE @IdOpcionVer INT = (SELECT [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Ver');
DECLARE @IdOpcionCrear INT = (SELECT [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Crear');
DECLARE @IdOpcionEditar INT = (SELECT [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Editar');
DECLARE @IdOpcionEliminar INT = (SELECT [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Eliminar');

PRINT 'Opciones verificadas correctamente';

-- =============================================
-- 4. INSERTAR PANTALLAS
-- =============================================

-- Función helper para insertar/actualizar pantallas
DECLARE @PantallaId INT;

-- Empleados
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Empleados';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Empleados', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Usuarios
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Usuarios';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Usuarios', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Roles
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Roles';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Roles', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Pantallas
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Pantallas';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Pantallas', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- RolPantallaOpcion (Permisos)
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'RolPantallaOpcion';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('RolPantallaOpcion', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Aeropuertos
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Aeropuertos';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Aeropuertos', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Aviones
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Aviones';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Aviones', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Mantenimientos
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Mantenimientos';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Mantenimientos', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Aerolineas
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Aerolineas';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Aerolineas', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Vuelos
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Vuelos';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Vuelos', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Horarios
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Horarios';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Horarios', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Escalas
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Escalas';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Escalas', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Pasajeros
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Pasajeros';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Pasajeros', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Boletos
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Boletos';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Boletos', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Equipaje
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Equipaje';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Equipaje', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Servicios
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Servicios';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Servicios', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Reservas
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Reservas';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Reservas', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Facturacion
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Facturacion';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Facturacion', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

-- Historiales
SELECT @PantallaId = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Historiales';
IF @PantallaId IS NULL
BEGIN
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [FechaCreacion], [UsuarioCreacion])
    VALUES ('Historiales', GETDATE(), 'Sistema');
    SET @PantallaId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [dbo].[Pantallas] 
    SET [FechaEliminacion] = NULL,
        [FechaActualizacion] = GETDATE(),
        [UsuarioActualizacion] = 'Sistema'
    WHERE [IdPantalla] = @PantallaId;
END

PRINT 'Pantallas insertadas/actualizadas correctamente';

-- =============================================
-- 5. FUNCIÓN HELPER PARA INSERTAR PERMISOS
-- =============================================

-- Esta función ayudará a insertar permisos de forma más fácil
-- Función para insertar/eliminar permisos de una pantalla para un rol
DECLARE @HelperSQL NVARCHAR(MAX) = '
CREATE OR ALTER FUNCTION dbo.fn_InsertarPermiso(
    @IdRol INT,
    @NombrePantalla NVARCHAR(100),
    @IdOpcion INT,
    @UsuarioCreacion NVARCHAR(100) = ''Sistema''
)
RETURNS INT
AS
BEGIN
    DECLARE @IdPantalla INT = (SELECT [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = @NombrePantalla AND [FechaEliminacion] IS NULL);
    IF @IdPantalla IS NULL
        RETURN -1;
    
    DECLARE @IdRolPantallaOpcion INT;
    
    -- Verificar si ya existe (activo)
    SELECT @IdRolPantallaOpcion = [IdRolPantallaOpcion]
    FROM [dbo].[RolPantallaOpcion]
    WHERE [IdRol] = @IdRol 
        AND [IdPantalla] = @IdPantalla 
        AND [IdOpcion] = @IdOpcion
        AND [FechaEliminacion] IS NULL;
    
    IF @IdRolPantallaOpcion IS NULL
    BEGIN
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (@IdRol, @IdPantalla, @IdOpcion, GETDATE(), @UsuarioCreacion);
        SET @IdRolPantallaOpcion = SCOPE_IDENTITY();
    END
    
    RETURN @IdRolPantallaOpcion;
END
';

-- Intentar crear la función (puede fallar si ya existe, pero no importa)
BEGIN TRY
    EXEC sp_executesql @HelperSQL;
END TRY
BEGIN CATCH
    -- Función ya existe o error, continuar
END CATCH

-- Función alternativa inline para insertar permisos
DECLARE @InsertarPermiso NVARCHAR(MAX) = '
DECLARE @IdPantallaLocal INT;
SELECT @IdPantallaLocal = [IdPantalla] 
FROM [dbo].[Pantallas] 
WHERE [NombrePantalla] = @NombrePantalla 
  AND [FechaEliminacion] IS NULL;

IF @IdPantallaLocal IS NOT NULL
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM [dbo].[RolPantallaOpcion]
        WHERE [IdRol] = @IdRol
          AND [IdPantalla] = @IdPantallaLocal
          AND [IdOpcion] = @IdOpcion
          AND [FechaEliminacion] IS NULL
    )
    BEGIN
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (@IdRol, @IdPantallaLocal, @IdOpcion, GETDATE(), @UsuarioCreacion);
    END
    ELSE
    BEGIN
        -- Reactivar si estaba eliminado lógicamente
        UPDATE [dbo].[RolPantallaOpcion]
        SET [FechaEliminacion] = NULL,
            [FechaActualizacion] = GETDATE(),
            [UsuarioActualizacion] = @UsuarioCreacion
        WHERE [IdRol] = @IdRol
          AND [IdPantalla] = @IdPantallaLocal
          AND [IdOpcion] = @IdOpcion;
    END
END
';

-- =============================================
-- 6. CONFIGURAR PERMISOS PARA CADA ROL
-- =============================================

-- Declarar variables para IDs de pantallas
DECLARE @IdPantallaEmpleados INT;
DECLARE @IdPantallaUsuarios INT;
DECLARE @IdPantallaRoles INT;
DECLARE @IdPantallaPantallas INT;
DECLARE @IdPantallaRolPantallaOpcion INT;
DECLARE @IdPantallaAeropuertos INT;
DECLARE @IdPantallaAviones INT;
DECLARE @IdPantallaMantenimientos INT;
DECLARE @IdPantallaAerolineas INT;
DECLARE @IdPantallaVuelos INT;
DECLARE @IdPantallaHorarios INT;
DECLARE @IdPantallaEscalas INT;
DECLARE @IdPantallaPasajeros INT;
DECLARE @IdPantallaBoletos INT;
DECLARE @IdPantallaEquipaje INT;
DECLARE @IdPantallaServicios INT;
DECLARE @IdPantallaReservas INT;
DECLARE @IdPantallaFacturacion INT;
DECLARE @IdPantallaHistoriales INT;

-- Obtener IDs de pantallas
SELECT @IdPantallaEmpleados = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Empleados' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaUsuarios = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Usuarios' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaRoles = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Roles' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaPantallas = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Pantallas' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaRolPantallaOpcion = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'RolPantallaOpcion' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaAeropuertos = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Aeropuertos' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaAviones = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Aviones' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaMantenimientos = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Mantenimientos' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaAerolineas = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Aerolineas' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaVuelos = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Vuelos' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaHorarios = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Horarios' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaEscalas = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Escalas' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaPasajeros = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Pasajeros' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaBoletos = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Boletos' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaEquipaje = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Equipaje' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaServicios = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Servicios' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaReservas = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Reservas' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaFacturacion = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Facturacion' AND [FechaEliminacion] IS NULL;
SELECT @IdPantallaHistoriales = [IdPantalla] FROM [dbo].[Pantallas] WHERE [NombrePantalla] = 'Historiales' AND [FechaEliminacion] IS NULL;

-- Función helper para insertar permiso
DECLARE @PermisoSQL NVARCHAR(MAX);
DECLARE @NombrePantallaParam NVARCHAR(100);

-- Helper para insertar un permiso
DECLARE @InsertPermiso NVARCHAR(MAX);

-- =============================================
-- 6.1. ROL 1: SuperAdmin - ACCESO TOTAL A TODO
-- =============================================

PRINT 'Configurando permisos para SuperAdmin (Rol 1)...';

-- SuperAdmin tiene Ver, Crear, Editar, Eliminar en TODAS las pantallas
DECLARE pantallas_cursor CURSOR FOR
SELECT [IdPantalla], [NombrePantalla]
FROM [dbo].[Pantallas]
WHERE [FechaEliminacion] IS NULL;

DECLARE @CurrentPantallaId INT;
DECLARE @CurrentPantallaNombre NVARCHAR(100);

OPEN pantallas_cursor;
FETCH NEXT FROM pantallas_cursor INTO @CurrentPantallaId, @CurrentPantallaNombre;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Ver
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 1 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionVer AND [FechaEliminacion] IS NULL)
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (1, @CurrentPantallaId, @IdOpcionVer, GETDATE(), 'Sistema');
    ELSE
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 1 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionVer;
    
    -- Crear
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 1 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionCrear AND [FechaEliminacion] IS NULL)
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (1, @CurrentPantallaId, @IdOpcionCrear, GETDATE(), 'Sistema');
    ELSE
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 1 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionCrear;
    
    -- Editar
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 1 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEditar AND [FechaEliminacion] IS NULL)
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (1, @CurrentPantallaId, @IdOpcionEditar, GETDATE(), 'Sistema');
    ELSE
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 1 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEditar;
    
    -- Eliminar
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 1 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEliminar AND [FechaEliminacion] IS NULL)
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (1, @CurrentPantallaId, @IdOpcionEliminar, GETDATE(), 'Sistema');
    ELSE
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 1 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEliminar;
    
    FETCH NEXT FROM pantallas_cursor INTO @CurrentPantallaId, @CurrentPantallaNombre;
END

CLOSE pantallas_cursor;
DEALLOCATE pantallas_cursor;

PRINT 'Permisos SuperAdmin configurados';

-- =============================================
-- 6.2. ROL 2: Admin - Acceso a gestión (sin configuración de permisos)
-- =============================================

PRINT 'Configurando permisos para Admin (Rol 2)...';

-- Admin tiene Ver, Crear, Editar, Eliminar en:
-- Empleados, Usuarios, Aeropuertos, Aviones, Mantenimientos, Aerolineas, Vuelos, Horarios, Escalas, Pasajeros, Boletos, Equipaje, Servicios, Reservas, Facturacion, Historiales
-- Admin NO tiene acceso a: Roles, Pantallas, RolPantallaOpcion

DECLARE @AdminPantallas TABLE ([IdPantalla] INT, [NombrePantalla] NVARCHAR(100));
INSERT INTO @AdminPantallas VALUES
(@IdPantallaEmpleados, 'Empleados'),
(@IdPantallaUsuarios, 'Usuarios'),
(@IdPantallaAeropuertos, 'Aeropuertos'),
(@IdPantallaAviones, 'Aviones'),
(@IdPantallaMantenimientos, 'Mantenimientos'),
(@IdPantallaAerolineas, 'Aerolineas'),
(@IdPantallaVuelos, 'Vuelos'),
(@IdPantallaHorarios, 'Horarios'),
(@IdPantallaEscalas, 'Escalas'),
(@IdPantallaPasajeros, 'Pasajeros'),
(@IdPantallaBoletos, 'Boletos'),
(@IdPantallaEquipaje, 'Equipaje'),
(@IdPantallaServicios, 'Servicios'),
(@IdPantallaReservas, 'Reservas'),
(@IdPantallaFacturacion, 'Facturacion'),
(@IdPantallaHistoriales, 'Historiales');

DECLARE admin_cursor CURSOR FOR
SELECT [IdPantalla] FROM @AdminPantallas;

OPEN admin_cursor;
FETCH NEXT FROM admin_cursor INTO @CurrentPantallaId;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @CurrentPantallaId IS NOT NULL
    BEGIN
        -- Ver
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 2 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionVer AND [FechaEliminacion] IS NULL)
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
            VALUES (2, @CurrentPantallaId, @IdOpcionVer, GETDATE(), 'Sistema');
        ELSE
            UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
            WHERE [IdRol] = 2 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionVer;
        
        -- Crear
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 2 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionCrear AND [FechaEliminacion] IS NULL)
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
            VALUES (2, @CurrentPantallaId, @IdOpcionCrear, GETDATE(), 'Sistema');
        ELSE
            UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
            WHERE [IdRol] = 2 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionCrear;
        
        -- Editar
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 2 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEditar AND [FechaEliminacion] IS NULL)
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
            VALUES (2, @CurrentPantallaId, @IdOpcionEditar, GETDATE(), 'Sistema');
        ELSE
            UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
            WHERE [IdRol] = 2 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEditar;
        
        -- Eliminar
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 2 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEliminar AND [FechaEliminacion] IS NULL)
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
            VALUES (2, @CurrentPantallaId, @IdOpcionEliminar, GETDATE(), 'Sistema');
        ELSE
            UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
            WHERE [IdRol] = 2 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEliminar;
    END
    
    FETCH NEXT FROM admin_cursor INTO @CurrentPantallaId;
END

CLOSE admin_cursor;
DEALLOCATE admin_cursor;

PRINT 'Permisos Admin configurados';

-- =============================================
-- 6.3. ROL 3: Recepcion - Solo consultas y reservas
-- =============================================

PRINT 'Configurando permisos para Recepcion (Rol 3)...';

-- Recepcion tiene:
-- Ver en: Vuelos, Horarios, Pasajeros, Boletos, Reservas, Usuarios (solo para ver)
-- Crear/Editar en: Pasajeros, Boletos, Reservas
-- Sin Eliminar en ninguna pantalla

DECLARE @RecepcionPantallas TABLE ([IdPantalla] INT, [NombrePantalla] NVARCHAR(100), [Ver] BIT, [Crear] BIT, [Editar] BIT, [Eliminar] BIT);
INSERT INTO @RecepcionPantallas VALUES
(@IdPantallaVuelos, 'Vuelos', 1, 0, 0, 0),
(@IdPantallaHorarios, 'Horarios', 1, 0, 0, 0),
(@IdPantallaPasajeros, 'Pasajeros', 1, 1, 1, 0),
(@IdPantallaBoletos, 'Boletos', 1, 1, 1, 0),
(@IdPantallaReservas, 'Reservas', 1, 1, 1, 0),
(@IdPantallaUsuarios, 'Usuarios', 1, 0, 0, 0);

DECLARE recepcion_cursor CURSOR FOR
SELECT [IdPantalla], [Ver], [Crear], [Editar], [Eliminar] FROM @RecepcionPantallas;

DECLARE @RecepcionVer BIT, @RecepcionCrear BIT, @RecepcionEditar BIT, @RecepcionEliminar BIT;

OPEN recepcion_cursor;
FETCH NEXT FROM recepcion_cursor INTO @CurrentPantallaId, @RecepcionVer, @RecepcionCrear, @RecepcionEditar, @RecepcionEliminar;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @CurrentPantallaId IS NOT NULL
    BEGIN
        -- Ver
        IF @RecepcionVer = 1
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 3 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionVer AND [FechaEliminacion] IS NULL)
                INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
                VALUES (3, @CurrentPantallaId, @IdOpcionVer, GETDATE(), 'Sistema');
            ELSE
                UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
                WHERE [IdRol] = 3 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionVer;
        END
        ELSE
        BEGIN
            -- Eliminar permiso Ver si no debe tenerlo
            UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = GETDATE(), [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
            WHERE [IdRol] = 3 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionVer AND [FechaEliminacion] IS NULL;
        END
        
        -- Crear
        IF @RecepcionCrear = 1
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 3 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionCrear AND [FechaEliminacion] IS NULL)
                INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
                VALUES (3, @CurrentPantallaId, @IdOpcionCrear, GETDATE(), 'Sistema');
            ELSE
                UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
                WHERE [IdRol] = 3 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionCrear;
        END
        ELSE
        BEGIN
            UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = GETDATE(), [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
            WHERE [IdRol] = 3 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionCrear AND [FechaEliminacion] IS NULL;
        END
        
        -- Editar
        IF @RecepcionEditar = 1
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 3 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEditar AND [FechaEliminacion] IS NULL)
                INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
                VALUES (3, @CurrentPantallaId, @IdOpcionEditar, GETDATE(), 'Sistema');
            ELSE
                UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
                WHERE [IdRol] = 3 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEditar;
        END
        ELSE
        BEGIN
            UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = GETDATE(), [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
            WHERE [IdRol] = 3 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEditar AND [FechaEliminacion] IS NULL;
        END
        
        -- Eliminar - Recepcion NO tiene permiso de eliminar
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = GETDATE(), [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 3 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionEliminar AND [FechaEliminacion] IS NULL;
    END
    
    FETCH NEXT FROM recepcion_cursor INTO @CurrentPantallaId, @RecepcionVer, @RecepcionCrear, @RecepcionEditar, @RecepcionEliminar;
END

CLOSE recepcion_cursor;
DEALLOCATE recepcion_cursor;

PRINT 'Permisos Recepcion configurados';

-- =============================================
-- 6.4. ROL 4: Auditoria - Solo lectura en todo
-- =============================================

PRINT 'Configurando permisos para Auditoria (Rol 4)...';

-- Auditoria tiene SOLO Ver en TODAS las pantallas (sin Crear, Editar, Eliminar)

DECLARE auditoria_cursor CURSOR FOR
SELECT [IdPantalla]
FROM [dbo].[Pantallas]
WHERE [FechaEliminacion] IS NULL;

OPEN auditoria_cursor;
FETCH NEXT FROM auditoria_cursor INTO @CurrentPantallaId;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @CurrentPantallaId IS NOT NULL
    BEGIN
        -- Ver
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 4 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionVer AND [FechaEliminacion] IS NULL)
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
            VALUES (4, @CurrentPantallaId, @IdOpcionVer, GETDATE(), 'Sistema');
        ELSE
            UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
            WHERE [IdRol] = 4 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] = @IdOpcionVer;
        
        -- Eliminar permisos de Crear, Editar, Eliminar (si existen)
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = GETDATE(), [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 4 AND [IdPantalla] = @CurrentPantallaId AND [IdOpcion] IN (@IdOpcionCrear, @IdOpcionEditar, @IdOpcionEliminar) AND [FechaEliminacion] IS NULL;
    END
    
    FETCH NEXT FROM auditoria_cursor INTO @CurrentPantallaId;
END

CLOSE auditoria_cursor;
DEALLOCATE auditoria_cursor;

PRINT 'Permisos Auditoria configurados';

-- =============================================
-- 6.5. ROL 5: Usuario - Solo comprar boletos y modificar su perfil
-- =============================================

PRINT 'Configurando permisos para Usuario (Rol 5)...';

-- Usuario tiene:
-- Ver en: Boletos, Usuarios (solo para ver su propio perfil)
-- Crear en: Boletos (comprar boletos)
-- Editar en: Boletos (modificar su reserva), Usuarios (modificar su propio perfil)
-- Sin Eliminar

-- Boletos: Ver, Crear, Editar
IF @IdPantallaBoletos IS NOT NULL
BEGIN
    -- Ver
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaBoletos AND [IdOpcion] = @IdOpcionVer AND [FechaEliminacion] IS NULL)
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (5, @IdPantallaBoletos, @IdOpcionVer, GETDATE(), 'Sistema');
    ELSE
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaBoletos AND [IdOpcion] = @IdOpcionVer;
    
    -- Crear (comprar boletos)
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaBoletos AND [IdOpcion] = @IdOpcionCrear AND [FechaEliminacion] IS NULL)
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (5, @IdPantallaBoletos, @IdOpcionCrear, GETDATE(), 'Sistema');
    ELSE
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaBoletos AND [IdOpcion] = @IdOpcionCrear;
    
    -- Editar (modificar reserva)
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaBoletos AND [IdOpcion] = @IdOpcionEditar AND [FechaEliminacion] IS NULL)
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (5, @IdPantallaBoletos, @IdOpcionEditar, GETDATE(), 'Sistema');
    ELSE
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaBoletos AND [IdOpcion] = @IdOpcionEditar;
    
    -- Eliminar - NO tiene permiso
    UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = GETDATE(), [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
    WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaBoletos AND [IdOpcion] = @IdOpcionEliminar AND [FechaEliminacion] IS NULL;
END

-- Usuarios: Ver, Editar (solo su propio perfil)
IF @IdPantallaUsuarios IS NOT NULL
BEGIN
    -- Ver
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaUsuarios AND [IdOpcion] = @IdOpcionVer AND [FechaEliminacion] IS NULL)
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (5, @IdPantallaUsuarios, @IdOpcionVer, GETDATE(), 'Sistema');
    ELSE
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaUsuarios AND [IdOpcion] = @IdOpcionVer;
    
    -- Editar (modificar su propio perfil)
    IF NOT EXISTS (SELECT 1 FROM [dbo].[RolPantallaOpcion] WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaUsuarios AND [IdOpcion] = @IdOpcionEditar AND [FechaEliminacion] IS NULL)
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [FechaCreacion], [UsuarioCreacion])
        VALUES (5, @IdPantallaUsuarios, @IdOpcionEditar, GETDATE(), 'Sistema');
    ELSE
        UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = NULL, [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
        WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaUsuarios AND [IdOpcion] = @IdOpcionEditar;
    
    -- Crear - NO tiene permiso
    UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = GETDATE(), [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
    WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaUsuarios AND [IdOpcion] = @IdOpcionCrear AND [FechaEliminacion] IS NULL;
    
    -- Eliminar - NO tiene permiso
    UPDATE [dbo].[RolPantallaOpcion] SET [FechaEliminacion] = GETDATE(), [FechaActualizacion] = GETDATE(), [UsuarioActualizacion] = 'Sistema'
    WHERE [IdRol] = 5 AND [IdPantalla] = @IdPantallaUsuarios AND [IdOpcion] = @IdOpcionEliminar AND [FechaEliminacion] IS NULL;
END

PRINT 'Permisos Usuario configurados';

-- =============================================
-- FINALIZAR TRANSACCIÓN
-- =============================================

COMMIT TRANSACTION;

PRINT '============================================';
PRINT 'SCRIPT EJECUTADO EXITOSAMENTE';
PRINT '============================================';
PRINT 'Roles creados: 5 (SuperAdmin, Admin, Recepcion, Auditoria, Usuario)';
PRINT 'Pantallas creadas: 19';
PRINT 'Permisos configurados correctamente para cada rol';
PRINT '============================================';

GO

