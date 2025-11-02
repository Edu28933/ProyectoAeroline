-- =============================================
-- STORED PROCEDURES PARA ROLPANTALLAOPCION
-- Basado en el esquema real de la base de datos
-- =============================================

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

-- 2. sp_RolPantallaOpcionBuscar - Buscar un permiso por ID
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
        ISNULL(MAX(P.[NombrePantalla]), '') AS [NombrePantalla]]
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
    @Ver BIT,
    @Crear BIT,
    @Editar BIT,
    @Eliminar BIT,
    @Estado VARCHAR(15) = 'Activo',
    @UsuarioCreacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Eliminar permisos existentes para esta combinación de Rol y Pantalla
        DELETE FROM [dbo].[RolPantallaOpcion]
        WHERE [IdRol] = @IdRol 
          AND [IdPantalla] = @IdPantalla
          AND [FechaEliminacion] IS NULL;
        
        -- Obtener IDs de las opciones
        DECLARE @IdOpcionVer INT, @IdOpcionCrear INT, @IdOpcionEditar INT, @IdOpcionEliminar INT
        
        SELECT @IdOpcionVer = [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Ver'
        SELECT @IdOpcionCrear = [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Crear'
        SELECT @IdOpcionEditar = [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Editar'
        SELECT @IdOpcionEliminar = [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Eliminar'
        
        -- Insertar las opciones seleccionadas
        IF @Ver = 1 AND @IdOpcionVer IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionVer, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
        END
        
        IF @Crear = 1 AND @IdOpcionCrear IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionCrear, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
        END
        
        IF @Editar = 1 AND @IdOpcionEditar IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionEditar, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
        END
        
        IF @Eliminar = 1 AND @IdOpcionEliminar IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionEliminar, @UsuarioCreacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
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
    @Ver BIT,
    @Crear BIT,
    @Editar BIT,
    @Eliminar BIT,
    @Estado VARCHAR(15) = 'Activo',
    @UsuarioActualizacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Eliminar permisos existentes para esta combinación de Rol y Pantalla
        DELETE FROM [dbo].[RolPantallaOpcion]
        WHERE [IdRol] = @IdRol 
          AND [IdPantalla] = @IdPantalla
          AND [FechaEliminacion] IS NULL;
        
        -- Obtener IDs de las opciones
        DECLARE @IdOpcionVer INT, @IdOpcionCrear INT, @IdOpcionEditar INT, @IdOpcionEliminar INT
        
        SELECT @IdOpcionVer = [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Ver'
        SELECT @IdOpcionCrear = [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Crear'
        SELECT @IdOpcionEditar = [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Editar'
        SELECT @IdOpcionEliminar = [IdOpcion] FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Eliminar'
        
        -- Insertar las opciones seleccionadas
        IF @Ver = 1 AND @IdOpcionVer IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionVer, @UsuarioActualizacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
        END
        
        IF @Crear = 1 AND @IdOpcionCrear IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionCrear, @UsuarioActualizacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
        END
        
        IF @Editar = 1 AND @IdOpcionEditar IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionEditar, @UsuarioActualizacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
        END
        
        IF @Eliminar = 1 AND @IdOpcionEliminar IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionEliminar, @UsuarioActualizacion, GETDATE(), CAST(GETDATE() AS TIME(7)))
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- 5. sp_RolPantallaOpcionEliminar - Eliminar permisos para un Rol y Pantalla
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
    
    IF @IdRol IS NULL OR @IdPantalla IS NULL
    BEGIN
        RETURN
    END
    
    -- Eliminar físicamente todos los permisos para esta combinación de Rol y Pantalla
    DELETE FROM [dbo].[RolPantallaOpcion]
    WHERE [IdRol] = @IdRol 
      AND [IdPantalla] = @IdPantalla
END
GO

PRINT 'Stored Procedures creados exitosamente para RolPantallaOpcion'
GO

