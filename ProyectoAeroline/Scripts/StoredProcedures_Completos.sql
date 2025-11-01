-- =============================================
-- STORED PROCEDURES COMPLETOS PARA ROLES, PANTALLAS Y ROLPANTALLAOPCION
-- Basados en la estructura real de la base de datos AerolineaPruebaDB
-- Fecha: 2025-11-01
-- =============================================

USE [AerolineaPruebaDB]
GO

-- =============================================
-- ==========  STORED PROCEDURES ROLES  ==========
-- =============================================

-- 1. sp_RolesSeleccionar - Listar todos los roles
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolesSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolesSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_RolesSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [IdRol],
        [NombreRol],
        [UsuarioCreacion],
        [FechaCreacion],
        [HoraCreacion],
        [UsuarioActualizacion],
        [FechaActualizacion],
        [HoraActualizacion],
        [UsuarioEliminacion],
        [FechaEliminacion],
        [HoraEliminacion]
    FROM [dbo].[Roles]
    WHERE [FechaEliminacion] IS NULL  -- Solo roles no eliminados
    ORDER BY [IdRol]
END
GO

-- 2. sp_RolBuscar - Buscar un rol por ID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolBuscar]
GO

CREATE PROCEDURE [dbo].[sp_RolBuscar]
    @IdRol INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [IdRol],
        [NombreRol],
        [UsuarioCreacion],
        [FechaCreacion],
        [HoraCreacion],
        [UsuarioActualizacion],
        [FechaActualizacion],
        [HoraActualizacion],
        [UsuarioEliminacion],
        [FechaEliminacion],
        [HoraEliminacion]
    FROM [dbo].[Roles]
    WHERE [IdRol] = @IdRol
END
GO

-- 3. sp_RolAgregar - Agregar un nuevo rol
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolAgregar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolAgregar]
GO

CREATE PROCEDURE [dbo].[sp_RolAgregar]
    @NombreRol VARCHAR(45),
    @UsuarioCreacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Roles] 
        ([NombreRol], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (@NombreRol, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
END
GO

-- 4. sp_RolModificar - Modificar un rol existente
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolModificar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolModificar]
GO

CREATE PROCEDURE [dbo].[sp_RolModificar]
    @IdRol INT,
    @NombreRol VARCHAR(45),
    @UsuarioActualizacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Roles]
    SET 
        [NombreRol] = @NombreRol,
        [UsuarioActualizacion] = @UsuarioActualizacion,
        [FechaActualizacion] = GETDATE(),
        [HoraActualizacion] = CAST(GETDATE() AS TIME(7))
    WHERE [IdRol] = @IdRol
END
GO

-- 5. sp_RolEliminar - Eliminar un rol (lógico)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolEliminar]
GO

CREATE PROCEDURE [dbo].[sp_RolEliminar]
    @IdRol INT,
    @UsuarioEliminacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UsuariosCount INT;
    
    -- Verificar si hay usuarios usando este rol
    SELECT @UsuariosCount = COUNT(*)
    FROM [dbo].[Usuarios]
    WHERE [IdRol] = @IdRol;
    
    IF @UsuariosCount > 0
    BEGIN
        -- Si hay usuarios, solo marcar como eliminado (lógico)
        UPDATE [dbo].[Roles]
        SET 
            [UsuarioEliminacion] = @UsuarioEliminacion,
            [FechaEliminacion] = GETDATE(),
            [HoraEliminacion] = CAST(GETDATE() AS TIME(7))
        WHERE [IdRol] = @IdRol;
    END
    ELSE
    BEGIN
        -- Si no hay usuarios, eliminar físicamente
        DELETE FROM [dbo].[Roles]
        WHERE [IdRol] = @IdRol;
    END
END
GO

-- =============================================
-- ==========  STORED PROCEDURES PANTALLAS  ==========
-- =============================================

-- 1. sp_PantallasSeleccionar - Listar todas las pantallas
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PantallasSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_PantallasSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_PantallasSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [IdPantalla],
        [NombrePantalla],
        [UsuarioCreacion],
        [FechaCreacion],
        [HoraCreacion],
        [UsuarioActualizacion],
        [FechaActualizacion],
        [HoraActualizacion],
        [UsuarioEliminacion],
        [FechaEliminacion],
        [HoraEliminacion]
    FROM [dbo].[Pantallas]
    WHERE [FechaEliminacion] IS NULL  -- Solo pantallas no eliminadas
    ORDER BY [IdPantalla]
END
GO

-- 2. sp_PantallaBuscar - Buscar una pantalla por ID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PantallaBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_PantallaBuscar]
GO

