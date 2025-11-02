-- =============================================
-- SCRIPT DE DATOS DE PRUEBA
-- Inserta 3 registros en cada tabla de la base de datos
-- Respetando las relaciones y foreign keys
-- Fecha: 2025-11-01
-- =============================================

USE [AerolineaPruebaDB]
GO

SET IDENTITY_INSERT [dbo].[Roles] OFF
GO

-- =============================================
-- 1. ROLES (Ya existen, pero verificamos)
-- =============================================
-- Los roles ya existen según el script inicial, pero verificamos
IF NOT EXISTS (SELECT 1 FROM Roles WHERE NombreRol = 'SuperAdmin')
BEGIN
    INSERT INTO [dbo].[Roles] ([NombreRol], [UsuarioCreacion], [FechaCreacion], [HoraCreacion], [Estado])
    VALUES 
        ('SuperAdmin', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)), 'Activo'),
        ('AdminOperaciones', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)), 'Activo'),
        ('AgenteVentas', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)), 'Activo');
END
GO

-- =============================================
-- 2. USUARIOS (Depende de Roles)
-- =============================================
DECLARE @IdRolSuperAdmin INT = (SELECT TOP 1 IdRol FROM Roles WHERE NombreRol = 'SuperAdmin');
DECLARE @IdRolAdmin INT = (SELECT TOP 1 IdRol FROM Roles WHERE NombreRol = 'AdminOperaciones');
DECLARE @IdRolAgente INT = (SELECT TOP 1 IdRol FROM Roles WHERE NombreRol = 'AgenteVentas');

SET IDENTITY_INSERT [dbo].[Usuarios] ON
GO

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Correo = 'usuario1@aerolinea.com')
BEGIN
    INSERT INTO [dbo].[Usuarios] ([IdUsuario], [IdRol], [Nombre], [Contraseña], [Correo], [Estado], [FechaCreacion])
    VALUES 
        (100, @IdRolSuperAdmin, 'Usuario Prueba 1', '12345', 'usuario1@aerolinea.com', 'Activo', GETDATE()),
        (101, @IdRolAdmin, 'Usuario Prueba 2', '12345', 'usuario2@aerolinea.com', 'Activo', GETDATE()),
        (102, @IdRolAgente, 'Usuario Prueba 3', '12345', 'usuario3@aerolinea.com', 'Activo', GETDATE());
END
GO

SET IDENTITY_INSERT [dbo].[Usuarios] OFF
GO

-- =============================================
-- 3. EMPLEADOS (Depende de Usuarios)
-- =============================================
SET IDENTITY_INSERT [dbo].[Empleados] ON
GO

IF NOT EXISTS (SELECT 1 FROM Empleados WHERE Correo = 'empleado1@aerolinea.com')
BEGIN
    INSERT INTO [dbo].[Empleados] ([IdEmpleado], [IdUsuario], [Nombre], [Cargo], [Licencia], [Telefono], [Correo], [Salario], [Direccion], [FechaIngreso], [ContactoEmergencia], [Estado], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, 100, 'Carlos Méndez', 'Piloto', 'LIC-001', 5551001, 'empleado1@aerolinea.com', 15000.00, 'Zona 10, Guatemala', GETDATE(), 5551002, 'Activo', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, 101, 'Ana García', 'Azafata', 'LIC-002', 5551003, 'empleado2@aerolinea.com', 8000.00, 'Zona 11, Guatemala', GETDATE(), 5551004, 'Activo', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, 102, 'Luis Ramírez', 'Mecánico', 'LIC-003', 5551005, 'empleado3@aerolinea.com', 9000.00, 'Zona 12, Guatemala', GETDATE(), 5551006, 'Activo', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Empleados] OFF
GO

-- =============================================
-- 4. AEROLÍNEAS (Depende de Empleados - opcional)
-- =============================================
SET IDENTITY_INSERT [dbo].[Aerolineas] ON
GO

