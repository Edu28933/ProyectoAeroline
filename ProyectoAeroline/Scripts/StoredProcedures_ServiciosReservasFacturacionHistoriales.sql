-- =============================================
-- STORED PROCEDURES PARA: Servicios, Reservas, Facturación e Historiales
-- Basados en la estructura real de la base de datos AerolineaPruebaDB
-- Fecha: 2025-11-01
-- =============================================

USE [AerolineaPruebaDB]
GO

-- =============================================
-- ==========  SERVICIOS  ==========
-- =============================================

-- 1. sp_ServiciosSeleccionar - Listar todos los servicios
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ServiciosSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ServiciosSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_ServiciosSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Servicios;
END
GO

-- 2. sp_ServicioBuscar - Buscar un servicio por ID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ServicioBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ServicioBuscar]
GO

CREATE PROCEDURE [dbo].[sp_ServicioBuscar]
    @IdServicio INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Servicios
    WHERE IdServicio = @IdServicio;
END
GO

-- 3. sp_ServicioAgregar - Agregar un nuevo servicio
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ServicioAgregar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ServicioAgregar]
GO

CREATE PROCEDURE [dbo].[sp_ServicioAgregar]
    @IdBoleto INT,
    @Fecha DATE = NULL,
    @TipoServicio VARCHAR(45),
    @Costo DECIMAL(10,2),
    @Cantidad INT = NULL,
    @CostoTotal DECIMAL(10,2) = NULL,
    @Estado VARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calcular CostoTotal si no se proporciona y hay cantidad
    DECLARE @CalculadoCostoTotal DECIMAL(10,2) = @CostoTotal;
    IF @CalculadoCostoTotal IS NULL AND @Cantidad IS NOT NULL
    BEGIN
        SET @CalculadoCostoTotal = @Costo * @Cantidad;
    END
    
    INSERT INTO Servicios (IdBoleto, Fecha, TipoServicio, Costo, Cantidad, CostoTotal, Estado)
    VALUES (@IdBoleto, @Fecha, @TipoServicio, @Costo, @Cantidad, @CalculadoCostoTotal, @Estado);
END
GO

-- 4. sp_ServicioModificar - Modificar un servicio existente
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ServicioModificar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ServicioModificar]
GO

CREATE PROCEDURE [dbo].[sp_ServicioModificar]
    @IdServicio INT,
    @IdBoleto INT,
    @Fecha DATE = NULL,
    @TipoServicio VARCHAR(45),
    @Costo DECIMAL(10,2),
    @Cantidad INT = NULL,
    @CostoTotal DECIMAL(10,2) = NULL,
    @Estado VARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calcular CostoTotal si no se proporciona y hay cantidad
    DECLARE @CalculadoCostoTotal DECIMAL(10,2) = @CostoTotal;
    IF @CalculadoCostoTotal IS NULL AND @Cantidad IS NOT NULL
    BEGIN
        SET @CalculadoCostoTotal = @Costo * @Cantidad;
    END
    
    UPDATE Servicios
    SET IdBoleto = @IdBoleto,
        Fecha = @Fecha,
        TipoServicio = @TipoServicio,
        Costo = @Costo,
        Cantidad = @Cantidad,
        CostoTotal = @CalculadoCostoTotal,
        Estado = @Estado
    WHERE IdServicio = @IdServicio;
END
GO

-- 5. sp_ServicioEliminar - Eliminar un servicio
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ServicioEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ServicioEliminar]
GO

CREATE PROCEDURE [dbo].[sp_ServicioEliminar]
    @IdServicio INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Servicios
    WHERE IdServicio = @IdServicio;
END
GO

-- =============================================
-- ==========  RESERVAS  ==========
-- =============================================

-- 1. sp_ReservasSeleccionar - Listar todas las reservas
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ReservasSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ReservasSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_ReservasSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Reservas;
END
GO

-- 2. sp_ReservaBuscar - Buscar una reserva por ID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ReservaBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ReservaBuscar]
GO

CREATE PROCEDURE [dbo].[sp_ReservaBuscar]
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Reservas
    WHERE IdReserva = @IdReserva;
END
GO

-- 3. sp_ReservaAgregar - Agregar una nueva reserva
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ReservaAgregar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ReservaAgregar]
GO

