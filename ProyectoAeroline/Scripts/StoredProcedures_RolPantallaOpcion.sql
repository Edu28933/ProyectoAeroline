-- =============================================
-- STORED PROCEDURES PARA LA TABLA RolPantallaOpcion
-- Basados en la estructura de sp_UsuariosSeleccionar, sp_UsuarioBuscar, etc.
-- =============================================

USE [AerolineaPruebaDB]
GO

-- =============================================
-- 1. sp_RolPantallaOpcionesSeleccionar - Listar todos los permisos con información relacionada
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionesSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionesSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        RPO.[IdRolPantallaOpcion],
        RPO.[IdRol],
        RPO.[IdPantalla],
        RPO.[Ver],
        RPO.[Crear],
        RPO.[Editar],
        RPO.[Eliminar],
        ISNULL(RPO.[Estado], 'Activo') AS [Estado],
        ISNULL(R.[NombreRol], '') AS [NombreRol],
        ISNULL(P.[NombrePantalla], '') AS [NombrePantalla]
    FROM [dbo].[RolPantallaOpcion] RPO
    LEFT JOIN [dbo].[Roles] R ON RPO.[IdRol] = R.[IdRol]
    LEFT JOIN [dbo].[Pantallas] P ON RPO.[IdPantalla] = P.[IdPantalla]
    WHERE (RPO.[Estado] != 'Eliminado' OR RPO.[Estado] IS NULL)
    ORDER BY RPO.[IdRolPantallaOpcion]
END
GO

-- =============================================
-- 2. sp_RolPantallaOpcionBuscar - Buscar un permiso por ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionBuscar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionBuscar]
    @IdRolPantallaOpcion INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        RPO.[IdRolPantallaOpcion],
        RPO.[IdRol],
        RPO.[IdPantalla],
        RPO.[Ver],
        RPO.[Crear],
        RPO.[Editar],
        RPO.[Eliminar],
        ISNULL(RPO.[Estado], 'Activo') AS [Estado],
        ISNULL(R.[NombreRol], '') AS [NombreRol],
        ISNULL(P.[NombrePantalla], '') AS [NombrePantalla]
    FROM [dbo].[RolPantallaOpcion] RPO
    LEFT JOIN [dbo].[Roles] R ON RPO.[IdRol] = R.[IdRol]
    LEFT JOIN [dbo].[Pantallas] P ON RPO.[IdPantalla] = P.[IdPantalla]
    WHERE RPO.[IdRolPantallaOpcion] = @IdRolPantallaOpcion
END
GO

-- =============================================
-- 3. sp_RolPantallaOpcionAgregar - Agregar un nuevo permiso
-- =============================================
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
    @Estado NVARCHAR(50) = 'Activo'
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[RolPantallaOpcion] 
        ([IdRol], [IdPantalla], [Ver], [Crear], [Editar], [Eliminar], [Estado])
    VALUES 
        (@IdRol, @IdPantalla, @Ver, @Crear, @Editar, @Eliminar, @Estado)
END
GO

-- =============================================
-- 4. sp_RolPantallaOpcionModificar - Modificar un permiso existente
-- =============================================
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
    @Estado NVARCHAR(50) = 'Activo'
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[RolPantallaOpcion]
    SET 
        [IdRol] = @IdRol,
        [IdPantalla] = @IdPantalla,
        [Ver] = @Ver,
        [Crear] = @Crear,
        [Editar] = @Editar,
        [Eliminar] = @Eliminar,
        [Estado] = @Estado
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
END
GO

-- =============================================
-- 5. sp_RolPantallaOpcionEliminar - Eliminar un permiso
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RolPantallaOpcionEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_RolPantallaOpcionEliminar]
GO

CREATE PROCEDURE [dbo].[sp_RolPantallaOpcionEliminar]
    @IdRolPantallaOpcion INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [dbo].[RolPantallaOpcion]
    WHERE [IdRolPantallaOpcion] = @IdRolPantallaOpcion
END
GO