IF NOT EXISTS (SELECT 1 FROM Aerolineas WHERE Nombre = 'Aerolínea de Prueba 1')
BEGIN
    INSERT INTO [dbo].[Aerolineas] ([IdAerolinea], [IdEmpleado], [IATA], [Nombre], [Pais], [Ciudad], [Direccion], [Telefono], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, 100, 'AP1', 'Aerolínea de Prueba 1', 'Guatemala', 'Ciudad de Guatemala', 'Avenida Principal 123', 22221001, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, 101, 'AP2', 'Aerolínea de Prueba 2', 'El Salvador', 'San Salvador', 'Boulevard Los Héroes 456', 22221002, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, 102, 'AP3', 'Aerolínea de Prueba 3', 'Honduras', 'Tegucigalpa', 'Colonia Centro 789', 22221003, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Aerolineas] OFF
GO

-- =============================================
-- 5. AEROPUERTOS (Depende de Empleados)
-- =============================================
SET IDENTITY_INSERT [dbo].[Aeropuertos] ON
GO

IF NOT EXISTS (SELECT 1 FROM Aeropuertos WHERE Nombre = 'Aeropuerto Internacional La Aurora - Prueba')
BEGIN
    INSERT INTO [dbo].[Aeropuertos] ([IdAeropuerto], [IdEmpleado], [IATA], [Nombre], [Pais], [Ciudad], [Direccion], [Telefono], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, 100, 'GUA', 'Aeropuerto Internacional La Aurora - Prueba', 'Guatemala', 'Ciudad de Guatemala', 'Zona 13, Guatemala', 24220001, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, 101, 'SAL', 'Aeropuerto Internacional de El Salvador - Prueba', 'El Salvador', 'San Salvador', 'San Luis Talpa, La Paz', 24220002, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, 102, 'TGU', 'Aeropuerto Internacional Toncontín - Prueba', 'Honduras', 'Tegucigalpa', 'Barrio El Centro', 24220003, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Aeropuertos] OFF
GO

-- =============================================
-- 6. AVIONES (Depende de Aerolíneas)
-- =============================================
DECLARE @IdAerolinea1 INT = (SELECT TOP 1 IdAerolinea FROM Aerolineas WHERE IATA = 'AP1');
DECLARE @IdAerolinea2 INT = (SELECT TOP 1 IdAerolinea FROM Aerolineas WHERE IATA = 'AP2');
DECLARE @IdAerolinea3 INT = (SELECT TOP 1 IdAerolinea FROM Aerolineas WHERE IATA = 'AP3');

SET IDENTITY_INSERT [dbo].[Aviones] ON
GO

IF NOT EXISTS (SELECT 1 FROM Aviones WHERE Placa = 'AV-PRUEBA-001')
BEGIN
    INSERT INTO [dbo].[Aviones] ([IdAvion], [IdAerolinea], [Placa], [Modelo], [Tipo], [Capacidad], [FechaUltimoMantenimiento], [RangoKm], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdAerolinea1, 'AV-PRUEBA-001', 'Boeing 737', 'Comercial', 180, DATEADD(DAY, -30, GETDATE()), 5600, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdAerolinea2, 'AV-PRUEBA-002', 'Airbus A320', 'Comercial', 180, DATEADD(DAY, -15, GETDATE()), 6100, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdAerolinea3, 'AV-PRUEBA-003', 'Boeing 787', 'Comercial', 242, DATEADD(DAY, -7, GETDATE()), 14000, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Aviones] OFF
GO

-- =============================================
-- 7. PASAJEROS (Sin dependencias)
-- =============================================
SET IDENTITY_INSERT [dbo].[Pasajeros] ON
GO

IF NOT EXISTS (SELECT 1 FROM Pasajeros WHERE Pasaporte = 'P001-PRUEBA')
BEGIN
    INSERT INTO [dbo].[Pasajeros] ([IdPasajero], [Nombres], [Apellidos], [Pasaporte], [Correo], [Direccion], [Telefono], [Pais], [TipoPasajero], [Nacionalidad], [ContactoEmergencia], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, 'María', 'López Pérez', 'P001-PRUEBA', 'maria.lopez@email.com', 'Zona 1, Guatemala', 5552001, 'Guatemala', 'Adulto', 'Guatemalteca', 5552002, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, 'Juan', 'Martínez Sánchez', 'P002-PRUEBA', 'juan.martinez@email.com', 'San Salvador, El Salvador', 5552003, 'El Salvador', 'Adulto', 'Salvadoreño', 5552004, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, 'Carmen', 'Rodríguez Díaz', 'P003-PRUEBA', 'carmen.rodriguez@email.com', 'Tegucigalpa, Honduras', 5552005, 'Honduras', 'Adulto', 'Hondureña', 5552006, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Pasajeros] OFF
