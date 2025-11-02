-- =============================================
-- STORED PROCEDURES PARA MANTENIMIENTOS
-- Con actualización automática del estado del avión
-- =============================================

-- 1. sp_MantenimientoSeleccionar - Seleccionar todos los mantenimientos
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_MantenimientoSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_MantenimientoSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_MantenimientoSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdMantenimiento,
        IdAvion,
        IdEmpleado,
        FechaIngreso,
        FechaSalida,
        Tipo,
        Costo,
        CostoExtra,
        Descripcion,
        Estado
    FROM Mantenimientos
    WHERE FechaEliminacion IS NULL
    ORDER BY IdMantenimiento DESC;
END
GO

-- 2. sp_MantenimientoBuscar - Buscar un mantenimiento por ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_MantenimientoBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_MantenimientoBuscar]
GO

CREATE PROCEDURE [dbo].[sp_MantenimientoBuscar]
    @IdMantenimiento INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdMantenimiento,
        IdAvion,
        IdEmpleado,
        FechaIngreso,
        FechaSalida,
        Tipo,
        Costo,
        CostoExtra,
        Descripcion,
        Estado
    FROM Mantenimientos
    WHERE IdMantenimiento = @IdMantenimiento
        AND FechaEliminacion IS NULL;
END
GO

-- 3. sp_MantenimientoAgregar - Agregar un nuevo mantenimiento
-- ACTUALIZA AUTOMÁTICAMENTE EL ESTADO DEL AVIÓN A "Mantenimiento"
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_MantenimientoAgregar]
    @IdAvion INT,
    @IdEmpleado INT = NULL,
    @FechaIngreso DATETIME,
    @FechaSalida DATETIME = NULL,
    @Tipo VARCHAR(45) = NULL,
    @Costo DECIMAL(10, 2),
    @CostoExtra DECIMAL(10, 2) = 0,
    @Descripcion VARCHAR(255) = NULL,
    @Estado VARCHAR(45) = 'Pendiente'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMsg VARCHAR(255);
    
    -- Validaciones básicas
    IF @IdAvion IS NULL
    BEGIN
        SET @ErrorMsg = 'El IdAvion es requerido.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    IF @FechaIngreso IS NULL
    BEGIN
        SET @ErrorMsg = 'La FechaIngreso es requerida.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    IF @Costo IS NULL
    BEGIN
        SET @Costo = 0;
    END
    
    IF @CostoExtra IS NULL
    BEGIN
        SET @CostoExtra = 0;
    END
    
    IF @Estado IS NULL OR LEN(@Estado) = 0
    BEGIN
        SET @Estado = 'Pendiente';
    END
    
    BEGIN TRY
        -- Insertar el mantenimiento
        INSERT INTO Mantenimientos 
            (IdAvion, IdEmpleado, FechaIngreso, FechaSalida, Tipo, Costo, CostoExtra, Descripcion, Estado,
             UsuarioCreacion, FechaCreacion, HoraCreacion)
        VALUES 
            (@IdAvion, @IdEmpleado, @FechaIngreso, @FechaSalida, @Tipo, @Costo, @CostoExtra, @Descripcion, @Estado,
             SYSTEM_USER, GETDATE(), CAST(GETDATE() AS TIME(7)));
        
        -- Actualizar el estado del avión a "Mantenimiento"
        UPDATE Aviones
        SET Estado = 'Mantenimiento',
            UsuarioActualizacion = SYSTEM_USER,
            FechaActualizacion = GETDATE(),
            HoraActualizacion = CAST(GETDATE() AS TIME(7))
        WHERE IdAvion = @IdAvion;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- 4. sp_MantenimientoModificar - Modificar un mantenimiento existente
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_MantenimientoModificar]
    @IdMantenimiento INT,
    @IdAvion INT,
    @IdEmpleado INT = NULL,
    @FechaIngreso DATETIME,
    @FechaSalida DATETIME = NULL,
    @Tipo VARCHAR(45) = NULL,
    @Costo DECIMAL(10, 2),
    @CostoExtra DECIMAL(10, 2) = 0,
    @Descripcion VARCHAR(255) = NULL,
    @Estado VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMsg VARCHAR(255);
    
    -- Validaciones básicas
    IF @IdMantenimiento IS NULL
    BEGIN
        SET @ErrorMsg = 'El IdMantenimiento es requerido.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    IF @IdAvion IS NULL
    BEGIN
        SET @ErrorMsg = 'El IdAvion es requerido.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    IF @FechaIngreso IS NULL
    BEGIN
        SET @ErrorMsg = 'La FechaIngreso es requerida.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    IF @Costo IS NULL
    BEGIN
        SET @Costo = 0;
    END
    
    IF @CostoExtra IS NULL
    BEGIN
        SET @CostoExtra = 0;
    END
    
    IF @Estado IS NULL OR LEN(@Estado) = 0
    BEGIN
        -- Mantener el estado actual si no se proporciona uno nuevo
        SELECT @Estado = Estado
        FROM Mantenimientos
        WHERE IdMantenimiento = @IdMantenimiento;
    END
    
    -- Obtener el IdAvion original antes de actualizar (por si cambió el avión)
    DECLARE @IdAvionAnterior INT;
    SELECT @IdAvionAnterior = IdAvion
    FROM Mantenimientos
    WHERE IdMantenimiento = @IdMantenimiento;
    
    BEGIN TRY
        -- Actualizar el mantenimiento
        UPDATE Mantenimientos
        SET 
            IdAvion = @IdAvion,
            IdEmpleado = @IdEmpleado,
            FechaIngreso = @FechaIngreso,
            FechaSalida = @FechaSalida,
            Tipo = @Tipo,
            Costo = @Costo,
            CostoExtra = @CostoExtra,
            Descripcion = @Descripcion,
            Estado = @Estado,
            UsuarioActualizacion = SYSTEM_USER,
            FechaActualizacion = GETDATE(),
            HoraActualizacion = CAST(GETDATE() AS TIME(7))
        WHERE IdMantenimiento = @IdMantenimiento;
        
        -- Si cambió el avión, manejar el avión anterior primero
        IF @IdAvionAnterior IS NOT NULL AND @IdAvionAnterior != @IdAvion
        BEGIN
            -- Verificar si el avión anterior tiene otros mantenimientos pendientes o en proceso
            DECLARE @MantenimientosAvionAnterior INT;
            SELECT @MantenimientosAvionAnterior = COUNT(*)
            FROM Mantenimientos
            WHERE IdAvion = @IdAvionAnterior
                AND Estado IN ('Pendiente', 'En proceso')
                AND FechaEliminacion IS NULL;
            
            -- Si no hay mantenimientos pendientes en el avión anterior, cambiar a Activo
            IF @MantenimientosAvionAnterior = 0
            BEGIN
                UPDATE Aviones
                SET Estado = 'Activo',
                    UsuarioActualizacion = SYSTEM_USER,
                    FechaActualizacion = GETDATE(),
                    HoraActualizacion = CAST(GETDATE() AS TIME(7))
                WHERE IdAvion = @IdAvionAnterior;
            END
        END
        
        -- Manejar el estado del avión actual según el estado del mantenimiento
        IF @Estado = 'Finalizado'
        BEGIN
            -- Si el mantenimiento está Finalizado, verificar si hay otros mantenimientos pendientes
            DECLARE @MantenimientosPendientes INT;
            SELECT @MantenimientosPendientes = COUNT(*)
            FROM Mantenimientos
            WHERE IdAvion = @IdAvion
                AND Estado IN ('Pendiente', 'En proceso')
                AND IdMantenimiento != @IdMantenimiento
                AND FechaEliminacion IS NULL;
            
            -- Si no hay mantenimientos pendientes o en proceso, cambiar el avión a Activo
            IF @MantenimientosPendientes = 0
            BEGIN
                UPDATE Aviones
                SET Estado = 'Activo',
                    UsuarioActualizacion = SYSTEM_USER,
                    FechaActualizacion = GETDATE(),
                    HoraActualizacion = CAST(GETDATE() AS TIME(7))
                WHERE IdAvion = @IdAvion;
            END
        END
        ELSE
        BEGIN
            -- Si el mantenimiento está Pendiente o En proceso, el avión debe estar en Mantenimiento
            UPDATE Aviones
            SET Estado = 'Mantenimiento',
                UsuarioActualizacion = SYSTEM_USER,
                FechaActualizacion = GETDATE(),
                HoraActualizacion = CAST(GETDATE() AS TIME(7))
            WHERE IdAvion = @IdAvion;
        END
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage2 NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage2, 16, 1);
    END CATCH
