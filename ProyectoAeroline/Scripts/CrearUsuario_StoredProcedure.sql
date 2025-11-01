-- =============================================
-- STORED PROCEDURE: usp_Usuarios_CrearBasico
-- Descripción: Crea un nuevo usuario con contraseña hasheada
-- =============================================

USE [AerolineaPruebaDB]
GO

-- Asegurar que la columna Contraseña sea suficientemente grande
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Usuarios' AND COLUMN_NAME = 'Contraseña'
           AND CHARACTER_MAXIMUM_LENGTH < 500)
BEGIN
    PRINT 'Actualizando tamaño de columna Contraseña...'
    ALTER TABLE [dbo].[Usuarios]
    ALTER COLUMN [Contraseña] NVARCHAR(MAX) NULL
    PRINT 'Columna Contraseña actualizada a NVARCHAR(MAX)'
END
GO

-- Crear o reemplazar el stored procedure
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_Usuarios_CrearBasico]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[usp_Usuarios_CrearBasico]
GO

CREATE PROCEDURE [dbo].[usp_Usuarios_CrearBasico]
    @Nombre NVARCHAR(150),
    @Correo NVARCHAR(150),
    @IdRol INT = 2,
    @Estado NVARCHAR(50) = 'Activo',
    @Contraseña NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdUsuario INT;
    
    -- Si no se proporciona contraseña, usar 'EXTERNAL_LOGIN' para usuarios de Google
    IF @Contraseña IS NULL OR @Contraseña = ''
        SET @Contraseña = 'EXTERNAL_LOGIN';
    
    -- Verificar si ya existe un usuario con ese correo
    IF EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Correo] = @Correo)
    BEGIN
        -- Si ya existe, retornar su ID (pero no actualizar nada)
        SELECT @IdUsuario = [IdUsuario] FROM [dbo].[Usuarios] WHERE [Correo] = @Correo;
        SELECT @IdUsuario AS IdUsuario;
        RETURN;
    END
    
    -- Insertar el nuevo usuario con la contraseña proporcionada (que debe venir hasheada desde C#)
    INSERT INTO [dbo].[Usuarios] ([IdRol], [Nombre], [Contraseña], [Estado], [Correo], [FechaCreacion])
    VALUES (@IdRol, @Nombre, @Contraseña, @Estado, @Correo, GETDATE());
    
    SET @IdUsuario = SCOPE_IDENTITY();
    
    -- Retornar el ID del usuario creado
    SELECT @IdUsuario AS IdUsuario;
END
GO

PRINT 'Stored procedure usp_Usuarios_CrearBasico creado correctamente'
GO