CREATE PROCEDURE [dbo].[sp_PantallaBuscar]
    @IdPantalla INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [IdPantalla],
        [NombrePantalla],
        [UsuarioCreacion],
        [FechaCreacion],
        [HoraCreacion],
        [UsuarioActualizacion],
        [FechaActualizacion],
        [HoraActualizacion],
        [UsuarioEliminacion],
        [FechaEliminacion],
        [HoraEliminacion]
    FROM [dbo].[Pantallas]
    WHERE [IdPantalla] = @IdPantalla
END
GO

-- 3. sp_PantallaAgregar - Agregar una nueva pantalla
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PantallaAgregar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_PantallaAgregar]
GO

CREATE PROCEDURE [dbo].[sp_PantallaAgregar]
    @NombrePantalla VARCHAR(45),
    @UsuarioCreacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Pantallas] 
        ([NombrePantalla], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (@NombrePantalla, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
END
GO

-- 4. sp_PantallaModificar - Modificar una pantalla existente
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PantallaModificar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_PantallaModificar]
GO

CREATE PROCEDURE [dbo].[sp_PantallaModificar]
    @IdPantalla INT,
    @NombrePantalla VARCHAR(45),
    @UsuarioActualizacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Pantallas]
    SET 
        [NombrePantalla] = @NombrePantalla,
        [UsuarioActualizacion] = @UsuarioActualizacion,
        [FechaActualizacion] = GETDATE(),
        [HoraActualizacion] = CAST(GETDATE() AS TIME(7))
    WHERE [IdPantalla] = @IdPantalla
END
GO

-- 5. sp_PantallaEliminar - Eliminar una pantalla
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PantallaEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_PantallaEliminar]
GO

CREATE PROCEDURE [dbo].[sp_PantallaEliminar]
    @IdPantalla INT,
    @UsuarioEliminacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Primero eliminar relaciones en RolPantallaOpcion
    DELETE FROM [dbo].[RolPantallaOpcion]
    WHERE [IdPantalla] = @IdPantalla;
    
    -- Ahora eliminar la pantalla físicamente
    DELETE FROM [dbo].[Pantallas]
    WHERE [IdPantalla] = @IdPantalla;
END
GO

-- =============================================
-- ==========  STORED PROCEDURES ROLPANTALLAOPCION  ==========
-- =============================================

-- 1. sp_RolPantallaOpcionesSeleccionar - Listar todos los permisos con información relacionada
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionesSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Intentar primero con estructura que incluye Ver, Crear, Editar, Eliminar directamente
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'RolPantallaOpcion' AND COLUMN_NAME = 'Ver')
    BEGIN
        SELECT 
            RPO.[IdRolPantallaOpcion],
            RPO.[IdRol],
            RPO.[IdPantalla],
            ISNULL(RPO.[Ver], 0) AS [Ver],
            ISNULL(RPO.[Crear], 0) AS [Crear],
            ISNULL(RPO.[Editar], 0) AS [Editar],
            ISNULL(RPO.[Eliminar], 0) AS [Eliminar],
            CASE 
                WHEN RPO.[FechaEliminacion] IS NULL THEN 'Activo'
                ELSE 'Inactivo'
            END AS [Estado],
            ISNULL(R.[NombreRol], '') AS [NombreRol],
            ISNULL(P.[NombrePantalla], '') AS [NombrePantalla]
        FROM [dbo].[RolPantallaOpcion] RPO
        LEFT JOIN [dbo].[Roles] R ON RPO.[IdRol] = R.[IdRol]
        LEFT JOIN [dbo].[Pantallas] P ON RPO.[IdPantalla] = P.[IdPantalla]
        WHERE RPO.[FechaEliminacion] IS NULL
        ORDER BY RPO.[IdRolPantallaOpcion]
    END
    ELSE
    BEGIN
        -- Si no tiene esos campos, usar IdOpcion y mapear desde Opciones
        SELECT 
            RPO.[IdRolPantallaOpcion],
            RPO.[IdRol],
            RPO.[IdPantalla],
            CASE WHEN O.[NombreOpcion] = 'Ver' THEN 1 ELSE 0 END AS [Ver],
            CASE WHEN O.[NombreOpcion] = 'Crear' THEN 1 ELSE 0 END AS [Crear],
            CASE WHEN O.[NombreOpcion] = 'Editar' THEN 1 ELSE 0 END AS [Editar],
            CASE WHEN O.[NombreOpcion] = 'Eliminar' THEN 1 ELSE 0 END AS [Eliminar],
            CASE 
                WHEN RPO.[FechaEliminacion] IS NULL THEN 'Activo'
                ELSE 'Inactivo'
            END AS [Estado],
            ISNULL(R.[NombreRol], '') AS [NombreRol],
            ISNULL(P.[NombrePantalla], '') AS [NombrePantalla]
        FROM [dbo].[RolPantallaOpcion] RPO
        LEFT JOIN [dbo].[Roles] R ON RPO.[IdRol] = R.[IdRol]
        LEFT JOIN [dbo].[Pantallas] P ON RPO.[IdPantalla] = P.[IdPantalla]
        LEFT JOIN [dbo].[Opciones] O ON RPO.[IdOpcion] = O.[IdOpcion]
        WHERE RPO.[FechaEliminacion] IS NULL
        ORDER BY RPO.[IdRolPantallaOpcion]
    END
