USE VetCareDb;
GO

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Usuarios
	(
		UsuarioId INT IDENTITY(1,1) NOT NULL,
		Correo NVARCHAR(150) NOT NULL,
		ClaveHash NVARCHAR(255) NOT NULL,
		Rol NVARCHAR(30) NOT NULL,
		FechaRegistro DATETIME2(0) NOT NULL
			CONSTRAINT DF_Usuarios_FechaRegistro DEFAULT SYSUTCDATETIME(),
		CONSTRAINT PK_Usuarios PRIMARY KEY (UsuarioId),
		CONSTRAINT UQ_Usuarios_Correo UNIQUE (Correo),
		CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN (N'Recepcionista', N'Veterinario'))
	);
END
GO

DELETE FROM dbo.Usuarios
WHERE ClaveHash = N'$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy'
  AND Correo IN (N'recepcion@vetcare.com', N'veterinario@vetcare.com');
GO

SELECT UsuarioId, Correo, Rol, FechaRegistro
FROM dbo.Usuarios
ORDER BY UsuarioId;
GO
