USE VetCareDb;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Correo = N'recepcion@vetcare.com')
   OR NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Correo = N'veterinario@vetcare.com')
BEGIN
	THROW 50001, 'Primero registra recepcion@vetcare.com y veterinario@vetcare.com mediante POST /api/auth/registro.', 1;
END
GO

UPDATE dbo.Usuarios
SET Rol = N'Recepcionista'
WHERE Correo = N'recepcion@vetcare.com';

UPDATE dbo.Usuarios
SET Rol = N'Veterinario'
WHERE Correo = N'veterinario@vetcare.com';
GO

SELECT UsuarioId, Correo, Rol, FechaRegistro
FROM dbo.Usuarios
WHERE Correo IN (N'recepcion@vetcare.com', N'veterinario@vetcare.com')
ORDER BY Correo;
GO