END
GO

-- 2. sp_RolPantallaOpcionBuscar - Buscar un permiso por ID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionBuscar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionBuscar]
    @IdRolPantallaOpcion INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Intentar primero con estructura que incluye Ver, Crear, Editar, Eliminar directamente
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'RolPantallaOpcion' AND COLUMN_NAME = 'Ver')
    BEGIN
        SELECT 
            RPO.[IdRolPantallaOpcion],
            RPO.[IdRol],
            RPO.[IdPantalla],
            ISNULL(RPO.[Ver], 0) AS [Ver],
            ISNULL(RPO.[Crear], 0) AS [Crear],
            ISNULL(RPO.[Editar], 0) AS [Editar],
            ISNULL(RPO.[Eliminar], 0) AS [Eliminar],
            CASE 
                WHEN RPO.[FechaEliminacion] IS NULL THEN 'Activo'
                ELSE 'Inactivo'
            END AS [Estado],
            ISNULL(R.[NombreRol], '') AS [NombreRol],
            ISNULL(P.[NombrePantalla], '') AS [NombrePantalla]
        FROM [dbo].[RolPantallaOpcion] RPO
        LEFT JOIN [dbo].[Roles] R ON RPO.[IdRol] = R.[IdRol]
        LEFT JOIN [dbo].[Pantallas] P ON RPO.[IdPantalla] = P.[IdPantalla]
        WHERE RPO.[IdRolPantallaOpcion] = @IdRolPantallaOpcion
    END
    ELSE
    BEGIN
        -- Si no tiene esos campos, usar IdOpcion y mapear desde Opciones
        SELECT 
            RPO.[IdRolPantallaOpcion],
            RPO.[IdRol],
            RPO.[IdPantalla],
            CASE WHEN O.[NombreOpcion] = 'Ver' THEN 1 ELSE 0 END AS [Ver],
            CASE WHEN O.[NombreOpcion] = 'Crear' THEN 1 ELSE 0 END AS [Crear],
            CASE WHEN O.[NombreOpcion] = 'Editar' THEN 1 ELSE 0 END AS [Editar],
            CASE WHEN O.[NombreOpcion] = 'Eliminar' THEN 1 ELSE 0 END AS [Eliminar],
            CASE 
                WHEN RPO.[FechaEliminacion] IS NULL THEN 'Activo'
                ELSE 'Inactivo'
            END AS [Estado],
            ISNULL(R.[NombreRol], '') AS [NombreRol],
            ISNULL(P.[NombrePantalla], '') AS [NombrePantalla]
        FROM [dbo].[RolPantallaOpcion] RPO
        LEFT JOIN [dbo].[Roles] R ON RPO.[IdRol] = R.[IdRol]
        LEFT JOIN [dbo].[Pantallas] P ON RPO.[IdPantalla] = P.[IdPantalla]
        LEFT JOIN [dbo].[Opciones] O ON RPO.[IdOpcion] = O.[IdOpcion]
        WHERE RPO.[IdRolPantallaOpcion] = @IdRolPantallaOpcion
    END
END
GO

