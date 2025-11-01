-- =============================================
-- Script: Aumentar tamaño de columna Contraseña
-- Descripción: La columna Contraseña necesita ser más grande para almacenar hashes
-- =============================================

USE [AerolineaPruebaDB]
GO

-- Verificar el tamaño actual de la columna
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Usuarios' 
  AND COLUMN_NAME = 'Contraseña'
GO

-- Aumentar el tamaño de la columna Contraseña a NVARCHAR(MAX) o al menos 256 caracteres
-- El hash Base64 puede ser de hasta ~100 caracteres, pero usamos MAX para estar seguros

-- Opción 1: Cambiar a NVARCHAR(MAX) (recomendado)
ALTER TABLE [dbo].[Usuarios]
ALTER COLUMN [Contraseña] NVARCHAR(MAX) NULL
GO

-- Si la opción 1 falla, usar esta alternativa con un tamaño fijo grande:
-- ALTER TABLE [dbo].[Usuarios]
-- ALTER COLUMN [Contraseña] NVARCHAR(500) NULL
-- GO

PRINT 'Columna Contraseña actualizada correctamente'
GO