END
GO

-- 5. sp_MantenimientoEliminar - Eliminar un mantenimiento (lógico)
-- ACTUALIZA AUTOMÁTICAMENTE EL ESTADO DEL AVIÓN SI NO HAY MÁS MANTENIMIENTOS PENDIENTES
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_MantenimientoEliminar]
    @IdMantenimiento INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMsg VARCHAR(255);
    
    -- Obtener el IdAvion antes de eliminar
    DECLARE @IdAvion INT;
    SELECT @IdAvion = IdAvion
    FROM Mantenimientos
    WHERE IdMantenimiento = @IdMantenimiento;
    
    IF @IdAvion IS NULL
    BEGIN
        SET @ErrorMsg = 'El mantenimiento no existe.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    BEGIN TRY
        -- Eliminación lógica del mantenimiento
        UPDATE Mantenimientos
        SET 
            FechaEliminacion = GETDATE(),
            HoraEliminacion = CAST(GETDATE() AS TIME(7)),
            UsuarioEliminacion = SYSTEM_USER
        WHERE IdMantenimiento = @IdMantenimiento;
        
        -- Verificar si el avión tiene otros mantenimientos pendientes o en proceso
        DECLARE @MantenimientosPendientes INT;
        SELECT @MantenimientosPendientes = COUNT(*)
        FROM Mantenimientos
        WHERE IdAvion = @IdAvion
            AND Estado IN ('Pendiente', 'En proceso')
            AND IdMantenimiento != @IdMantenimiento
            AND FechaEliminacion IS NULL;
        
        -- Si no hay mantenimientos pendientes, cambiar el avión a Activo
        IF @MantenimientosPendientes = 0
        BEGIN
            UPDATE Aviones
            SET Estado = 'Activo',
                UsuarioActualizacion = SYSTEM_USER,
                FechaActualizacion = GETDATE(),
                HoraActualizacion = CAST(GETDATE() AS TIME(7))
            WHERE IdAvion = @IdAvion;
        END
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage3 NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage3, 16, 1);
    END CATCH
END
GO

-- 6. usp_ListarAvionesActivos - Listar aviones activos para combo box
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ListarAvionesActivos]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[usp_ListarAvionesActivos]
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_ListarAvionesActivos]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdAvion,
        Placa
    FROM Aviones
    WHERE Estado IN ('Activo', 'Mantenimiento')
        AND FechaEliminacion IS NULL
    ORDER BY Placa;
END
GO

-- 7. usp_ListarEmpleadosActivos - Listar empleados activos para combo box
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ListarEmpleadosActivos]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[usp_ListarEmpleadosActivos]
GO

CREATE PROCEDURE [dbo].[usp_ListarEmpleadosActivos]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdEmpleado,
        Nombre
    FROM Empleados
    WHERE Estado = 'Activo'
        AND FechaEliminacion IS NULL
    ORDER BY Nombre;
END
GO

