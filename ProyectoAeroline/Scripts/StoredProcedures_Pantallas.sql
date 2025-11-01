-- =============================================
-- STORED PROCEDURES PARA LA TABLA PANTALLAS
-- Basados en la estructura de sp_UsuariosSeleccionar, sp_UsuarioBuscar, etc.
-- =============================================

USE [AerolineaPruebaDB]
GO

-- =============================================
-- 1. sp_PantallasSeleccionar - Listar todas las pantallas
-- =============================================
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
        ISNULL([Ruta], '') AS [Ruta],
        ISNULL([Icono], '') AS [Icono],
        ISNULL([Descripcion], '') AS [Descripcion],
        ISNULL([Estado], 'Activo') AS [Estado]
    FROM [dbo].[Pantallas]
    WHERE ([Estado] != 'Eliminado' OR [Estado] IS NULL)
    ORDER BY [IdPantalla]
END
GO

-- =============================================
-- 2. sp_PantallaBuscar - Buscar una pantalla por ID
-- =============================================
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
        ISNULL([Ruta], '') AS [Ruta],
        ISNULL([Icono], '') AS [Icono],
        ISNULL([Descripcion], '') AS [Descripcion],
        ISNULL([Estado], 'Activo') AS [Estado]
    FROM [dbo].[Pantallas]
    WHERE [IdPantalla] = @IdPantalla
END
GO

-- =============================================
-- 3. sp_PantallaAgregar - Agregar una nueva pantalla
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PantallaAgregar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_PantallaAgregar]
GO

CREATE PROCEDURE [dbo].[sp_PantallaAgregar]
    @NombrePantalla NVARCHAR(150),
    @Ruta NVARCHAR(500) = NULL,
    @Icono NVARCHAR(100) = NULL,
    @Descripcion NVARCHAR(MAX) = NULL,
    @Estado NVARCHAR(50) = 'Activo'
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Pantallas] ([NombrePantalla], [Ruta], [Icono], [Descripcion], [Estado])
    VALUES (@NombrePantalla, @Ruta, @Icono, @Descripcion, @Estado)
END
GO

-- =============================================
-- 4. sp_PantallaModificar - Modificar una pantalla existente
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PantallaModificar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_PantallaModificar]
GO

CREATE PROCEDURE [dbo].[sp_PantallaModificar]
    @IdPantalla INT,
    @NombrePantalla NVARCHAR(150),
    @Ruta NVARCHAR(500) = NULL,
    @Icono NVARCHAR(100) = NULL,
    @Descripcion NVARCHAR(MAX) = NULL,
    @Estado NVARCHAR(50) = 'Activo'
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Pantallas]
    SET 
        [NombrePantalla] = @NombrePantalla,
        [Ruta] = @Ruta,
        [Icono] = @Icono,
        [Descripcion] = @Descripcion,
        [Estado] = @Estado
    WHERE [IdPantalla] = @IdPantalla
END
GO

-- =============================================
-- 5. sp_PantallaEliminar - Eliminar una pantalla (lógico o físico)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PantallaEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_PantallaEliminar]
GO

CREATE PROCEDURE [dbo].[sp_PantallaEliminar]
    @IdPantalla INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @RolesPantallasCount INT;
    
    -- Primero eliminar relaciones en RolesPantallas si existen
    DELETE FROM [dbo].[RolesPantallas]
    WHERE [IdPantalla] = @IdPantalla;
    
    -- También eliminar relaciones en RolPantallaOpcion
    DELETE FROM [dbo].[RolPantallaOpcion]
    WHERE [IdPantalla] = @IdPantalla;
    
    -- Ahora eliminar la pantalla físicamente
    DELETE FROM [dbo].[Pantallas]
    WHERE [IdPantalla] = @IdPantalla;
END
GO

