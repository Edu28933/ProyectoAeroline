-- =============================================
-- ==========  STORED PROCEDURES ROLPANTALLAOPCION (CORREGIDO)  ==========
-- =============================================
-- Este script usa la estructura real: IdRol, IdPantalla, IdOpcion
-- y agrupa las opciones por Rol y Pantalla

USE [AerolineaPruebaDB]
GO

-- 1. sp_RolPantallaOpcionesSeleccionar - Listar todos los permisos agrupados
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionesSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Agrupar por IdRol e IdPantalla y consolidar las opciones en una sola fila
    SELECT 
        MIN(RPO.[IdRolPantallaOpcion]) AS [IdRolPantallaOpcion],
        RPO.[IdRol],
        RPO.[IdPantalla],
        MAX(CASE WHEN O.[NombreOpcion] = 'Ver' THEN 1 ELSE 0 END) AS [Ver],
        MAX(CASE WHEN O.[NombreOpcion] = 'Crear' THEN 1 ELSE 0 END) AS [Crear],
        MAX(CASE WHEN O.[NombreOpcion] = 'Editar' THEN 1 ELSE 0 END) AS [Editar],
        MAX(CASE WHEN O.[NombreOpcion] = 'Eliminar' THEN 1 ELSE 0 END) AS [Eliminar],
        CASE 
            WHEN MAX(RPO.[FechaEliminacion]) IS NULL THEN 'Activo'
            ELSE 'Inactivo'
        END AS [Estado],
        ISNULL(MAX(R.[NombreRol]), '') AS [NombreRol],
        ISNULL(MAX(P.[NombrePantalla]), '') AS [NombrePantalla]
    FROM [dbo].[RolPantallaOpcion] RPO
    LEFT JOIN [dbo].[Roles] R ON RPO.[IdRol] = R.[IdRol]
    LEFT JOIN [dbo].[Pantallas] P ON RPO.[IdPantalla] = P.[IdPantalla]
    LEFT JOIN [dbo].[Opciones] O ON RPO.[IdOpcion] = O.[IdOpcion]
    WHERE RPO.[FechaEliminacion] IS NULL
    GROUP BY RPO.[IdRol], RPO.[IdPantalla]
    ORDER BY MIN(RPO.[IdRolPantallaOpcion])
END
GO

-- 2. sp_RolPantallaOpcionBuscar - Buscar un permiso por ID (busca el primero del grupo)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionBuscar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionBuscar]
    @IdRolPantallaOpcion INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Obtener IdRol e IdPantalla del registro especificado
    DECLARE @IdRol INT, @IdPantalla INT
    
    SELECT @IdRol = [IdRol], @IdPantalla = [IdPantalla]
    FROM [dbo].[RolPantallaOpcion]
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    
    IF @IdRol IS NULL OR @IdPantalla IS NULL
    BEGIN
        RETURN
    END
    
    -- Agrupar por IdRol e IdPantalla y consolidar las opciones
    SELECT 
        MIN(RPO.[IdRolPantallaOpcion]) AS [IdRolPantallaOpcion],
        RPO.[IdRol],
        RPO.[IdPantalla],
        MAX(CASE WHEN O.[NombreOpcion] = 'Ver' THEN 1 ELSE 0 END) AS [Ver],
        MAX(CASE WHEN O.[NombreOpcion] = 'Crear' THEN 1 ELSE 0 END) AS [Crear],
        MAX(CASE WHEN O.[NombreOpcion] = 'Editar' THEN 1 ELSE 0 END) AS [Editar],
        MAX(CASE WHEN O.[NombreOpcion] = 'Eliminar' THEN 1 ELSE 0 END) AS [Eliminar],
        CASE 
            WHEN MAX(RPO.[FechaEliminacion]) IS NULL THEN 'Activo'
            ELSE 'Inactivo'
        END AS [Estado],
        ISNULL(MAX(R.[NombreRol]), '') AS [NombreRol],
        ISNULL(MAX(P.[NombrePantalla]), '') AS [NombrePantalla]
    FROM [dbo].[RolPantallaOpcion] RPO
    LEFT JOIN [dbo].[Roles] R ON RPO.[IdRol] = R.[IdRol]
    LEFT JOIN [dbo].[Pantallas] P ON RPO.[IdPantalla] = P.[IdPantalla]
    LEFT JOIN [dbo].[Opciones] O ON RPO.[IdOpcion] = O.[IdOpcion]
    WHERE RPO.[IdRol] = @IdRol 
      AND RPO.[IdPantalla] = @IdPantalla
      AND RPO.[FechaEliminacion] IS NULL
    GROUP BY RPO.[IdRol], RPO.[IdPantalla]
END
GO

-- 3. sp_RolPantallaOpcionAgregar - Agregar permisos para un Rol y Pantalla
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
    
    -- Eliminar permisos existentes para este Rol y Pantalla
    DELETE FROM [dbo].[RolPantallaOpcion]
    WHERE [IdRol] = @IdRol 
      AND [IdPantalla] = @IdPantalla
      AND [FechaEliminacion] IS NULL
    
    -- Agregar permisos según las opciones seleccionadas
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
GO

-- 4. sp_RolPantallaOpcionModificar - Modificar permisos para un Rol y Pantalla
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
    
    -- Eliminar permisos existentes para este Rol y Pantalla
    UPDATE [dbo].[RolPantallaOpcion]
    SET 
        [UsuarioEliminacion] = @UsuarioActualizacion,
        [FechaEliminacion] = GETDATE(),
        [HoraEliminacion] = CAST(GETDATE() AS TIME(7))
    WHERE [IdRol] = @IdRol 
      AND [IdPantalla] = @IdPantalla
      AND [FechaEliminacion] IS NULL
    
    -- Agregar los nuevos permisos usando el mismo método que Agregar
    EXEC [dbo].[sp_RolPantallaOpcionAgregar] 
        @IdRol = @IdRol,
        @IdPantalla = @IdPantalla,
        @Ver = @Ver,
        @Crear = @Crear,
        @Editar = @Editar,
        @Eliminar = @Eliminar,
        @Estado = @Estado,
        @UsuarioCreacion = @UsuarioActualizacion
END
GO

-- 5. sp_RolPantallaOpcionEliminar - Eliminar todos los permisos de un Rol y Pantalla
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionEliminar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionEliminar]
    @IdRolPantallaOpcion INT,
    @UsuarioEliminacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Obtener IdRol e IdPantalla del registro especificado
    DECLARE @IdRol INT, @IdPantalla INT
    
    SELECT @IdRol = [IdRol], @IdPantalla = [IdPantalla]
    FROM [dbo].[RolPantallaOpcion]
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    
    IF @IdRol IS NOT NULL AND @IdPantalla IS NOT NULL
    BEGIN
        -- Eliminar físicamente todos los permisos de este Rol y Pantalla
        DELETE FROM [dbo].[RolPantallaOpcion]
        WHERE [IdRol] = @IdRol 
          AND [IdPantalla] = @IdPantalla
    END
END
GO

PRINT 'Stored Procedures creados exitosamente para RolPantallaOpcion (usando estructura con IdOpcion)'
GO