GO

-- =============================================
-- 8. VUELOS (Depende de Aviones)
-- =============================================
DECLARE @IdAvion1 INT = (SELECT TOP 1 IdAvion FROM Aviones WHERE Placa = 'AV-PRUEBA-001');
DECLARE @IdAvion2 INT = (SELECT TOP 1 IdAvion FROM Aviones WHERE Placa = 'AV-PRUEBA-002');
DECLARE @IdAvion3 INT = (SELECT TOP 1 IdAvion FROM Aviones WHERE Placa = 'AV-PRUEBA-003');

DECLARE @IdAeropuertoOrigen1 INT = (SELECT TOP 1 IdAeropuerto FROM Aeropuertos WHERE IATA = 'GUA');
DECLARE @IdAeropuertoDestino1 INT = (SELECT TOP 1 IdAeropuerto FROM Aeropuertos WHERE IATA = 'SAL');
DECLARE @IdAeropuertoOrigen2 INT = (SELECT TOP 1 IdAeropuerto FROM Aeropuertos WHERE IATA = 'SAL');
DECLARE @IdAeropuertoDestino2 INT = (SELECT TOP 1 IdAeropuerto FROM Aeropuertos WHERE IATA = 'TGU');

SET IDENTITY_INSERT [dbo].[Vuelos] ON
GO

IF NOT EXISTS (SELECT 1 FROM Vuelos WHERE NumeroVuelo = 'VUELO-PRUEBA-001')
BEGIN
    INSERT INTO [dbo].[Vuelos] ([IdVuelo], [IdAvion], [Aerolinea], [NumeroVuelo], [AeropuertoOrigen], [CodigoIATAOrigen], [AeropuertoDestino], [CodigoIATADestino], [FechaHoraSalida], [FechaHoraLlegada], [Clase], [AsientosDisponibles], [Precio], [Moneda], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdAvion1, 'Aerolínea de Prueba 1', 'VUELO-PRUEBA-001', 'Aeropuerto Internacional La Aurora - Prueba', 'GUA', 'Aeropuerto Internacional de El Salvador - Prueba', 'SAL', DATEADD(DAY, 7, GETDATE()), DATEADD(DAY, 7, DATEADD(HOUR, 2, GETDATE())), 'Económica', 150, 350.00, 'USD', 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdAvion2, 'Aerolínea de Prueba 2', 'VUELO-PRUEBA-002', 'Aeropuerto Internacional de El Salvador - Prueba', 'SAL', 'Aeropuerto Internacional Toncontín - Prueba', 'TGU', DATEADD(DAY, 10, GETDATE()), DATEADD(DAY, 10, DATEADD(HOUR, 1, GETDATE())), 'Ejecutiva', 50, 550.00, 'USD', 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdAvion3, 'Aerolínea de Prueba 3', 'VUELO-PRUEBA-003', 'Aeropuerto Internacional Toncontín - Prueba', 'TGU', 'Aeropuerto Internacional La Aurora - Prueba', 'GUA', DATEADD(DAY, 14, GETDATE()), DATEADD(DAY, 14, DATEADD(HOUR, 1, GETDATE())), 'Económica', 200, 400.00, 'USD', 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Vuelos] OFF
GO

-- =============================================
-- 9. BOLETOS (Depende de Vuelos y Pasajeros)
-- =============================================
DECLARE @IdVuelo1 INT = (SELECT TOP 1 IdVuelo FROM Vuelos WHERE NumeroVuelo = 'VUELO-PRUEBA-001');
DECLARE @IdVuelo2 INT = (SELECT TOP 1 IdVuelo FROM Vuelos WHERE NumeroVuelo = 'VUELO-PRUEBA-002');
DECLARE @IdVuelo3 INT = (SELECT TOP 1 IdVuelo FROM Vuelos WHERE NumeroVuelo = 'VUELO-PRUEBA-003');

