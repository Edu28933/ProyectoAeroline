-- =============================================
-- SCRIPT PARA CORREGIR STORED PROCEDURES DE PANTALLAS Y PERMISOS
-- Este script corrige todos los stored procedures necesarios
-- =============================================

USE [AerolineaPruebaDB]
GO

-- =============================================
-- 1. CORREGIR sp_PantallaEliminar (ELIMINACIÓN LÓGICA)
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
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Obtener valores de fecha y hora una vez
        DECLARE @FechaActual DATETIME = GETDATE();
        DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
        
        -- Primero eliminar relaciones en RolPantallaOpcion (eliminación lógica)
        UPDATE [dbo].[RolPantallaOpcion] WITH (ROWLOCK)
        SET 
            [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioEliminacion,
            [FechaActualizacion] = @FechaActual,
            [HoraActualizacion] = @HoraActual
        WHERE [IdPantalla] = @IdPantalla
          AND [FechaEliminacion] IS NULL;
        
        -- Ahora eliminar la pantalla (eliminación lógica)
        UPDATE [dbo].[Pantallas] WITH (ROWLOCK)
        SET 
            [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioEliminacion,
            [FechaActualizacion] = @FechaActual,
            [HoraActualizacion] = @HoraActual
        WHERE [IdPantalla] = @IdPantalla;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- =============================================
-- 2. CREAR/ACTUALIZAR STORED PROCEDURES DE ROLPANTALLAOPCION
-- =============================================

-- 2.1. sp_RolPantallaOpcionesSeleccionar
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionesSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    -- Usar CTE para mejorar rendimiento
    WITH PermisosAgrupados AS (
        SELECT 
            RPO.[IdRol],
            RPO.[IdPantalla],
            MIN(RPO.[IdRolPantallaOpcion]) AS [IdRolPantallaOpcion],
            MAX(CASE WHEN O.[NombreOpcion] = 'Ver' THEN 1 ELSE 0 END) AS [Ver],
            MAX(CASE WHEN O.[NombreOpcion] = 'Crear' THEN 1 ELSE 0 END) AS [Crear],
            MAX(CASE WHEN O.[NombreOpcion] = 'Editar' THEN 1 ELSE 0 END) AS [Editar],
            MAX(CASE WHEN O.[NombreOpcion] = 'Eliminar' THEN 1 ELSE 0 END) AS [Eliminar],
            MAX(RPO.[FechaEliminacion]) AS [FechaEliminacion]
        FROM [dbo].[RolPantallaOpcion] RPO WITH (NOLOCK)
        INNER JOIN [dbo].[Opciones] O WITH (NOLOCK) ON RPO.[IdOpcion] = O.[IdOpcion]
        WHERE RPO.[FechaEliminacion] IS NULL
        GROUP BY RPO.[IdRol], RPO.[IdPantalla]
    )
    SELECT 
        PA.[IdRolPantallaOpcion],
        PA.[IdRol],
        PA.[IdPantalla],
        PA.[Ver],
        PA.[Crear],
        PA.[Editar],
        PA.[Eliminar],
        CASE 
            WHEN PA.[FechaEliminacion] IS NULL THEN 'Activo'
            ELSE 'Inactivo'
        END AS [Estado],
        ISNULL(R.[NombreRol], '') AS [NombreRol],
        ISNULL(P.[NombrePantalla], '') AS [NombrePantalla]
    FROM PermisosAgrupados PA
    LEFT JOIN [dbo].[Roles] R WITH (NOLOCK) ON PA.[IdRol] = R.[IdRol]
    LEFT JOIN [dbo].[Pantallas] P WITH (NOLOCK) ON PA.[IdPantalla] = P.[IdPantalla]
    ORDER BY PA.[IdRolPantallaOpcion]
END
GO

-- 2.2. sp_RolPantallaOpcionBuscar
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
    FROM [dbo].[RolPantallaOpcion]
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    
    IF @IdRol IS NULL OR @IdPantalla IS NULL
    BEGIN
        RETURN
    END
    
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

-- 2.3. sp_RolPantallaOpcionAgregar
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
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Obtener valores de fecha y hora una vez
        DECLARE @FechaActual DATETIME = GETDATE();
        DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
        
        -- Eliminar permisos existentes para esta combinación de Rol y Pantalla (eliminación lógica)
        UPDATE [dbo].[RolPantallaOpcion] WITH (ROWLOCK)
        SET 
            [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioCreacion
        WHERE [IdRol] = @IdRol 
          AND [IdPantalla] = @IdPantalla
          AND [FechaEliminacion] IS NULL;
        
        -- Obtener IDs de las opciones en una sola consulta
        DECLARE @IdOpcionVer INT, @IdOpcionCrear INT, @IdOpcionEditar INT, @IdOpcionEliminar INT
        
        SELECT 
            @IdOpcionVer = MAX(CASE WHEN [NombreOpcion] = 'Ver' THEN [IdOpcion] END),
            @IdOpcionCrear = MAX(CASE WHEN [NombreOpcion] = 'Crear' THEN [IdOpcion] END),
            @IdOpcionEditar = MAX(CASE WHEN [NombreOpcion] = 'Editar' THEN [IdOpcion] END),
            @IdOpcionEliminar = MAX(CASE WHEN [NombreOpcion] = 'Eliminar' THEN [IdOpcion] END)
        FROM [dbo].[Opciones] WITH (NOLOCK)
        WHERE [NombreOpcion] IN ('Ver', 'Crear', 'Editar', 'Eliminar');
        
        -- Insertar las opciones seleccionadas usando INSERT con múltiples valores
        INSERT INTO [dbo].[RolPantallaOpcion] ([IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
        SELECT @IdRol, @IdPantalla, [IdOpcion], @UsuarioCreacion, @FechaActual, @HoraActual
        FROM (VALUES 
            (@IdOpcionVer, @Ver),
            (@IdOpcionCrear, @Crear),
            (@IdOpcionEditar, @Editar),
            (@IdOpcionEliminar, @Eliminar)
        ) AS Opciones([IdOpcion], [Habilitado])
        WHERE [Habilitado] = 1 AND [IdOpcion] IS NOT NULL;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- 2.4. sp_RolPantallaOpcionModificar
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
    
    -- Usar el mismo procedimiento que Agregar (elimina y vuelve a insertar)
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

-- 2.5. sp_RolPantallaOpcionEliminar
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
    
    SELECT @IdRol = [IdRol], @IdPantalla = [IdPantalla]
    FROM [dbo].[RolPantallaOpcion] WITH (NOLOCK)
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
    
    IF @IdRol IS NULL OR @IdPantalla IS NULL
    BEGIN
        RETURN
    END
    
    -- Obtener valores de fecha y hora una vez
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    
    -- Eliminación lógica de todos los permisos para esta combinación de Rol y Pantalla
    UPDATE [dbo].[RolPantallaOpcion] WITH (ROWLOCK)
    SET 
        [FechaEliminacion] = @FechaActual,
        [HoraEliminacion] = @HoraActual,
        [UsuarioEliminacion] = @UsuarioEliminacion,
        [FechaActualizacion] = @FechaActual,
        [HoraActualizacion] = @HoraActual
    WHERE [IdRol] = @IdRol 
      AND [IdPantalla] = @IdPantalla
      AND [FechaEliminacion] IS NULL
END
GO

PRINT '============================================='
PRINT 'STORED PROCEDURES CORREGIDOS EXITOSAMENTE'
PRINT '============================================='
PRINT '- sp_PantallaEliminar (eliminación lógica)'
PRINT '- sp_RolPantallaOpcionesSeleccionar'
PRINT '- sp_RolPantallaOpcionBuscar'
PRINT '- sp_RolPantallaOpcionAgregar'
PRINT '- sp_RolPantallaOpcionModificar'
PRINT '- sp_RolPantallaOpcionEliminar (eliminación lógica)'
PRINT '============================================='
GO