CREATE PROCEDURE [dbo].[sp_ReservaAgregar]
    @IdPasajero INT,
    @IdVuelo INT,
    @FechaReserva DATE,
    @MontoAnticipo DECIMAL(10,2) = NULL,
    @FechaVuelo DATE = NULL,
    @Observaciones VARCHAR(45) = NULL,
    @Estado VARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Reservas (IdPasajero, IdVuelo, FechaReserva, MontoAnticipo, FechaVuelo, Observaciones, Estado)
    VALUES (@IdPasajero, @IdVuelo, @FechaReserva, @MontoAnticipo, @FechaVuelo, @Observaciones, @Estado);
END
GO

-- 4. sp_ReservaModificar - Modificar una reserva existente
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ReservaModificar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ReservaModificar]
GO

CREATE PROCEDURE [dbo].[sp_ReservaModificar]
    @IdReserva INT,
    @IdPasajero INT,
    @IdVuelo INT,
    @FechaReserva DATE,
    @MontoAnticipo DECIMAL(10,2) = NULL,
    @FechaVuelo DATE = NULL,
    @Observaciones VARCHAR(45) = NULL,
    @Estado VARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Reservas
    SET IdPasajero = @IdPasajero,
        IdVuelo = @IdVuelo,
        FechaReserva = @FechaReserva,
        MontoAnticipo = @MontoAnticipo,
        FechaVuelo = @FechaVuelo,
        Observaciones = @Observaciones,
        Estado = @Estado
    WHERE IdReserva = @IdReserva;
END
GO

-- 5. sp_ReservaEliminar - Eliminar una reserva
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ReservaEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_ReservaEliminar]
GO

CREATE PROCEDURE [dbo].[sp_ReservaEliminar]
    @IdReserva INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Reservas
    WHERE IdReserva = @IdReserva;
END
GO

-- =============================================
-- ==========  FACTURACIÓN  ==========
-- =============================================

-- 1. sp_FacturacionSeleccionar - Listar todas las facturas
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_FacturacionSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_FacturacionSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_FacturacionSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Facturacion;
END
GO

-- 2. sp_FacturacionBuscar - Buscar una factura por ID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_FacturacionBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_FacturacionBuscar]
GO

CREATE PROCEDURE [dbo].[sp_FacturacionBuscar]
    @IdFactura INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Facturacion
    WHERE IdFactura = @IdFactura;
END
GO

-- 3. sp_FacturacionAgregar - Agregar una nueva factura
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_FacturacionAgregar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_FacturacionAgregar]
GO

CREATE PROCEDURE [dbo].[sp_FacturacionAgregar]
    @IdBoleto INT,
    @FechaEmision DATE,
    @HoraEmision TIME(7) = NULL,
    @Descripcion VARCHAR(45) = NULL,
    @TipoPago VARCHAR(45) = NULL,
    @Moneda VARCHAR(45) = NULL,
    @Monto DECIMAL(10,2) = NULL,
    @Impuesto DECIMAL(10,2) = NULL,
    @MontoFactura DECIMAL(10,2),
    @MontoTotal DECIMAL(10,2) = NULL,
    @NumeroAutorizacion INT = NULL,
    @Estado VARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calcular MontoTotal si no se proporciona
    DECLARE @CalculadoMontoTotal DECIMAL(10,2) = @MontoTotal;
    IF @CalculadoMontoTotal IS NULL
    BEGIN
        SET @CalculadoMontoTotal = ISNULL(@MontoFactura, 0) + ISNULL(@Impuesto, 0);
    END
    
    -- Si no se proporciona hora, usar la hora actual
    DECLARE @HoraActual TIME(7) = @HoraEmision;
    IF @HoraActual IS NULL
    BEGIN
        SET @HoraActual = CAST(GETDATE() AS TIME(7));
    END
    
    INSERT INTO Facturacion (IdBoleto, FechaEmision, HoraEmision, Descripcion, TipoPago, Moneda, Monto, Impuesto, MontoFactura, MontoTotal, NumeroAutorizacion, Estado)
    VALUES (@IdBoleto, @FechaEmision, @HoraActual, @Descripcion, @TipoPago, @Moneda, @Monto, @Impuesto, @MontoFactura, @CalculadoMontoTotal, @NumeroAutorizacion, @Estado);
END
GO

