-- =============================================
-- STORED PROCEDURES PARA PASAJEROS
-- Actualizado según estructura real de la base de datos
-- =============================================

-- =============================================
-- SP: Seleccionar todos los pasajeros
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SeleccionarPasajeros]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdPasajero,
        Nombres,
        Apellidos,
        Pasaporte,
        Correo,
        Direccion,
        Telefono,
        Pais,
        TipoPasajero,
        Nacionalidad,
        ContactoEmergencia,
        Estado
    FROM Pasajeros
    WHERE FechaEliminacion IS NULL  -- Solo pasajeros no eliminados
    ORDER BY IdPasajero DESC;
END
GO

-- =============================================
-- SP: Buscar pasajero por ID
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_BuscarPasajero]
    @IdPasajero INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdPasajero,
        Nombres,
        Apellidos,
        Pasaporte,
        Correo,
        Direccion,
        Telefono,
        Pais,
        TipoPasajero,
        Nacionalidad,
        ContactoEmergencia,
        Estado
    FROM Pasajeros
    WHERE IdPasajero = @IdPasajero
        AND FechaEliminacion IS NULL;  -- Solo pasajeros no eliminados
END
GO

-- =============================================
-- SP: Agregar pasajero (según estructura real)
-- =============================================
ALTER PROCEDURE [dbo].[sp_AgregarPasajero]
    @Nombres VARCHAR(45),
    @Apellidos VARCHAR(45),
    @Pasaporte VARCHAR(13),
    @Correo VARCHAR(45),
    @Direccion VARCHAR(45),
    @Telefono INT,
    @Pais VARCHAR(45),
    @TipoPasajero VARCHAR(45),
    @Nacionalidad VARCHAR(45),
    @ContactoEmergencia INT,
    @Estado VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Pasajeros (Nombres, Apellidos, Pasaporte, Correo, Direccion, Telefono, Pais, TipoPasajero, Nacionalidad, ContactoEmergencia, Estado)
    VALUES (@Nombres, @Apellidos, UPPER(@Pasaporte), @Correo, @Direccion, @Telefono, @Pais, @TipoPasajero, @Nacionalidad, @ContactoEmergencia, @Estado);
END
GO

-- =============================================
-- SP: Modificar pasajero
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ModificarPasajero]
    @IdPasajero INT,
    @Nombres VARCHAR(45),
    @Apellidos VARCHAR(45),
    @Pasaporte VARCHAR(13),
    @Correo VARCHAR(45),
    @Direccion VARCHAR(45),
    @Telefono INT,
    @Pais VARCHAR(45),
    @TipoPasajero VARCHAR(45),
    @Nacionalidad VARCHAR(45),
    @ContactoEmergencia INT,
    @Estado VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Pasajeros
    SET 
        Nombres = @Nombres,
        Apellidos = @Apellidos,
        Pasaporte = UPPER(@Pasaporte),
        Correo = @Correo,
        Direccion = @Direccion,
        Telefono = @Telefono,
        Pais = @Pais,
        TipoPasajero = @TipoPasajero,
        Nacionalidad = @Nacionalidad,
        ContactoEmergencia = @ContactoEmergencia,
        Estado = @Estado
    WHERE IdPasajero = @IdPasajero;
END
GO

-- =============================================
-- SP: Eliminar pasajero (DELETE físico con validación)
-- Valida que el estado no sea "Activo" antes de eliminar
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_EliminarPasajero]
    @IdPasajero INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Estado VARCHAR(15);
    DECLARE @ErrorMsg VARCHAR(255);
    
    -- Verificar que el pasajero existe y obtener su estado
    SELECT @Estado = Estado
    FROM Pasajeros
    WHERE IdPasajero = @IdPasajero;
    
    -- Validar que el pasajero existe
    IF @Estado IS NULL
    BEGIN
        SET @ErrorMsg = 'El pasajero no existe o ya fue eliminado.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    -- Validar que el estado no sea "Activo"
    IF @Estado = 'Activo'
    BEGIN
        SET @ErrorMsg = 'No se puede eliminar un pasajero con estado "Activo". Por favor, cambie el estado a "Inactivo" antes de eliminar.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    -- Si pasa las validaciones, proceder con la eliminación
    BEGIN TRY
        DELETE FROM Pasajeros
        WHERE IdPasajero = @IdPasajero;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- SP: Listar pasajeros activos (para combos en Boletos)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_ListarPasajerosActivos]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdPasajero,
        Nombres,
        Apellidos,
        TipoPasajero
    FROM Pasajeros
    WHERE Estado = 'Activo' 
    AND FechaEliminacion IS NULL
    ORDER BY Nombres, Apellidos;
END
GO
