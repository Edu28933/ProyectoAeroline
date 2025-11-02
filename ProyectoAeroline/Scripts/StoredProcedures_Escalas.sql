-- =============================================
-- STORED PROCEDURES PARA ESCALAS
-- Con validaciones y mejoras
-- =============================================

-- =============================================
-- SP: Obtener aeropuertos para combo (IdAeropuerto + Nombre)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[usp_ObtenerAeropuertosParaCombo]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdAeropuerto,
        -- Formato: "IdAeropuerto - Nombre"
        CAST(IdAeropuerto AS VARCHAR(10)) + ' - ' + Nombre AS DescripcionAeropuerto,
        Nombre
    FROM Aeropuertos
    WHERE Estado = 'Activo'  -- Solo aeropuertos activos
    ORDER BY Nombre;
END
GO

-- =============================================
-- SP: Seleccionar todas las escalas (con JOIN a Vuelos y Aeropuertos)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_SeleccionarEscalas]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.IdEscala,
        e.IdVuelo,
        e.IdAeropuerto,
        e.HoraLlegada,
        e.HoraSalida,
        e.TiempoEspera,
        e.Estado,
        e.UsuarioCreacion,
        e.FechaCreacion,
        e.HoraCreacion,
        e.UsuarioActualizacion,
        e.FechaActualizacion,
        e.HoraActualizacion,
        e.UsuarioEliminacion,
        e.FechaEliminacion,
        e.HoraEliminacion,
        -- Información del vuelo relacionado
        CASE 
            WHEN v.NumeroVuelo IS NOT NULL AND v.NumeroVuelo != '' 
            THEN CAST(v.IdVuelo AS VARCHAR(10)) + ' - ' + v.NumeroVuelo + ' - ' + v.AeropuertoOrigen + ' → ' + v.AeropuertoDestino
            ELSE CAST(v.IdVuelo AS VARCHAR(10)) + ' - ' + v.AeropuertoOrigen + ' → ' + v.AeropuertoDestino
        END AS DescripcionVuelo,
        -- Información del aeropuerto relacionado (IdAeropuerto - Nombre)
        CAST(a.IdAeropuerto AS VARCHAR(10)) + ' - ' + a.Nombre AS DescripcionAeropuerto
    FROM Escalas e
    INNER JOIN Vuelos v ON e.IdVuelo = v.IdVuelo
    INNER JOIN Aeropuertos a ON e.IdAeropuerto = a.IdAeropuerto
    WHERE e.UsuarioEliminacion IS NULL  -- Solo registros no eliminados
    ORDER BY e.IdEscala DESC;
END
GO

-- =============================================
-- SP: Buscar escala por ID
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_BuscarEscala]
    @IdEscala INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.IdEscala,
        e.IdVuelo,
        e.IdAeropuerto,
        e.HoraLlegada,
        e.HoraSalida,
        e.TiempoEspera,
        e.Estado,
        e.UsuarioCreacion,
        e.FechaCreacion,
        e.HoraCreacion,
        e.UsuarioActualizacion,
        e.FechaActualizacion,
        e.HoraActualizacion,
        e.UsuarioEliminacion,
        e.FechaEliminacion,
        e.HoraEliminacion,
        -- Información del vuelo relacionado
        CASE 
            WHEN v.NumeroVuelo IS NOT NULL AND v.NumeroVuelo != '' 
            THEN CAST(v.IdVuelo AS VARCHAR(10)) + ' - ' + v.NumeroVuelo + ' - ' + v.AeropuertoOrigen + ' → ' + v.AeropuertoDestino
            ELSE CAST(v.IdVuelo AS VARCHAR(10)) + ' - ' + v.AeropuertoOrigen + ' → ' + v.AeropuertoDestino
        END AS DescripcionVuelo,
        -- Información del aeropuerto relacionado (IdAeropuerto - Nombre)
        CAST(a.IdAeropuerto AS VARCHAR(10)) + ' - ' + a.Nombre AS DescripcionAeropuerto
    FROM Escalas e
    INNER JOIN Vuelos v ON e.IdVuelo = v.IdVuelo
    INNER JOIN Aeropuertos a ON e.IdAeropuerto = a.IdAeropuerto
    WHERE e.IdEscala = @IdEscala;
END
GO