-- 4. sp_FacturacionModificar - Modificar una factura existente
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_FacturacionModificar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_FacturacionModificar]
GO

CREATE PROCEDURE [dbo].[sp_FacturacionModificar]
    @IdFactura INT,
    @IdBoleto INT,
    @FechaEmision DATE,
    @HoraEmision TIME(7) = NULL,
    @Descripcion VARCHAR(45) = NULL,
    @TipoPago VARCHAR(45) = NULL,
    @Moneda VARCHAR(45) = NULL,
    @Monto DECIMAL(10,2) = NULL,
    @Impuesto DECIMAL(10,2) = NULL,
    @MontoFactura DECIMAL(10,2),
    @MontoTotal DECIMAL(10,2) = NULL,
    @NumeroAutorizacion INT = NULL,
    @Estado VARCHAR(15) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calcular MontoTotal si no se proporciona
    DECLARE @CalculadoMontoTotal DECIMAL(10,2) = @MontoTotal;
    IF @CalculadoMontoTotal IS NULL
    BEGIN
        SET @CalculadoMontoTotal = ISNULL(@MontoFactura, 0) + ISNULL(@Impuesto, 0);
    END
    
    UPDATE Facturacion
    SET IdBoleto = @IdBoleto,
        FechaEmision = @FechaEmision,
        HoraEmision = @HoraEmision,
        Descripcion = @Descripcion,
        TipoPago = @TipoPago,
        Moneda = @Moneda,
        Monto = @Monto,
        Impuesto = @Impuesto,
        MontoFactura = @MontoFactura,
        MontoTotal = @CalculadoMontoTotal,
        NumeroAutorizacion = @NumeroAutorizacion,
        Estado = @Estado
    WHERE IdFactura = @IdFactura;
END
GO

-- 5. sp_FacturacionEliminar - Eliminar una factura
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_FacturacionEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_FacturacionEliminar]
GO

CREATE PROCEDURE [dbo].[sp_FacturacionEliminar]
    @IdFactura INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Facturacion
    WHERE IdFactura = @IdFactura;
END
GO

-- =============================================
-- ==========  HISTORIALES  ==========
-- =============================================

-- 1. sp_HistorialesSeleccionar - Listar todos los historiales
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_HistorialesSeleccionar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_HistorialesSeleccionar]
GO

CREATE PROCEDURE [dbo].[sp_HistorialesSeleccionar]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Historiales;
END
GO

-- 2. sp_HistorialBuscar - Buscar un historial por ID
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_HistorialBuscar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_HistorialBuscar]
GO

CREATE PROCEDURE [dbo].[sp_HistorialBuscar]
    @IdHistorial INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Historiales
    WHERE IdHistorial = @IdHistorial;
END
GO

-- 3. sp_HistorialAgregar - Agregar un nuevo historial
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_HistorialAgregar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_HistorialAgregar]
GO

CREATE PROCEDURE [dbo].[sp_HistorialAgregar]
    @IdBoleto INT,
    @IdPasajero INT,
    @IdAerolinea INT,
    @IdVuelo INT,
    @Observacion VARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Historiales (IdBoleto, IdPasajero, IdAerolinea, IdVuelo, Observacion)
    VALUES (@IdBoleto, @IdPasajero, @IdAerolinea, @IdVuelo, @Observacion);
END
GO

-- 4. sp_HistorialModificar - Modificar un historial existente
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_HistorialModificar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_HistorialModificar]
GO

CREATE PROCEDURE [dbo].[sp_HistorialModificar]
    @IdHistorial INT,
    @IdBoleto INT,
    @IdPasajero INT,
    @IdAerolinea INT,
    @IdVuelo INT,
    @Observacion VARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Historiales
    SET IdBoleto = @IdBoleto,
        IdPasajero = @IdPasajero,
        IdAerolinea = @IdAerolinea,
        IdVuelo = @IdVuelo,
        Observacion = @Observacion
    WHERE IdHistorial = @IdHistorial;
END
GO

-- 5. sp_HistorialEliminar - Eliminar un historial
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_HistorialEliminar]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_HistorialEliminar]
GO

CREATE PROCEDURE [dbo].[sp_HistorialEliminar]
    @IdHistorial INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Historiales
    WHERE IdHistorial = @IdHistorial;
END
GO

