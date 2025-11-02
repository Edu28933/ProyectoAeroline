-- =============================================
-- STORED PROCEDURES PARA BOLETOS
-- Incluye FechaCompra en todas las operaciones
-- =============================================

-- =============================================
-- 1. sp_BoletoAgregar - Agregar nuevo boleto
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_BoletoAgregar]
    @IdVuelo INT,
    @IdPasajero INT,
    @NumeroAsiento VARCHAR(255) = NULL,
    @Clase VARCHAR(45) = NULL,
    @Precio DECIMAL(10, 2),
    @Cantidad INT = NULL,
    @Descuento DECIMAL(10, 2) = NULL,
    @Impuesto DECIMAL(10, 2) = NULL,
    @Total DECIMAL(10, 2),
    @Reembolso VARCHAR(50) = NULL,
    @FechaCompra DATETIME,
    @Estado VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ErrorMsg VARCHAR(255);

    -- Validaciones básicas
    IF @IdVuelo IS NULL
    BEGIN
        SET @ErrorMsg = 'El IdVuelo es requerido.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    IF @IdPasajero IS NULL
    BEGIN
        SET @ErrorMsg = 'El IdPasajero es requerido.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    IF @Precio IS NULL OR @Precio < 0
    BEGIN
        SET @ErrorMsg = 'El Precio es requerido y debe ser mayor o igual a cero.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    IF @Total IS NULL OR @Total < 0
    BEGIN
        SET @ErrorMsg = 'El Total es requerido y debe ser mayor o igual a cero.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    IF @FechaCompra IS NULL
    BEGIN
        SET @FechaCompra = GETDATE();
    END

    IF @Estado IS NULL OR LEN(@Estado) = 0
    BEGIN
        SET @Estado = 'Pendiente';
    END

    BEGIN TRY
        INSERT INTO Boletos
            (IdVuelo, IdPasajero, NumeroAsiento, Clase, Precio, Cantidad, Descuento, 
             Impuesto, Total, Reembolso, FechaCompra, Estado,
             UsuarioCreacion, FechaCreacion, HoraCreacion)
        VALUES
            (@IdVuelo, @IdPasajero, @NumeroAsiento, @Clase, @Precio, @Cantidad, @Descuento,
             @Impuesto, @Total, @Reembolso, @FechaCompra, @Estado,
             SYSTEM_USER, GETDATE(), CAST(GETDATE() AS TIME(7)));
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- 2. sp_BoletoModificar - Modificar boleto existente
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_BoletoModificar]
    @IdBoleto INT,
    @IdVuelo INT,
    @IdPasajero INT,
    @NumeroAsiento VARCHAR(255) = NULL,
    @Clase VARCHAR(45) = NULL,
    @Precio DECIMAL(10, 2),
    @Cantidad INT = NULL,
    @Descuento DECIMAL(10, 2) = NULL,
    @Impuesto DECIMAL(10, 2) = NULL,
    @Total DECIMAL(10, 2),
    @Reembolso VARCHAR(50) = NULL,
    @FechaCompra DATETIME,
    @Estado VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ErrorMsg VARCHAR(255);

    -- Validaciones básicas
    IF @IdBoleto IS NULL
    BEGIN
        SET @ErrorMsg = 'El IdBoleto es requerido.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    IF @IdVuelo IS NULL
    BEGIN
        SET @ErrorMsg = 'El IdVuelo es requerido.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    IF @IdPasajero IS NULL
    BEGIN
        SET @ErrorMsg = 'El IdPasajero es requerido.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    IF @Precio IS NULL OR @Precio < 0
    BEGIN
        SET @ErrorMsg = 'El Precio es requerido y debe ser mayor o igual a cero.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    IF @Total IS NULL OR @Total < 0
    BEGIN
        SET @ErrorMsg = 'El Total es requerido y debe ser mayor o igual a cero.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    IF @FechaCompra IS NULL
    BEGIN
        -- Mantener la fecha de compra original si no se proporciona una nueva
        SELECT @FechaCompra = FechaCompra
        FROM Boletos
        WHERE IdBoleto = @IdBoleto;
    END

    IF @Estado IS NULL OR LEN(@Estado) = 0
    BEGIN
        -- Mantener el estado actual si no se proporciona uno nuevo
        SELECT @Estado = Estado
        FROM Boletos
        WHERE IdBoleto = @IdBoleto;
    END

    BEGIN TRY
        UPDATE Boletos
        SET
            IdVuelo = @IdVuelo,
            IdPasajero = @IdPasajero,
            NumeroAsiento = @NumeroAsiento,
            Clase = @Clase,
            Precio = @Precio,
            Cantidad = @Cantidad,
            Descuento = @Descuento,
            Impuesto = @Impuesto,
            Total = @Total,
            Reembolso = @Reembolso,
            FechaCompra = @FechaCompra,
            Estado = @Estado,
            UsuarioActualizacion = SYSTEM_USER,
            FechaActualizacion = GETDATE(),
            HoraActualizacion = CAST(GETDATE() AS TIME(7))
        WHERE IdBoleto = @IdBoleto;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage2 NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage2, 16, 1);
    END CATCH
END
GO

-- =============================================
-- 3. sp_BoletosSeleccionar - Listar todos los boletos
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_BoletosSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdBoleto,
        IdVuelo,
        IdPasajero,
        NumeroAsiento,
        Clase,
        Precio,
        Cantidad,
        Descuento,
        Impuesto,
        Total,
        Reembolso,
        FechaCompra,
        Estado
    FROM Boletos
    WHERE FechaEliminacion IS NULL
    ORDER BY IdBoleto DESC;
END
GO

-- =============================================
-- 4. sp_BoletoBuscar - Buscar boleto por ID
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_BoletoBuscar]
    @IdBoleto INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdBoleto,
        IdVuelo,
        IdPasajero,
        NumeroAsiento,
        Clase,
        Precio,
        Cantidad,
        Descuento,
        Impuesto,
        Total,
        Reembolso,
        FechaCompra,
        Estado
    FROM Boletos
    WHERE IdBoleto = @IdBoleto;
END
GO

-- =============================================
-- 5. sp_BoletoEliminar - Eliminar boleto (eliminación lógica)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_BoletoEliminar]
    @IdBoleto INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ErrorMsg VARCHAR(255);

    -- Verificar que el boleto existe
    IF NOT EXISTS (SELECT 1 FROM Boletos WHERE IdBoleto = @IdBoleto)
    BEGIN
        SET @ErrorMsg = 'El boleto no existe.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END

    BEGIN TRY
        -- Eliminación lógica
        UPDATE Boletos
        SET
            FechaEliminacion = GETDATE(),
            HoraEliminacion = CAST(GETDATE() AS TIME(7)),
            UsuarioEliminacion = SYSTEM_USER
        WHERE IdBoleto = @IdBoleto;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage3 NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage3, 16, 1);
    END CATCH
END
GO