-- =============================================
-- SP: Agregar escala
-- Validación: HoraSalida no puede ser mayor que HoraLlegada
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_AgregarEscala]
    @IdVuelo INT,
    @IdAeropuerto INT,
    @HoraLlegada TIME(7),
    @HoraSalida TIME(7),
    @TiempoEspera TIME(7) = NULL,
    @Estado VARCHAR(15) = 'Activo',
    @UsuarioCreacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMsg VARCHAR(255);
    
    -- Validación: HoraLlegada debe ser menor que HoraSalida (llega, espera, sale)
    IF @HoraLlegada >= @HoraSalida
    BEGIN
        SET @ErrorMsg = 'La hora de llegada debe ser menor que la hora de salida.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    -- Validación: TiempoEspera no puede ser negativo
    IF @TiempoEspera IS NOT NULL AND @TiempoEspera < CAST('00:00:00' AS TIME(7))
    BEGIN
        SET @ErrorMsg = 'El tiempo de espera no puede ser negativo.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    BEGIN TRY
        INSERT INTO Escalas (
            IdVuelo,
            IdAeropuerto,
            HoraLlegada,
            HoraSalida,
            TiempoEspera,
            Estado,
            UsuarioCreacion,
            FechaCreacion,
            HoraCreacion
        )
        VALUES (
            @IdVuelo,
            @IdAeropuerto,
            @HoraLlegada,
            @HoraSalida,
            @TiempoEspera,
            @Estado,
            @UsuarioCreacion,
            GETDATE(),
            CAST(GETDATE() AS TIME(7))
        );
        
        SELECT SCOPE_IDENTITY() AS IdEscala;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- SP: Modificar escala
-- Validación: HoraSalida no puede ser mayor que HoraLlegada
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ModificarEscala]
    @IdEscala INT,
    @IdVuelo INT,
    @IdAeropuerto INT,
    @HoraLlegada TIME(7),
    @HoraSalida TIME(7),
    @TiempoEspera TIME(7) = NULL,
    @Estado VARCHAR(15),
    @UsuarioActualizacion VARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorMsg VARCHAR(255);
    
    -- Validación: HoraLlegada debe ser menor que HoraSalida (llega, espera, sale)
    IF @HoraLlegada >= @HoraSalida
    BEGIN
        SET @ErrorMsg = 'La hora de llegada debe ser menor que la hora de salida.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    -- Validación: TiempoEspera no puede ser negativo
    IF @TiempoEspera IS NOT NULL AND @TiempoEspera < CAST('00:00:00' AS TIME(7))
    BEGIN
        SET @ErrorMsg = 'El tiempo de espera no puede ser negativo.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    BEGIN TRY
        UPDATE Escalas
        SET 
            IdVuelo = @IdVuelo,
            IdAeropuerto = @IdAeropuerto,
            HoraLlegada = @HoraLlegada,
            HoraSalida = @HoraSalida,
            TiempoEspera = @TiempoEspera,
            Estado = @Estado,
            UsuarioActualizacion = @UsuarioActualizacion,
            FechaActualizacion = GETDATE(),
            HoraActualizacion = CAST(GETDATE() AS TIME(7))
        WHERE IdEscala = @IdEscala;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

-- =============================================
-- SP: Eliminar escala (DELETE físico con validación)
-- Valida que el estado no sea "Activo" antes de eliminar
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_EliminarEscala]
    @IdEscala INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Estado VARCHAR(15);
    DECLARE @ErrorMsg VARCHAR(255);
    
    -- Verificar que la escala existe y obtener su estado
    SELECT @Estado = Estado
    FROM Escalas
    WHERE IdEscala = @IdEscala;
    
    -- Validar que la escala existe
    IF @Estado IS NULL
    BEGIN
        SET @ErrorMsg = 'La escala no existe o ya fue eliminada.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    -- Validar que el estado no sea "Activo"
    IF @Estado = 'Activo'
    BEGIN
        SET @ErrorMsg = 'No se puede eliminar una escala con estado "Activo". Por favor, cambie el estado a "Inactivo" antes de eliminar.';
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END
    
    -- Si pasa las validaciones, proceder con la eliminación
    BEGIN TRY
        DELETE FROM Escalas
        WHERE IdEscala = @IdEscala;
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END
GO

