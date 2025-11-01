-- =============================================
-- STORED PROCEDURES PARA LA TABLA ROLES
-- Basados en la estructura de sp_UsuariosSeleccionar, sp_UsuarioBuscar, etc.
-- =============================================

USE [AerolineaPruebaDB]
GO

-- =============================================
-- 1. sp_RolesSeleccionar - Listar todos los roles
-- =============================================
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
        ISNULL([Descripcion], '') AS [Descripcion],
        ISNULL([Estado], 'Activo') AS [Estado]
    FROM [dbo].[Roles]
    WHERE ([Estado] != 'Eliminado' OR [Estado] IS NULL)
    ORDER BY [IdRol]
END
GO

-- =============================================
-- 2. sp_RolBuscar - Buscar un rol por ID
-- =============================================
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
        ISNULL([Descripcion], '') AS [Descripcion],
        ISNULL([Estado], 'Activo') AS [Estado]
    FROM [dbo].[Roles]
    WHERE [IdRol] = @IdRol
END
GO

-- =============================================
-- 3. sp_RolAgregar - Agregar un nuevo rol
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolAgregar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolAgregar]
GO

CREATE PROCEDURE [dbo].[sp_RolAgregar]
    @NombreRol NVARCHAR(150),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Estado NVARCHAR(50) = 'Activo'
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[Roles] ([NombreRol], [Descripcion], [Estado])
    VALUES (@NombreRol, @Descripcion, @Estado)
END
GO

-- =============================================
-- 4. sp_RolModificar - Modificar un rol existente
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolModificar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolModificar]
GO

CREATE PROCEDURE [dbo].[sp_RolModificar]
    @IdRol INT,
    @NombreRol NVARCHAR(150),
    @Descripcion NVARCHAR(MAX) = NULL,
    @Estado NVARCHAR(50) = 'Activo'
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[Roles]
    SET 
        [NombreRol] = @NombreRol,
        [Descripcion] = @Descripcion,
        [Estado] = @Estado
    WHERE [IdRol] = @IdRol
END
GO

-- =============================================
-- 5. sp_RolEliminar - Eliminar un rol (lógico o físico)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolEliminar]
GO

CREATE PROCEDURE [dbo].[sp_RolEliminar]
    @IdRol INT
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
        -- Si hay usuarios, solo cambiar el estado a Inactivo
        UPDATE [dbo].[Roles]
        SET [Estado] = 'Inactivo'
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