-- 3. sp_RolPantallaOpcionAgregar - Agregar un nuevo permiso
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionAgregar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionAgregar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionAgregar]
    @IdRol INT,
    @IdPantalla INT,
    @Ver BIT = 0,
    @Crear BIT = 0,
    @Editar BIT = 0,
    @Eliminar BIT = 0,
    @Estado NVARCHAR(50) = 'Activo',
    @UsuarioCreacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Si la tabla tiene Ver, Crear, Editar, Eliminar directamente
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'RolPantallaOpcion' AND COLUMN_NAME = 'Ver')
    BEGIN
        INSERT INTO [dbo].[RolPantallaOpcion] 
            ([IdRol], [IdPantalla], [Ver], [Crear], [Editar], [Eliminar], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
        VALUES 
            (@IdRol, @IdPantalla, @Ver, @Crear, @Editar, @Eliminar, @Estado, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
    END
    ELSE
    BEGIN
        -- Si usa IdOpcion, crear registros para cada opción seleccionada
        IF @Ver = 1
        BEGIN
            DECLARE @IdOpcionVer INT = (SELECT TOP 1 [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Ver')
            IF @IdOpcionVer IS NOT NULL
            BEGIN
                INSERT INTO [dbo].[RolPantallaOpcion] 
                    ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
                VALUES 
                    (@IdRol, @IdPantalla, @IdOpcionVer, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
            END
        END
        
        IF @Crear = 1
        BEGIN
            DECLARE @IdOpcionCrear INT = (SELECT TOP 1 [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Crear')
            IF @IdOpcionCrear IS NOT NULL
            BEGIN
                INSERT INTO [dbo].[RolPantallaOpcion] 
                    ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
                VALUES 
                    (@IdRol, @IdPantalla, @IdOpcionCrear, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
            END
        END
        
        IF @Editar = 1
        BEGIN
            DECLARE @IdOpcionEditar INT = (SELECT TOP 1 [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Editar')
            IF @IdOpcionEditar IS NOT NULL
            BEGIN
                INSERT INTO [dbo].[RolPantallaOpcion] 
                    ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
                VALUES 
                    (@IdRol, @IdPantalla, @IdOpcionEditar, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
            END
        END
        
        IF @Eliminar = 1
        BEGIN
            DECLARE @IdOpcionEliminar INT = (SELECT TOP 1 [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Eliminar')
            IF @IdOpcionEliminar IS NOT NULL
            BEGIN
                INSERT INTO [dbo].[RolPantallaOpcion] 
                    ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
                VALUES 
                    (@IdRol, @IdPantalla, @IdOpcionEliminar, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
            END
        END
    END
END
GO

-- 4. sp_RolPantallaOpcionModificar - Modificar un permiso existente
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionModificar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionModificar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionModificar]
    @IdRolPantallaOpcion INT,
    @IdRol INT,
    @IdPantalla INT,
    @Ver BIT = 0,
    @Crear BIT = 0,
    @Editar BIT = 0,
    @Eliminar BIT = 0,
    @Estado NVARCHAR(50) = 'Activo',
    @UsuarioActualizacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Si la tabla tiene Ver, Crear, Editar, Eliminar directamente
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'RolPantallaOpcion' AND COLUMN_NAME = 'Ver')
    BEGIN
        UPDATE [dbo].[RolPantallaOpcion]
        SET 
            [IdRol] = @IdRol,
            [IdPantalla] = @IdPantalla,
            [Ver] = @Ver,
            [Crear] = @Crear,
            [Editar] = @Editar,
            [Eliminar] = @Eliminar,
            [Estado] = @Estado,
            [UsuarioActualizacion] = @UsuarioActualizacion,
            [FechaActualizacion] = GETDATE(),
            [HoraActualizacion] = CAST(GETDATE() AS TIME(7))
        WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    END
    ELSE
    BEGIN
        -- Si usa IdOpcion, necesitaríamos una lógica más compleja
        -- Por ahora, simplemente actualizar lo básico
        UPDATE [dbo].[RolPantallaOpcion]
        SET 
            [IdRol] = @IdRol,
            [IdPantalla] = @IdPantalla,
            [UsuarioActualizacion] = @UsuarioActualizacion,
            [FechaActualizacion] = GETDATE(),
            [HoraActualizacion] = CAST(GETDATE() AS TIME(7))
        WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    END
END
GO

-- 5. sp_RolPantallaOpcionEliminar - Eliminar un permiso
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionEliminar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionEliminar]
    @IdRolPantallaOpcion INT,
    @UsuarioEliminacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Eliminar físicamente (o marcar como eliminado según tu lógica)
    DELETE FROM [dbo].[RolPantallaOpcion]
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    
    -- Si prefieres eliminación lógica, descomenta esto:
    /*
    UPDATE [dbo].[RolPantallaOpcion]
    SET 
        [UsuarioEliminacion] = @UsuarioEliminacion,
        [FechaEliminacion] = GETDATE(),
        [HoraEliminacion] = CAST(GETDATE() AS TIME(7))
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    */
END
GO

PRINT 'Stored Procedures creados exitosamente para Roles, Pantallas y RolPantallaOpcion'
GO