DECLARE @IdPasajero1 INT = (SELECT TOP 1 IdPasajero FROM Pasajeros WHERE Pasaporte = 'P001-PRUEBA');
DECLARE @IdPasajero2 INT = (SELECT TOP 1 IdPasajero FROM Pasajeros WHERE Pasaporte = 'P002-PRUEBA');
DECLARE @IdPasajero3 INT = (SELECT TOP 1 IdPasajero FROM Pasajeros WHERE Pasaporte = 'P003-PRUEBA');

SET IDENTITY_INSERT [dbo].[Boletos] ON
GO

IF NOT EXISTS (SELECT 1 FROM Boletos WHERE NumeroAsiento = 'A10-PRUEBA')
BEGIN
    INSERT INTO [dbo].[Boletos] ([IdBoleto], [IdVuelo], [IdPasajero], [NumeroAsiento], [Clase], [Precio], [Cantidad], [Descuento], [Impuesto], [Total], [Reembolso], [FechaCompra], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdVuelo1, @IdPasajero1, 'A10-PRUEBA', 'Económica', 350.00, 1, 0.00, 42.00, 392.00, 'No aplica', GETDATE(), 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdVuelo2, @IdPasajero2, 'B15-PRUEBA', 'Ejecutiva', 550.00, 1, 0.00, 66.00, 616.00, 'No aplica', GETDATE(), 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdVuelo3, @IdPasajero3, 'C20-PRUEBA', 'Económica', 400.00, 2, 40.00, 91.20, 851.20, 'No aplica', GETDATE(), 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Boletos] OFF
GO

-- =============================================
-- 10. EQUIPAJE (Depende de Boletos)
-- =============================================
DECLARE @IdBoleto1 INT = (SELECT TOP 1 IdBoleto FROM Boletos WHERE NumeroAsiento = 'A10-PRUEBA');
DECLARE @IdBoleto2 INT = (SELECT TOP 1 IdBoleto FROM Boletos WHERE NumeroAsiento = 'B15-PRUEBA');
DECLARE @IdBoleto3 INT = (SELECT TOP 1 IdBoleto FROM Boletos WHERE NumeroAsiento = 'C20-PRUEBA');

SET IDENTITY_INSERT [dbo].[Equipaje] ON
GO

IF NOT EXISTS (SELECT 1 FROM Equipaje WHERE IdBoleto = @IdBoleto1)
BEGIN
    INSERT INTO [dbo].[Equipaje] ([IdEquipaje], [IdBoleto], [Peso], [Dimensiones], [CaracteristicasEspeciales], [Monto], [CostoExtra], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdBoleto1, 23.50, '40x60x100', 'Fragil', 50.00, 25.00, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdBoleto2, 18.00, '35x55x95', NULL, 30.00, 0.00, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdBoleto3, 32.00, '45x65x110', 'Equipaje deportivo', 80.00, 40.00, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Equipaje] OFF
GO

-- =============================================
-- 11. SERVICIOS (Depende de Boletos)
-- =============================================
SET IDENTITY_INSERT [dbo].[Servicios] ON
GO

IF NOT EXISTS (SELECT 1 FROM Servicios WHERE IdBoleto = @IdBoleto1 AND TipoServicio = 'WiFi')
BEGIN
    INSERT INTO [dbo].[Servicios] ([IdServicio], [IdBoleto], [Fecha], [TipoServicio], [Costo], [Cantidad], [CostoTotal], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdBoleto1, GETDATE(), 'WiFi', 15.00, 1, 15.00, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdBoleto2, GETDATE(), 'Comida', 25.00, 2, 50.00, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdBoleto3, GETDATE(), 'Entretenimiento', 10.00, 1, 10.00, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Servicios] OFF
GO

-- =============================================
-- 12. RESERVAS (Depende de Pasajeros y Vuelos)
-- =============================================
SET IDENTITY_INSERT [dbo].[Reservas] ON
GO

