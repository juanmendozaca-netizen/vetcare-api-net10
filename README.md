# VetCare

Web API REST para la gestión básica de una veterinaria, desarrollada con ASP.NET Core .NET 10, Entity Framework Core, SQL Server, DTOs y AutoMapper.

## 1. Problema que resuelve

VetCare permite registrar propietarios y sus mascotas, consultar la información mediante una API REST y aplicar reglas de negocio para evitar datos duplicados o eliminaciones que romperían la relación entre las entidades.

## 2. Modelo de datos

- `Propietario`: cliente de la veterinaria.
- `Mascota`: animal registrado y asociado a un propietario.
- Relación: un propietario puede tener muchas mascotas; cada mascota pertenece a un propietario.

El script se encuentra en `database/VetCareDb.sql` e incluye datos de ejemplo.

## 3. Arquitectura

```text
VetCare.Api          -> controladores, Program.cs y Swagger
VetCare.Negocio      -> servicios, reglas de negocio y MappingProfile
VetCare.Datos        -> DbContext y repositorios
VetCare.Entidades    -> entidades y DTOs
```

Referencias:

```text
Api -> Negocio -> Datos -> Entidades
```

Los controladores solo dependen de interfaces de servicios y DTOs. No utilizan directamente el DbContext ni las entidades.

## 4. Reglas de negocio

1. No se permite registrar dos propietarios con el mismo correo.
2. No se permite registrar dos mascotas con el mismo número de microchip.
3. No se puede eliminar un propietario que tenga mascotas registradas.
4. La mascota debe tener un propietario existente.

Las reglas lanzan `ReglaNegocioException`, que los controladores convierten en `409 Conflict`.

## 5. Sustentación de AutoMapper

| Mapeo | Origen -> destino | Motivo | Configuración especial |
|---|---|---|---|
| `Propietario -> PropietarioDto` | Entidad -> DTO de lectura | La API devuelve datos públicos sin exponer la entidad | `MascotasCount` se obtiene de la colección relacionada mediante `MapFrom` |
| `PropietarioCrearDto -> Propietario` | DTO de entrada -> entidad | Convierte la solicitud del cliente en una entidad persistible | El identificador y la fecha se generan en la base de datos |
| `PropietarioActualizarDto -> Propietario` | DTO de entrada -> entidad existente | Actualiza una entidad cargada por el servicio | Se ignoran identificador, fecha y colección de mascotas |
| `Mascota -> MascotaDto` | Entidad -> DTO de lectura | La API devuelve la mascota sin exponer la entidad ni el objeto relacionado completo | `PropietarioNombre` se construye con `Nombres` y `Apellidos` mediante `MapFrom` |
| `MascotaCrearDto -> Mascota` | DTO de entrada -> entidad | Prepara una mascota nueva para guardarla | Se ignoran identificador, fecha y navegación `Propietario` |
| `MascotaActualizarDto -> Mascota` | DTO de entrada -> entidad existente | Actualiza una mascota cargada por el servicio | Se ignoran identificador, fecha y navegación `Propietario` |

No se usa `ReverseMap()` porque los DTOs de entrada y los DTOs de salida tienen responsabilidades distintas y contienen propiedades calculadas.

## 6. Endpoints

### Propietarios

- `GET /api/propietarios`
- `GET /api/propietarios/{id}`
- `POST /api/propietarios`
- `PUT /api/propietarios/{id}`
- `DELETE /api/propietarios/{id}`

### Mascotas

- `GET /api/mascotas?especie=Perro&nombre=Bo`
- `GET /api/mascotas/{id}`
- `GET /api/propietarios/{propietarioId}/mascotas`
- `POST /api/mascotas`
- `PUT /api/mascotas/{id}`
- `DELETE /api/mascotas/{id}`

El archivo `VetCare.Api/VetCare.http` contiene solicitudes para demostrar los códigos `200`, `201`, `204`, `400`, `404` y `409`.

## 7. Ejecución local

1. Ejecutar `database/VetCareDb.sql` en SQL Server Management Studio conectado a `LAPTOP-L6CI0M35\\SQLEXPRESS`.
2. Verificar que exista la base `VetCareDb`.
3. Configurar la cadena local en `VetCare.Api/appsettings.Development.json`.
4. Ejecutar la solución desde Visual Studio o con:

```powershell
dotnet run --project .\VetCare.Api\VetCare.Api.csproj
```

5. Abrir Swagger en la URL indicada por la aplicación, normalmente `/swagger`.

La configuración de desarrollo está excluida mediante `.gitignore` para no subir cadenas de conexión locales.

## 8. Resultados y conclusiones

La API implementa una arquitectura en capas, separación de responsabilidades, persistencia con EF Core, conversión exclusiva mediante DTOs, validación de entrada, reglas de negocio y documentación Swagger. Las pruebas locales confirmaron las respuestas HTTP requeridas por la actividad.
