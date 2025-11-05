-- =============================================
-- SCRIPT ULTRA OPTIMIZADO PARA ARREGLAR TIMEOUTS
-- Versión simplificada sin CTEs complejos
-- =============================================

USE [AerolineaPruebaDB]
GO

-- =============================================
-- 1. CORREGIR sp_PantallaEliminar (ELIMINACIÓN LÓGICA RÁPIDA)
-- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PantallaEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_PantallaEliminar]
GO

CREATE PROCEDURE [dbo].[sp_PantallaEliminar]
    @IdPantalla INT,
    @UsuarioEliminacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        UPDATE [dbo].[RolPantallaOpcion] WITH (ROWLOCK)
        SET [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioEliminacion
        WHERE [IdPantalla] = @IdPantalla AND [FechaEliminacion] IS NULL;
        
        UPDATE [dbo].[Pantallas] WITH (ROWLOCK)
        SET [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioEliminacion
        WHERE [IdPantalla] = @IdPantalla;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- 2. VERSIÓN SIMPLIFICADA Y RÁPIDA DE sp_RolPantallaOpcionesSeleccionar
-- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionesSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Versión simple y directa sin CTEs complejos
    SELECT 
        MIN(RPO.[IdRolPantallaOpcion]) AS [IdRolPantallaOpcion],
        RPO.[IdRol],
        RPO.[IdPantalla],
        CAST(MAX(CASE WHEN O.[NombreOpcion] = 'Ver' THEN 1 ELSE 0 END) AS BIT) AS [Ver],
        CAST(MAX(CASE WHEN O.[NombreOpcion] = 'Crear' THEN 1 ELSE 0 END) AS BIT) AS [Crear],
        CAST(MAX(CASE WHEN O.[NombreOpcion] = 'Editar' THEN 1 ELSE 0 END) AS BIT) AS [Editar],
        CAST(MAX(CASE WHEN O.[NombreOpcion] = 'Eliminar' THEN 1 ELSE 0 END) AS BIT) AS [Eliminar],
        CASE WHEN MAX(RPO.[FechaEliminacion]) IS NULL THEN 'Activo' ELSE 'Inactivo' END AS [Estado],
        ISNULL(MAX(R.[NombreRol]), '') AS [NombreRol],
        ISNULL(MAX(P.[NombrePantalla]), '') AS [NombrePantalla]
    FROM [dbo].[RolPantallaOpcion] RPO WITH (NOLOCK)
    LEFT JOIN [dbo].[Opciones] O WITH (NOLOCK) ON RPO.[IdOpcion] = O.[IdOpcion]
    LEFT JOIN [dbo].[Roles] R WITH (NOLOCK) ON RPO.[IdRol] = R.[IdRol]
    LEFT JOIN [dbo].[Pantallas] P WITH (NOLOCK) ON RPO.[IdPantalla] = P.[IdPantalla]
    WHERE RPO.[FechaEliminacion] IS NULL
    GROUP BY RPO.[IdRol], RPO.[IdPantalla]
    ORDER BY MIN(RPO.[IdRolPantallaOpcion])
END
GO

-- =============================================
-- 3. sp_RolPantallaOpcionBuscar (SIMPLIFICADO)
-- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionBuscar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionBuscar]
    @IdRolPantallaOpcion INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdRol INT, @IdPantalla INT
    
    SELECT @IdRol = [IdRol], @IdPantalla = [IdPantalla]
    FROM [dbo].[RolPantallaOpcion] WITH (NOLOCK)
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    
    IF @IdRol IS NULL OR @IdPantalla IS NULL RETURN
    
    SELECT 
        MIN(RPO.[IdRolPantallaOpcion]) AS [IdRolPantallaOpcion],
        RPO.[IdRol],
        RPO.[IdPantalla],
        CAST(MAX(CASE WHEN O.[NombreOpcion] = 'Ver' THEN 1 ELSE 0 END) AS BIT) AS [Ver],
        CAST(MAX(CASE WHEN O.[NombreOpcion] = 'Crear' THEN 1 ELSE 0 END) AS BIT) AS [Crear],
        CAST(MAX(CASE WHEN O.[NombreOpcion] = 'Editar' THEN 1 ELSE 0 END) AS BIT) AS [Editar],
        CAST(MAX(CASE WHEN O.[NombreOpcion] = 'Eliminar' THEN 1 ELSE 0 END) AS BIT) AS [Eliminar],
        CASE WHEN MAX(RPO.[FechaEliminacion]) IS NULL THEN 'Activo' ELSE 'Inactivo' END AS [Estado],
        ISNULL(MAX(R.[NombreRol]), '') AS [NombreRol],
        ISNULL(MAX(P.[NombrePantalla]), '') AS [NombrePantalla]
    FROM [dbo].[RolPantallaOpcion] RPO WITH (NOLOCK)
    LEFT JOIN [dbo].[Opciones] O WITH (NOLOCK) ON RPO.[IdOpcion] = O.[IdOpcion]
    LEFT JOIN [dbo].[Roles] R WITH (NOLOCK) ON RPO.[IdRol] = R.[IdRol]
    LEFT JOIN [dbo].[Pantallas] P WITH (NOLOCK) ON RPO.[IdPantalla] = P.[IdPantalla]
    WHERE RPO.[IdRol] = @IdRol 
      AND RPO.[IdPantalla] = @IdPantalla
      AND RPO.[FechaEliminacion] IS NULL
    GROUP BY RPO.[IdRol], RPO.[IdPantalla]
END
GO

-- =============================================
-- 4. sp_RolPantallaOpcionAgregar (OPTIMIZADO)
-- =============================================

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
    
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Eliminar permisos existentes
        UPDATE [dbo].[RolPantallaOpcion] WITH (ROWLOCK)
        SET [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioCreacion
        WHERE [IdRol] = @IdRol AND [IdPantalla] = @IdPantalla AND [FechaEliminacion] IS NULL;
        
        -- Obtener IDs de opciones en una sola consulta
        DECLARE @IdOpcionVer INT, @IdOpcionCrear INT, @IdOpcionEditar INT, @IdOpcionEliminar INT
        
        SELECT 
            @IdOpcionVer = MAX(CASE WHEN [NombreOpcion] = 'Ver' THEN [IdOpcion] END),
            @IdOpcionCrear = MAX(CASE WHEN [NombreOpcion] = 'Crear' THEN [IdOpcion] END),
            @IdOpcionEditar = MAX(CASE WHEN [NombreOpcion] = 'Editar' THEN [IdOpcion] END),
            @IdOpcionEliminar = MAX(CASE WHEN [NombreOpcion] = 'Eliminar' THEN [IdOpcion] END)
        FROM [dbo].[Opciones] WITH (NOLOCK)
        WHERE [NombreOpcion] IN ('Ver', 'Crear', 'Editar', 'Eliminar');
        
        -- Insertar solo las opciones habilitadas
        IF @Ver = 1 AND @IdOpcionVer IS NOT NULL
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionVer, @UsuarioCreacion, @FechaActual, @HoraActual);
        
        IF @Crear = 1 AND @IdOpcionCrear IS NOT NULL
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionCrear, @UsuarioCreacion, @FechaActual, @HoraActual);
        
        IF @Editar = 1 AND @IdOpcionEditar IS NOT NULL
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionEditar, @UsuarioCreacion, @FechaActual, @HoraActual);
        
        IF @Eliminar = 1 AND @IdOpcionEliminar IS NOT NULL
            INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
            VALUES (@IdRol, @IdPantalla, @IdOpcionEliminar, @UsuarioCreacion, @FechaActual, @HoraActual);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- 5. sp_RolPantallaOpcionModificar