IF NOT EXISTS (SELECT 1 FROM Reservas WHERE IdPasajero = @IdPasajero1)
BEGIN
    INSERT INTO [dbo].[Reservas] ([IdReserva], [IdPasajero], [IdVuelo], [FechaReserva], [MontoAnticipo], [FechaVuelo], [Observaciones], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdPasajero1, @IdVuelo1, GETDATE(), 100.00, DATEADD(DAY, 7, GETDATE()), 'Reserva confirmada', 'Confirmado', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdPasajero2, @IdVuelo2, GETDATE(), 150.00, DATEADD(DAY, 10, GETDATE()), 'Requiere servicios especiales', 'Pendiente', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdPasajero3, @IdVuelo3, GETDATE(), 200.00, DATEADD(DAY, 14, GETDATE()), 'Grupo familiar', 'Confirmado', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Reservas] OFF
GO

-- =============================================
-- 13. FACTURACIÓN (Depende de Boletos)
-- =============================================
SET IDENTITY_INSERT [dbo].[Facturacion] ON
GO

IF NOT EXISTS (SELECT 1 FROM Facturacion WHERE IdBoleto = @IdBoleto1)
BEGIN
    INSERT INTO [dbo].[Facturacion] ([IdFactura], [IdBoleto], [FechaEmision], [HoraEmision], [Descripcion], [TipoPago], [Moneda], [Monto], [Impuesto], [MontoFactura], [MontoTotal], [NumeroAutorizacion], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdBoleto1, GETDATE(), CAST(GETDATE() AS TIME(7)), 'Factura boleto económico', 'Tarjeta de Crédito', 'USD', 350.00, 42.00, 350.00, 392.00, 123456789, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdBoleto2, GETDATE(), CAST(GETDATE() AS TIME(7)), 'Factura boleto ejecutivo', 'Efectivo', 'USD', 550.00, 66.00, 550.00, 616.00, 987654321, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdBoleto3, GETDATE(), CAST(GETDATE() AS TIME(7)), 'Factura boleto múltiple', 'Transferencia', 'USD', 800.00, 96.00, 800.00, 896.00, 456789123, 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Facturacion] OFF
GO

-- =============================================
-- 14. HISTORIALES (Depende de Boletos, Pasajeros, Aerolíneas, Vuelos)
-- =============================================
SET IDENTITY_INSERT [dbo].[Historiales] ON
GO

IF NOT EXISTS (SELECT 1 FROM Historiales WHERE IdBoleto = @IdBoleto1)
BEGIN
    INSERT INTO [dbo].[Historiales] ([IdHistorial], [IdBoleto], [IdPasajero], [IdAerolinea], [IdVuelo], [Observacion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdBoleto1, @IdPasajero1, @IdAerolinea1, @IdVuelo1, 'Vuelo completado exitosamente. Pasajero satisfecho con el servicio.', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdBoleto2, @IdPasajero2, @IdAerolinea2, @IdVuelo2, 'Vuelo en proceso. Pasajero requiere atención especial.', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdBoleto3, @IdPasajero3, @IdAerolinea3, @IdVuelo3, 'Reserva confirmada. Grupo familiar con 2 niños. Requiere servicios adicionales.', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Historiales] OFF
GO

-- =============================================
-- 15. MANTENIMIENTOS (Depende de Aviones y Empleados)
-- =============================================
DECLARE @IdEmpleado1 INT = (SELECT TOP 1 IdEmpleado FROM Empleados WHERE Nombre = 'Carlos Méndez');
DECLARE @IdEmpleado2 INT = (SELECT TOP 1 IdEmpleado FROM Empleados WHERE Nombre = 'Ana García');
DECLARE @IdEmpleado3 INT = (SELECT TOP 1 IdEmpleado FROM Empleados WHERE Nombre = 'Luis Ramírez');

SET IDENTITY_INSERT [dbo].[Mantenimientos] ON
GO

IF NOT EXISTS (SELECT 1 FROM Mantenimientos WHERE IdAvion = @IdAvion1 AND Tipo = 'Preventivo')
BEGIN
    INSERT INTO [dbo].[Mantenimientos] ([IdMantenimiento], [IdAvion], [IdEmpleado], [FechaIngreso], [FechaSalida], [Tipo], [Costo], [CostoExtra], [Descripcion], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdAvion1, @IdEmpleado3, DATEADD(DAY, -10, GETDATE()), DATEADD(DAY, -8, GETDATE()), 'Preventivo', 5000.00, 500.00, 'Mantenimiento preventivo rutinario. Revisión de motores y sistemas.', 'Finalizado', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdAvion2, @IdEmpleado3, DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, -3, GETDATE()), 'Correctivo', 8000.00, 1200.00, 'Reparación de sistema de navegación. Reemplazo de componentes.', 'Finalizado', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdAvion3, @IdEmpleado3, GETDATE(), DATEADD(DAY, 2, GETDATE()), 'Inspección', 3000.00, 0.00, 'Inspección anual completa. Verificación de certificaciones.', 'En Proceso', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Mantenimientos] OFF
