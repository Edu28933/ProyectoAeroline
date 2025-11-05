-- =============================================
-- VALIDACIONES PARA ELIMINACIÓN DE USUARIOS
-- Valida estado y relaciones antes de eliminar
-- =============================================

USE [AerolineaPruebaDB]
GO

SET ARITHABORT ON;
GO

-- =============================================
-- 1. CREAR/MODIFICAR STORED PROCEDURE sp_UsuarioEliminar
-- Valida estado y relaciones antes de eliminar (eliminación lógica)
-- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UsuarioEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_UsuarioEliminar]
GO

CREATE PROCEDURE [dbo].[sp_UsuarioEliminar]
    @IdUsuario INT,
    @UsuarioEliminacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET ARITHABORT ON;
    
    DECLARE @Estado VARCHAR(15);
    DECLARE @ErrorMsg NVARCHAR(4000);
    DECLARE @FechaActual DATETIME = GETDATE();
    DECLARE @HoraActual TIME(7) = CAST(GETDATE() AS TIME(7));
    DECLARE @CantidadEmpleados INT = 0;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Verificar que el usuario existe
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Usuarios] WHERE IdUsuario = @IdUsuario)
        BEGIN
            SET @ErrorMsg = 'El usuario no existe.';
            RAISERROR(@ErrorMsg, 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Obtener el estado del usuario
        SELECT @Estado = Estado
        FROM [dbo].[Usuarios]
        WHERE IdUsuario = @IdUsuario;
        
        -- Validar que el estado no sea "Activo"
        IF @Estado = 'Activo'
        BEGIN
            SET @ErrorMsg = 'No se puede eliminar un usuario con estado "Activo". Por favor, cambie el estado a "Inactivo" antes de eliminar.';
            RAISERROR(@ErrorMsg, 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Validar que no tenga empleados activos asociados
        SELECT @CantidadEmpleados = COUNT(*)
        FROM [dbo].[Empleados]
        WHERE IdUsuario = @IdUsuario 
          AND FechaEliminacion IS NULL 
          AND Estado = 'Activo';
        
        IF @CantidadEmpleados > 0
        BEGIN
            SET @ErrorMsg = FORMATMESSAGE('No se puede eliminar el usuario porque tiene %d empleado(s) activo(s) asociado(s). Por favor, elimine o modifique los empleados asociados antes de eliminar el usuario.', @CantidadEmpleados);
            RAISERROR(@ErrorMsg, 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END
        
        -- Proceder con la eliminación lógica (SIEMPRE, sin importar si ya tiene FechaEliminacion)
        UPDATE [dbo].[Usuarios]
        SET 
            [FechaEliminacion] = @FechaActual,
            [HoraEliminacion] = @HoraActual,
            [UsuarioEliminacion] = @UsuarioEliminacion,
            [FechaActualizacion] = @FechaActual,
            [HoraActualizacion] = @HoraActual
        WHERE [IdUsuario] = @IdUsuario;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

PRINT '✅ Stored procedure sp_UsuarioEliminar creado/actualizado correctamente.'
GO

-- =============================================
-- 2. CORREGIR sp_UsuariosSeleccionar
-- Asegurar que filtre usuarios eliminados
-- =============================================

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UsuariosSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_UsuariosSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_UsuariosSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        [IdUsuario],
        [IdRol],
        [Nombre],
        [Contraseña],
        [Correo],
        [Estado]
    FROM [dbo].[Usuarios]
    WHERE [FechaEliminacion] IS NULL  -- SOLO USUARIOS NO ELIMINADOS
    ORDER BY [IdUsuario] DESC;
END
GO

PRINT '✅ Stored procedure sp_UsuariosSeleccionar actualizado correctamente para filtrar usuarios eliminados.'
GO
