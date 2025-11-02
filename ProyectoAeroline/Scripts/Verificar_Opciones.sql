-- =============================================
-- SCRIPT PARA VERIFICAR Y CREAR OPCIONES
-- =============================================
-- Este script asegura que la tabla Opciones tenga los registros necesarios
-- para que funcionen los stored procedures de RolPantallaOpcion

-- Verificar si existe la tabla Opciones
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Opciones')
BEGIN
    PRINT 'ERROR: La tabla Opciones no existe en la base de datos.'
    PRINT 'Por favor, crea la tabla primero usando el esquema que compartiste.'
    RETURN
END

-- Insertar 'Ver' si no existe
IF NOT EXISTS (SELECT * FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Ver' AND [FechaEliminacion] IS NULL)
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES ('Ver', SYSTEM_USER, GETDATE(), CAST(GETDATE() AS TIME(7)))
    PRINT 'Opción "Ver" creada.'
END
ELSE
BEGIN
    PRINT 'Opción "Ver" ya existe.'
END

-- Insertar 'Crear' si no existe
IF NOT EXISTS (SELECT * FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Crear' AND [FechaEliminacion] IS NULL)
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES ('Crear', SYSTEM_USER, GETDATE(), CAST(GETDATE() AS TIME(7)))
    PRINT 'Opción "Crear" creada.'
END
ELSE
BEGIN
    PRINT 'Opción "Crear" ya existe.'
END

-- Insertar 'Editar' si no existe
IF NOT EXISTS (SELECT * FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Editar' AND [FechaEliminacion] IS NULL)
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES ('Editar', SYSTEM_USER, GETDATE(), CAST(GETDATE() AS TIME(7)))
    PRINT 'Opción "Editar" creada.'
END
ELSE
BEGIN
    PRINT 'Opción "Editar" ya existe.'
END

-- Insertar 'Eliminar' si no existe
IF NOT EXISTS (SELECT * FROM [dbo].[Opciones] WHERE [NombreOpcion] = 'Eliminar' AND [FechaEliminacion] IS NULL)
BEGIN
    INSERT INTO [dbo].[Opciones] ([NombreOpcion], [UsuarioCreacion], [FechaCreacion], [HoraCreacion])
    VALUES ('Eliminar', SYSTEM_USER, GETDATE(), CAST(GETDATE() AS TIME(7)))
    PRINT 'Opción "Eliminar" creada.'
END
ELSE
BEGIN
    PRINT 'Opción "Eliminar" ya existe.'
END

PRINT ''
PRINT 'Verificación de Opciones completada.'
PRINT 'Ejecuta ahora el script StoredProcedures_RolPantallaOpcion_Corregido.sql'
GO

