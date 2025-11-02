-- =============================================
-- STORED PROCEDURES PARA HORARIOS
-- Con validaciones y mejoras
-- =============================================

-- =============================================
-- SP: Obtener vuelos para combo (Código + Origen + Destino)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_ObtenerVuelosParaCombo]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdVuelo,
        -- Formato: "Código - Origen → Destino"
        -- Si NumeroVuelo es NULL, usar IdVuelo como código
        CASE 
            WHEN NumeroVuelo IS NOT NULL AND NumeroVuelo != '' 
            THEN CAST(IdVuelo AS VARCHAR(10)) + ' - ' + NumeroVuelo + ' - ' + AeropuertoOrigen + ' → ' + AeropuertoDestino
            ELSE CAST(IdVuelo AS VARCHAR(10)) + ' - ' + AeropuertoOrigen + ' → ' + AeropuertoDestino
        END AS DescripcionVuelo,
        NumeroVuelo,
        AeropuertoOrigen,
        AeropuertoDestino
    FROM Vuelos
    WHERE Estado = 'Activo'  -- Solo vuelos activos
    ORDER BY IdVuelo DESC;
END
GO

-- =============================================
-- SP: Seleccionar todos los horarios (con JOIN a Vuelos)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SeleccionarHorarios]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        h.IdHorario,
        h.IdVuelo,
        h.HoraSalida,
        h.HoraLlegada,
        h.TiempoEspera,
        h.Estado,
        h.UsuarioCreacion,
        h.FechaCreacion,
        h.HoraCreacion,
        h.UsuarioActualizacion,
        h.FechaActualizacion,
        h.HoraActualizacion,
        h.UsuarioEliminacion,
        h.FechaEliminacion,
        h.HoraEliminacion,
        -- Información del vuelo relacionado
        CASE 
            WHEN v.NumeroVuelo IS NOT NULL AND v.NumeroVuelo != '' 
            THEN CAST(v.IdVuelo AS VARCHAR(10)) + ' - ' + v.NumeroVuelo + ' - ' + v.AeropuertoOrigen + ' → ' + v.AeropuertoDestino
            ELSE CAST(v.IdVuelo AS VARCHAR(10)) + ' - ' + v.AeropuertoOrigen + ' → ' + v.AeropuertoDestino
        END AS DescripcionVuelo,
        v.NumeroVuelo,
        v.AeropuertoOrigen,
        v.AeropuertoDestino
    FROM Horarios h
    INNER JOIN Vuelos v ON h.IdVuelo = v.IdVuelo
    WHERE h.FechaEliminacion IS NULL  -- Solo horarios no eliminados
    ORDER BY h.IdHorario DESC;
END
GO

-- =============================================
-- SP: Agregar horario (con validación de horas)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_AgregarHorario]
    @IdVuelo INT,
    @HoraSalida TIME(7),
    @HoraLlegada TIME(7),
    @TiempoEspera TIME(7) = NULL,
    @Estado VARCHAR(15) = 'Activo'
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validación: HoraSalida no puede ser mayor que HoraLlegada
    IF @HoraSalida >= @HoraLlegada
    BEGIN
        RAISERROR('La hora de salida no puede ser mayor o igual a la hora de llegada.', 16, 1);
        RETURN;
    END
    
    -- Validar que el vuelo existe y está activo
    IF NOT EXISTS (SELECT 1 FROM Vuelos WHERE IdVuelo = @IdVuelo AND Estado = 'Activo')
    BEGIN
        RAISERROR('El vuelo seleccionado no existe o no está activo.', 16, 1);
        RETURN;
    END
    
    INSERT INTO Horarios 
        (IdVuelo, HoraSalida, HoraLlegada, TiempoEspera, Estado)
    VALUES
        (@IdVuelo, @HoraSalida, @HoraLlegada, @TiempoEspera, @Estado);
    
    SELECT SCOPE_IDENTITY() AS IdHorario;
END
GO

-- =============================================
-- SP: Buscar horario por ID
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_BuscarHorario]
    @IdHorario INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        h.*,
        CASE 
            WHEN v.NumeroVuelo IS NOT NULL AND v.NumeroVuelo != '' 
            THEN CAST(v.IdVuelo AS VARCHAR(10)) + ' - ' + v.NumeroVuelo + ' - ' + v.AeropuertoOrigen + ' → ' + v.AeropuertoDestino
            ELSE CAST(v.IdVuelo AS VARCHAR(10)) + ' - ' + v.AeropuertoOrigen + ' → ' + v.AeropuertoDestino
        END AS DescripcionVuelo
    FROM Horarios h
    INNER JOIN Vuelos v ON h.IdVuelo = v.IdVuelo
    WHERE h.IdHorario = @IdHorario;
END
GO

-- =============================================
-- SP: Modificar horario (con validación de horas)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ModificarHorario]
    @IdHorario INT,
    @IdVuelo INT,
    @HoraSalida TIME(7),
    @HoraLlegada TIME(7),
    @TiempoEspera TIME(7) = NULL,
    @Estado VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validación: HoraSalida no puede ser mayor que HoraLlegada
    IF @HoraSalida >= @HoraLlegada
    BEGIN
        RAISERROR('La hora de salida no puede ser mayor o igual a la hora de llegada.', 16, 1);
        RETURN;
    END
    
    -- Validar que el vuelo existe y está activo
    IF NOT EXISTS (SELECT 1 FROM Vuelos WHERE IdVuelo = @IdVuelo AND Estado = 'Activo')
    BEGIN
        RAISERROR('El vuelo seleccionado no existe o no está activo.', 16, 1);
        RETURN;
    END
    
    UPDATE Horarios
    SET 
        IdVuelo = @IdVuelo,
        HoraSalida = @HoraSalida,
        HoraLlegada = @HoraLlegada,
        TiempoEspera = @TiempoEspera,
        Estado = @Estado
    WHERE IdHorario = @IdHorario;
END
GO

-- =============================================
-- SP: Eliminar horario
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_EliminarHorario]
    @IdHorario INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM Horarios
    WHERE IdHorario = @IdHorario;
END
GO