GO

-- =============================================
-- 16. HORARIOS (Depende de Vuelos)
-- =============================================
SET IDENTITY_INSERT [dbo].[Horarios] ON
GO

IF NOT EXISTS (SELECT 1 FROM Horarios WHERE IdVuelo = @IdVuelo1)
BEGIN
    INSERT INTO [dbo].[Horarios] ([IdHorario], [IdVuelo], [HoraSalida], [HoraLlegada], [TiempoEspera], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdVuelo1, '08:00:00', '10:00:00', '00:30:00', 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdVuelo2, '14:30:00', '15:30:00', '00:15:00', 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdVuelo3, '10:15:00', '11:15:00', '00:20:00', 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Horarios] OFF
GO

-- =============================================
-- 17. ESCALAS (Depende de Vuelos y Aeropuertos)
-- =============================================
SET IDENTITY_INSERT [dbo].[Escalas] ON
GO

DECLARE @IdAeropuerto1 INT = (SELECT TOP 1 IdAeropuerto FROM Aeropuertos WHERE IATA = 'GUA');
DECLARE @IdAeropuerto2 INT = (SELECT TOP 1 IdAeropuerto FROM Aeropuertos WHERE IATA = 'SAL');
DECLARE @IdAeropuerto3 INT = (SELECT TOP 1 IdAeropuerto FROM Aeropuertos WHERE IATA = 'TGU');

IF NOT EXISTS (SELECT 1 FROM Escalas WHERE IdVuelo = @IdVuelo1)
BEGIN
    INSERT INTO [dbo].[Escalas] ([IdEscala], [IdVuelo], [IdAeropuerto], [HoraLlegada], [HoraSalida], [TiempoEspera], [Estado], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdVuelo1, @IdAeropuerto2, '09:30:00', '10:00:00', '00:30:00', 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdVuelo2, @IdAeropuerto3, '15:00:00', '15:15:00', '00:15:00', 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdVuelo3, @IdAeropuerto1, '10:45:00', '11:05:00', '00:20:00', 'Activo', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Escalas] OFF
GO

-- =============================================
-- 18. OPCIONES (Sin dependencias)
-- =============================================
SET IDENTITY_INSERT [dbo].[Opciones] ON
GO