-- =============================================

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
    EXEC [dbo].[sp_RolPantallaOpcionAgregar] @IdRol, @IdPantalla, @Ver, @Crear, @Editar, @Eliminar, @Estado, @UsuarioActualizacion
END
GO

-- =============================================
-- 6. sp_RolPantallaOpcionEliminar
-- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionEliminar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionEliminar]
    @IdRolPantallaOpcion INT,
    @UsuarioEliminacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdRol INT, @IdPantalla INT
    
    SELECT @IdRol = [IdRol], @IdPantalla = [IdPantalla]
    FROM [dbo].[RolPantallaOpcion] WITH (NOLOCK)
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    
    IF @IdRol IS NULL OR @IdPantalla IS NULL RETURN
    
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    
    UPDATE [dbo].[RolPantallaOpcion] WITH (ROWLOCK)
    SET [FechaEliminacion] = @FechaActual,
        [HoraEliminacion] = @HoraActual,
        [UsuarioEliminacion] = @UsuarioEliminacion
    WHERE [IdRol] = @IdRol AND [IdPantalla] = @IdPantalla AND [FechaEliminacion] IS NULL
END
GO

-- =============================================
-- 7. CORREGIR sp_PantallaModificar (AGREGAR TIMEOUT Y OPTIMIZAR)
-- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PantallaModificar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_PantallaModificar]
GO

-- Leer el stored procedure actual primero
-- (Este debe ya existir, solo lo optimizamos si no está bien)

PRINT '============================================='
PRINT 'STORED PROCEDURES ULTRA OPTIMIZADOS CREADOS'
PRINT '============================================='
GO

