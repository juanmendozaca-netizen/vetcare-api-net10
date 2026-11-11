IF DB_ID(N'VetCareDb') IS NULL
BEGIN
	CREATE DATABASE VetCareDb;
END
GO

USE VetCareDb;
GO

IF OBJECT_ID(N'dbo.Propietarios', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Propietarios
	(
		PropietarioId INT IDENTITY(1,1) NOT NULL,
		Nombres NVARCHAR(80) NOT NULL,
		Apellidos NVARCHAR(80) NOT NULL,
		Telefono NVARCHAR(20) NOT NULL,
		Correo NVARCHAR(150) NOT NULL,
		FechaRegistro DATETIME2(0) NOT NULL
			CONSTRAINT DF_Propietarios_FechaRegistro DEFAULT SYSUTCDATETIME(),
		CONSTRAINT PK_Propietarios PRIMARY KEY (PropietarioId),
		CONSTRAINT UQ_Propietarios_Correo UNIQUE (Correo)
	);
END
GO

IF OBJECT_ID(N'dbo.Mascotas', N'U') IS NULL
BEGIN
	CREATE TABLE dbo.Mascotas
	(
		MascotaId INT IDENTITY(1,1) NOT NULL,
		PropietarioId INT NOT NULL,
		Nombre NVARCHAR(80) NOT NULL,
		Especie NVARCHAR(40) NOT NULL,
		Raza NVARCHAR(80) NULL,
		FechaNacimiento DATE NULL,
		Peso DECIMAL(6,2) NULL,
		NumeroMicrochip NVARCHAR(30) NOT NULL,
		FechaRegistro DATETIME2(0) NOT NULL
			CONSTRAINT DF_Mascotas_FechaRegistro DEFAULT SYSUTCDATETIME(),
		CONSTRAINT PK_Mascotas PRIMARY KEY (MascotaId),
		CONSTRAINT UQ_Mascotas_NumeroMicrochip UNIQUE (NumeroMicrochip),
		CONSTRAINT CK_Mascotas_Peso CHECK (Peso IS NULL OR Peso > 0),
		CONSTRAINT FK_Mascotas_Propietarios
			FOREIGN KEY (PropietarioId)
			REFERENCES dbo.Propietarios (PropietarioId)
	);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Propietarios WHERE Correo = N'ana.garcia@correo.com')
BEGIN
	INSERT INTO dbo.Propietarios (Nombres, Apellidos, Telefono, Correo)
	VALUES
		(N'Ana', N'García', N'987654321', N'ana.garcia@correo.com'),
		(N'Luis', N'Mendoza', N'986123456', N'luis.mendoza@correo.com');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Mascotas WHERE NumeroMicrochip = N'MC-100001')
BEGIN
	INSERT INTO dbo.Mascotas
		(PropietarioId, Nombre, Especie, Raza, FechaNacimiento, Peso, NumeroMicrochip)
	SELECT
		p.PropietarioId,
		datos.Nombre,
		datos.Especie,
		datos.Raza,
		datos.FechaNacimiento,
		datos.Peso,
		datos.NumeroMicrochip
	FROM
	(
		VALUES
			(N'ana.garcia@correo.com', N'Bobby', N'Perro', N'Beagle', CONVERT(date, '2021-04-15'), CONVERT(decimal(6,2), 12.50), N'MC-100001'),
			(N'ana.garcia@correo.com', N'Luna', N'Gato', N'Siamés', CONVERT(date, '2022-08-03'), CONVERT(decimal(6,2), 4.20), N'MC-100002'),
			(N'luis.mendoza@correo.com', N'Rocky', N'Perro', N'Labrador', CONVERT(date, '2020-11-20'), CONVERT(decimal(6,2), 28.00), N'MC-100003')
	) AS datos(CorreoPropietario, Nombre, Especie, Raza, FechaNacimiento, Peso, NumeroMicrochip)
	INNER JOIN dbo.Propietarios AS p
		ON p.Correo = datos.CorreoPropietario;
END
GO

SELECT
	p.PropietarioId,
	p.Nombres,
	p.Apellidos,
	m.MascotaId,
	m.Nombre AS NombreMascota,
	m.Especie,
	m.NumeroMicrochip
FROM dbo.Propietarios AS p
LEFT JOIN dbo.Mascotas AS m
	ON m.PropietarioId = p.PropietarioId
ORDER BY p.PropietarioId, m.MascotaId;
GO
