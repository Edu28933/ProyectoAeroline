-- =============================================
-- Script: Stored Procedures para Google Login
-- Descripción: Procedimientos para manejar la confirmación de login con Google
-- Fecha: 2025-01-11
-- =============================================

USE [AerolineaPruebaDB]
GO

-- =============================================
-- 1. PROCEDURE: usp_GoogleLogin_CrearTokenConfirmacion
-- Descripción: Crea un token temporal para confirmar el login con Google
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GoogleLogin_CrearTokenConfirmacion]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[usp_GoogleLogin_CrearTokenConfirmacion]
GO

CREATE PROCEDURE [dbo].[usp_GoogleLogin_CrearTokenConfirmacion]
    @ProviderKey NVARCHAR(450),
    @Email NVARCHAR(256),
    @DisplayName NVARCHAR(256),
    @MinutosValidez INT = 60,
    @IpSolicitud NVARCHAR(45) = NULL,
    @UserAgent NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Token NVARCHAR(256);
    DECLARE @FechaExpiracion DATETIME2;
    
    -- Generar token único (usando GUID y hash)
    SET @Token = CONVERT(NVARCHAR(256), NEWID()) + '_' + CONVERT(NVARCHAR(256), HASHBYTES('SHA2_256', @ProviderKey + @Email + CONVERT(NVARCHAR(50), GETDATE())));
    SET @FechaExpiracion = DATEADD(MINUTE, @MinutosValidez, GETDATE());
    
    -- Crear o verificar si existe tabla temporal para tokens de Google
    -- Si no existe, se crea automáticamente con la estructura necesaria
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GoogleLoginTokens')
    BEGIN
        CREATE TABLE [dbo].[GoogleLoginTokens] (
            [Id] INT IDENTITY(1,1) PRIMARY KEY,
            [Token] NVARCHAR(256) NOT NULL,
            [ProviderKey] NVARCHAR(450) NOT NULL,
            [Email] NVARCHAR(256) NOT NULL,
            [DisplayName] NVARCHAR(256) NULL,
            [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
            [FechaExpiracion] DATETIME2 NOT NULL,
            [IpSolicitud] NVARCHAR(45) NULL,
            [UserAgent] NVARCHAR(500) NULL,
            [Usado] BIT NOT NULL DEFAULT 0
        );
        
        -- Crear índices después de la tabla
        CREATE UNIQUE NONCLUSTERED INDEX [IX_GoogleLoginTokens_Token] 
        ON [dbo].[GoogleLoginTokens] ([Token]);
        
        CREATE NONCLUSTERED INDEX [IX_GoogleLoginTokens_ProviderKey] 
        ON [dbo].[GoogleLoginTokens] ([ProviderKey]);
        
        CREATE NONCLUSTERED INDEX [IX_GoogleLoginTokens_Email] 
        ON [dbo].[GoogleLoginTokens] ([Email]);
    END
    
    -- Insertar el token
    INSERT INTO [dbo].[GoogleLoginTokens] 
        ([Token], [ProviderKey], [Email], [DisplayName], [FechaCreacion], [FechaExpiracion], [IpSolicitud], [UserAgent], [Usado])
    VALUES 
        (@Token, @ProviderKey, @Email, @DisplayName, GETDATE(), @FechaExpiracion, @IpSolicitud, @UserAgent, 0);
    
    -- Limpiar tokens expirados (más de 24 horas)
    DELETE FROM [dbo].[GoogleLoginTokens] 
    WHERE [FechaExpiracion] < DATEADD(HOUR, -24, GETDATE());
    
    -- Retornar el token
    SELECT @Token AS Token;
END
GO

-- =============================================
-- 2. PROCEDURE: usp_GoogleLogin_VerificarToken
-- Descripción: Verifica si un token es válido sin consumirlo
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GoogleLogin_VerificarToken]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[usp_GoogleLogin_VerificarToken]
GO

CREATE PROCEDURE [dbo].[usp_GoogleLogin_VerificarToken]
    @Token NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Valido BIT = 0;
    DECLARE @FechaExpiracion DATETIME2;
    DECLARE @Usado BIT;
    
    SELECT 
        @FechaExpiracion = [FechaExpiracion],
        @Usado = [Usado]
    FROM [dbo].[GoogleLoginTokens]
    WHERE [Token] = @Token;
    
    -- Token es válido si existe, no ha expirado y no ha sido usado
    IF @FechaExpiracion IS NOT NULL 
       AND @FechaExpiracion > GETDATE() 
       AND @Usado = 0
    BEGIN
        SET @Valido = 1;
    END
    
    SELECT @Valido AS Valido;
END
GO

-- =============================================
-- 3. PROCEDURE: usp_GoogleLogin_Confirmar
-- Descripción: Confirma el token, crea el usuario y lo vincula con Google
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GoogleLogin_Confirmar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[usp_GoogleLogin_Confirmar]
GO