IF NOT EXISTS (SELECT 1 FROM Opciones WHERE NombreOpcion = 'Crear')
BEGIN
    INSERT INTO [dbo].[Opciones] ([IdOpcion], [NombreOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, 'Crear', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, 'Editar', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, 'Eliminar', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Opciones] OFF
GO

-- =============================================
-- 19. PANTALLAS (Sin dependencias)
-- =============================================
SET IDENTITY_INSERT [dbo].[Pantallas] ON
GO

IF NOT EXISTS (SELECT 1 FROM Pantallas WHERE NombrePantalla = 'Equipaje')
BEGIN
    INSERT INTO [dbo].[Pantallas] ([IdPantalla], [NombrePantalla], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, 'Equipaje', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, 'Servicios', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, 'Reservas', 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[Pantallas] OFF
GO

-- =============================================
-- 20. ROLPANTALLAOPCION (Depende de Roles, Pantallas, Opciones)
-- =============================================
DECLARE @IdRol INT = (SELECT TOP 1 IdRol FROM Roles WHERE NombreRol = 'SuperAdmin');
DECLARE @IdPantalla1 INT = (SELECT TOP 1 IdPantalla FROM Pantallas WHERE NombrePantalla = 'Equipaje');
DECLARE @IdPantalla2 INT = (SELECT TOP 1 IdPantalla FROM Pantallas WHERE NombrePantalla = 'Servicios');
DECLARE @IdPantalla3 INT = (SELECT TOP 1 IdPantalla FROM Pantallas WHERE NombrePantalla = 'Reservas');

DECLARE @IdOpcion1 INT = (SELECT TOP 1 IdOpcion FROM Opciones WHERE NombreOpcion = 'Crear');
DECLARE @IdOpcion2 INT = (SELECT TOP 1 IdOpcion FROM Opciones WHERE NombreOpcion = 'Editar');
DECLARE @IdOpcion3 INT = (SELECT TOP 1 IdOpcion FROM Opciones WHERE NombreOpcion = 'Eliminar');

SET IDENTITY_INSERT [dbo].[RolPantallaOpcion] ON
GO

IF NOT EXISTS (SELECT 1 FROM RolPantallaOpcion WHERE IdRol = @IdRol AND IdPantalla = @IdPantalla1 AND IdOpcion = @IdOpcion1)
BEGIN
    INSERT INTO [dbo].[RolPantallaOpcion] ([IdRolPantallaOpcion], [IdRol], [IdPantalla], [IdOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES 
        (100, @IdRol, @IdPantalla1, @IdOpcion1, 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (101, @IdRol, @IdPantalla2, @IdOpcion2, 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7))),
        (102, @IdRol, @IdPantalla3, @IdOpcion3, 'SYSTEM', GETDATE(), CAST(GETDATE() AS TIME(7)));
END
GO

SET IDENTITY_INSERT [dbo].[RolPantallaOpcion] OFF
GO

-- =============================================
-- VERIFICACIÓN FINAL
-- =============================================
PRINT '=== VERIFICACIÓN DE DATOS INSERTADOS ===';
PRINT 'Roles: ' + CAST((SELECT COUNT(*) FROM Roles) AS VARCHAR);
PRINT 'Usuarios: ' + CAST((SELECT COUNT(*) FROM Usuarios) AS VARCHAR);
PRINT 'Empleados: ' + CAST((SELECT COUNT(*) FROM Empleados) AS VARCHAR);
PRINT 'Aerolíneas: ' + CAST((SELECT COUNT(*) FROM Aerolineas) AS VARCHAR);
PRINT 'Aeropuertos: ' + CAST((SELECT COUNT(*) FROM Aeropuertos) AS VARCHAR);
PRINT 'Aviones: ' + CAST((SELECT COUNT(*) FROM Aviones) AS VARCHAR);
PRINT 'Pasajeros: ' + CAST((SELECT COUNT(*) FROM Pasajeros) AS VARCHAR);
PRINT 'Vuelos: ' + CAST((SELECT COUNT(*) FROM Vuelos) AS VARCHAR);
PRINT 'Boletos: ' + CAST((SELECT COUNT(*) FROM Boletos) AS VARCHAR);
PRINT 'Equipaje: ' + CAST((SELECT COUNT(*) FROM Equipaje) AS VARCHAR);
PRINT 'Servicios: ' + CAST((SELECT COUNT(*) FROM Servicios) AS VARCHAR);
PRINT 'Reservas: ' + CAST((SELECT COUNT(*) FROM Reservas) AS VARCHAR);
PRINT 'Facturación: ' + CAST((SELECT COUNT(*) FROM Facturacion) AS VARCHAR);
PRINT 'Historiales: ' + CAST((SELECT COUNT(*) FROM Historiales) AS VARCHAR);
PRINT 'Mantenimientos: ' + CAST((SELECT COUNT(*) FROM Mantenimientos) AS VARCHAR);
PRINT 'Horarios: ' + CAST((SELECT COUNT(*) FROM Horarios) AS VARCHAR);
PRINT 'Escalas: ' + CAST((SELECT COUNT(*) FROM Escalas) AS VARCHAR);
PRINT 'Opciones: ' + CAST((SELECT COUNT(*) FROM Opciones) AS VARCHAR);
PRINT 'Pantallas: ' + CAST((SELECT COUNT(*) FROM Pantallas) AS VARCHAR);
PRINT 'RolPantallaOpcion: ' + CAST((SELECT COUNT(*) FROM RolPantallaOpcion) AS VARCHAR);
PRINT '=========================================';
PRINT 'Script de datos de prueba ejecutado correctamente.';
GO

