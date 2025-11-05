-- =============================================
-- FIX FINAL - ARREGLAR TODOS LOS TIMEOUTS
-- Optimiza sp_RolEliminar, sp_PantallaModificar y todos los SP críticos
-- =============================================

USE [AerolineaPruebaDB]
GO

SET ARITHABORT ON;
GO

-- =============================================
-- 1. OPTIMIZAR sp_RolEliminar (ELIMINACIÓN LÓGICA RÁPIDA CON TRANSACCIÓN)
-- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolEliminar]
GO

CREATE PROCEDURE [dbo].[sp_RolEliminar]
    @IdRol INT,
    @UsuarioEliminacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Primero eliminar lógicamente los permisos relacionados
        UPDATE [dbo].[RolPantallaOpcion] WITH (ROWLOCK)
        SET [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioEliminacion
        WHERE [IdRol] = @IdRol AND [FechaEliminacion] IS NULL;
        
        -- Ahora eliminar el rol lógicamente
        UPDATE [dbo].[Roles] WITH (ROWLOCK)
        SET [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioEliminacion
        WHERE [IdRol] = @IdRol AND [FechaEliminacion] IS NULL;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- 2. OPTIMIZAR sp_PantallaModificar (CON CACHE Y ROWLOCK)
-- =============================================

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
    SET ARITHABORT ON;
    
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        UPDATE [dbo].[Pantallas] WITH (ROWLOCK)
        SET 
            [NombrePantalla] = @NombrePantalla,
            [UsuarioActualizacion] = @UsuarioActualizacion,
            [FechaActualizacion] = @FechaActual,
            [HoraActualizacion] = @HoraActual
        WHERE [IdPantalla] = @IdPantalla AND [FechaEliminacion] IS NULL;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- 3. OPTIMIZAR sp_PantallaEliminar (MEJORA FINAL)
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
    SET ARITHABORT ON;
    
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Eliminar lógicamente los permisos relacionados
        UPDATE [dbo].[RolPantallaOpcion] WITH (ROWLOCK)
        SET [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioEliminacion
        WHERE [IdPantalla] = @IdPantalla AND [FechaEliminacion] IS NULL;
        
        -- Eliminar lógicamente la pantalla
        UPDATE [dbo].[Pantallas] WITH (ROWLOCK)
        SET [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioEliminacion
        WHERE [IdPantalla] = @IdPantalla AND [FechaEliminacion] IS NULL;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- 4. OPTIMIZAR sp_RolPantallaOpcionesSeleccionar (VERSIÓN ULTRA SIMPLE)
-- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionesSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
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
-- 5. OPTIMIZAR sp_RolPantallaOpcionAgregar (VERSIÓN MEJORADA)
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
    SET ARITHABORT ON;
    
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Eliminar permisos existentes lógicamente
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
-- 6. OPTIMIZAR sp_RolPantallaOpcionEliminar (MEJORA FINAL)
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
    SET ARITHABORT ON;
    
    DECLARE @IdRol INT, @IdPantalla INT
    
    -- Obtener IdRol e IdPantalla de forma rápida
    SELECT TOP 1 @IdRol = [IdRol], @IdPantalla = [IdPantalla]
    FROM [dbo].[RolPantallaOpcion] WITH (NOLOCK)
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    
    IF @IdRol IS NULL OR @IdPantalla IS NULL RETURN
    
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Eliminar lógicamente todos los permisos para este rol y pantalla
        UPDATE [dbo].[RolPantallaOpcion] WITH (ROWLOCK)
        SET [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioEliminacion
        WHERE [IdRol] = @IdRol AND [IdPantalla] = @IdPantalla AND [FechaEliminacion] IS NULL
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- 7. OPTIMIZAR sp_RolModificar (MEJORA FINAL)
-- =============================================

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
    SET ARITHABORT ON;
    
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        UPDATE [dbo].[Roles] WITH (ROWLOCK)
        SET 
            [NombreRol] = @NombreRol,
            [UsuarioActualizacion] = @UsuarioActualizacion,
            [FechaActualizacion] = @FechaActual,
            [HoraActualizacion] = @HoraActual
        WHERE [IdRol] = @IdRol AND [FechaEliminacion] IS NULL;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

PRINT '============================================='
PRINT 'STORED PROCEDURES OPTIMIZADOS EXITOSAMENTE'
PRINT '============================================='
PRINT 'sp_RolEliminar - Optimizado con transacciones'
PRINT 'sp_PantallaModificar - Optimizado con ROWLOCK'
PRINT 'sp_PantallaEliminar - Optimizado con transacciones'
PRINT 'sp_RolPantallaOpcionesSeleccionar - Simplificado'
PRINT 'sp_RolPantallaOpcionAgregar - Optimizado'
PRINT 'sp_RolPantallaOpcionEliminar - Optimizado'
PRINT 'sp_RolModificar - Optimizado'
PRINT '============================================='
GO