CREATE PROCEDURE [dbo].[usp_GoogleLogin_Confirmar]
    @Token NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ProviderKey NVARCHAR(450);
    DECLARE @Email NVARCHAR(256);
    DECLARE @DisplayName NVARCHAR(256);
    DECLARE @FechaExpiracion DATETIME2;
    DECLARE @Usado BIT;
    DECLARE @IdUsuario INT;
    DECLARE @IdRol INT = (SELECT TOP 1 [IdRol] FROM [dbo].[Roles] WHERE [NombreRol] = 'Usuario' AND [FechaEliminacion] IS NULL ORDER BY [IdRol]);
    IF @IdRol IS NULL SET @IdRol = 5; -- Fallback si no existe el rol Usuario
    DECLARE @Ok BIT = 0;
    DECLARE @NombreFinal NVARCHAR(256);
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Obtener datos del token
        SELECT 
            @ProviderKey = [ProviderKey],
            @Email = [Email],
            @DisplayName = [DisplayName],
            @FechaExpiracion = [FechaExpiracion],
            @Usado = [Usado]
        FROM [dbo].[GoogleLoginTokens]
        WHERE [Token] = @Token;
        
        -- Validar token
        IF @ProviderKey IS NULL OR @FechaExpiracion IS NULL OR @FechaExpiracion < GETDATE() OR @Usado = 1
        BEGIN
            SELECT @Ok AS Ok, NULL AS IdUsuario, NULL AS IdRol, NULL AS Nombre, NULL AS Correo, NULL AS Estado, NULL AS NombreRol;
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Marcar token como usado
        UPDATE [dbo].[GoogleLoginTokens]
        SET [Usado] = 1
        WHERE [Token] = @Token;
        
        -- Verificar si el usuario ya existe por UsuariosExternalLogins
        SELECT @IdUsuario = UEL.[IdUsuario], @IdRol = U.[IdRol], @NombreFinal = U.[Nombre]
        FROM [dbo].[UsuariosExternalLogins] UEL
        INNER JOIN [dbo].[Usuarios] U ON UEL.[IdUsuario] = U.[IdUsuario]
        WHERE UEL.[Proveedor] = 'Google' AND UEL.[ProviderKey] = @ProviderKey;
        
        -- Si no existe por ExternalLogins, buscar por correo
        IF @IdUsuario IS NULL
        BEGIN
            SELECT @IdUsuario = [IdUsuario], @IdRol = [IdRol], @NombreFinal = [Nombre]
            FROM [dbo].[Usuarios]
            WHERE [Correo] = @Email;
        END
        
        -- Si el usuario no existe, crearlo
        IF @IdUsuario IS NULL
        BEGIN
            SET @NombreFinal = ISNULL(@DisplayName, LEFT(@Email, CHARINDEX('@', @Email) - 1));
            
            -- Verificar si ya existe por correo antes de crear
            IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE [Correo] = @Email)
            BEGIN
                -- Insertar directamente para evitar resultsets intermedios
                INSERT INTO [dbo].[Usuarios]([IdRol], [Nombre], [Contraseña], [Estado], [Correo], [FechaCreacion])
                VALUES(@IdRol, @NombreFinal, 'EXTERNAL_LOGIN', 'Activo', @Email, GETDATE());
                
                SET @IdUsuario = SCOPE_IDENTITY();
            END
            ELSE
            BEGIN
                -- Ya existe por correo, obtener su ID
                SELECT @IdUsuario = [IdUsuario], @IdRol = [IdRol], @NombreFinal = [Nombre]
                FROM [dbo].[Usuarios]
                WHERE [Correo] = @Email;
            END
        END
        
        -- Vincular Google con el usuario (si no está ya vinculado)
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[UsuariosExternalLogins] 
            WHERE [IdUsuario] = @IdUsuario 
            AND [Proveedor] = 'Google' 
            AND [ProviderKey] = @ProviderKey
        )
        BEGIN
            -- Insertar directamente para evitar resultsets intermedios
            INSERT INTO [dbo].[UsuariosExternalLogins]
                ([IdUsuario], [Proveedor], [ProviderKey], [EmailProveedor], [DisplayName], [AvatarUrl], [FechaVinculo], [UltimoAccesoUtc])
            VALUES
                (@IdUsuario, 'Google', @ProviderKey, @Email, @DisplayName, NULL, SYSUTCDATETIME(), SYSUTCDATETIME());
        END
        
        -- Obtener datos finales del usuario
        SELECT 
            @IdRol = [IdRol],
            @NombreFinal = [Nombre],
            @Email = [Correo]
        FROM [dbo].[Usuarios]
        WHERE [IdUsuario] = @IdUsuario;
        
        SET @Ok = 1;
        
        -- Retornar datos del usuario
        SELECT 
            @Ok AS Ok,
            @IdUsuario AS IdUsuario,
            @IdRol AS IdRol,
            @NombreFinal AS Nombre,
            @Email AS Correo,
            'Activo' AS Estado,
            (SELECT TOP 1 [NombreRol] FROM [dbo].[Roles] WHERE [IdRol] = @IdRol) AS NombreRol;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SELECT 
            0 AS Ok,
            NULL AS IdUsuario,
            NULL AS IdRol,
            NULL AS Nombre,
            NULL AS Correo,
            NULL AS Estado,
            NULL AS NombreRol;
    END CATCH
END
GO

-- =============================================
-- NOTAS IMPORTANTES:
-- =============================================
-- 1. Este script crea automáticamente la tabla [GoogleLoginTokens] si no existe
-- 2. Asegúrate de que existan las siguientes tablas:
--    - [Usuarios] (con columnas: IdUsuario, IdRol, Nombre, Correo, Estado)
--    - [Roles] (con columna: Nombre)
--    - [ExternalLogins] (opcional, para vincular cuentas de Google)
--
-- 3. Si tu estructura de tablas es diferente, ajusta los nombres de columnas en los SP
--
-- 4. El SP usp_Usuarios_CrearBasico es opcional. Si no existe, se hace INSERT directo
--
-- 5. Ejecuta este script en tu base de datos SQL Server antes de usar el login con Google
-- =============================================

