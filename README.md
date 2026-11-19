# VetCare

Web API REST para la gestión básica de una veterinaria, desarrollada con ASP.NET Core .NET 10, Entity Framework Core, SQL Server, DTOs y AutoMapper.

## 1. Problema que resuelve

VetCare permite registrar propietarios y sus mascotas, consultar la información mediante una API REST y aplicar reglas de negocio para evitar datos duplicados o eliminaciones que romperían la relación entre las entidades.

## 2. Modelo de datos

- `Propietario`: cliente de la veterinaria.
- `Mascota`: animal registrado y asociado a un propietario.
- `Usuario`: cuenta de acceso con correo, hash BCrypt de la clave y rol de negocio.
- Relación: un propietario puede tener muchas mascotas; cada mascota pertenece a un propietario.

Los scripts están en `database/VetCareDb.sql`, `database/Usuarios.sql` y `database/AsignarRolesUsuarios.sql`.

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

### Autenticación

- `POST /api/auth/registro`: público; registra una cuenta como `Recepcionista`.
- `POST /api/auth/login`: público; devuelve un JWT o `401` si las credenciales no son válidas.
- `GET /api/auth/perfil`: requiere token; devuelve correo y rol leídos de sus claims.

## 7. Roles y matriz de permisos

`Recepcionista` registra propietarios y mascotas y consulta los datos. `Veterinario` consulta los datos y actualiza o elimina mascotas. El rol lo asigna el servidor; el registro no acepta un rol del cliente.

| Endpoint | Sin token | Recepcionista | Veterinario |
|---|---|---|---|
| `POST /api/auth/registro` | Público | Público | Público |
| `POST /api/auth/login` | Público | Público | Público |
| `GET /api/auth/perfil` | `401` | Sí | Sí |
| `GET /api/propietarios` y `GET /api/propietarios/{id}` | `401` | Sí | Sí |
| `POST/PUT/DELETE /api/propietarios` | `401` | Sí | `403` |
| `GET /api/mascotas` y `GET /api/mascotas/{id}` | `401` | Sí | Sí |
| `GET /api/propietarios/{id}/mascotas` | `401` | Sí | Sí |
| `POST /api/mascotas` | `401` | Sí | `403` |
| `PUT/DELETE /api/mascotas/{id}` | `401` | `403` | Sí |

`401 Unauthorized` significa que falta un token válido o las credenciales de login no son válidas. `403 Forbidden` significa que el token sí es válido, pero su rol no está autorizado para esa operación.

## 8. Sustentación de seguridad

- El JWT incluye `sub` (identificador del usuario), `email` y `rol`, para identificar al usuario y autorizar sus operaciones. No se deben incluir contraseñas, hashes, claves secretas ni información sensible innecesaria.
- BCrypt guarda una representación con hash y salt, no la contraseña en texto. Si se guardara texto plano, una filtración de la base revelaría directamente las claves y permitiría reutilizarlas en otros servicios.
- El rol se asigna en el servidor porque confiar en un rol recibido durante el registro permitiría que un cliente se concediera permisos elevados. La autorización usa el rol incluido en el JWT firmado.
- El registro devuelve `409` si el correo ya existe; la API responde `400` ante datos de entrada no válidos; las consultas y el login correcto devuelven `200`; el registro exitoso devuelve `201`; las actualizaciones y eliminaciones exitosas devuelven `204`; un recurso inexistente devuelve `404`.

## 9. Pruebas y evidencias

Las pruebas manuales pendientes deben realizarse en Swagger con cuentas registradas mediante `POST /api/auth/registro`. Registra `recepcion@vetcare.com` y `veterinario@vetcare.com` con claves de prueba elegidas por ti; luego ejecuta `database/AsignarRolesUsuarios.sql` para dejar cada cuenta con su rol. El script no contiene hashes de contraseña inventados.

Comprueba los códigos `200`, `201`, `204`, `400`, `401`, `403`, `404` y `409`, usa tokens de ambos roles, prueba el botón **Authorize** y guarda las capturas requeridas. No publiques los tokens ni secretos. La compilación se valida con `dotnet build`; las pruebas en Swagger y sus evidencias quedan a cargo del usuario.

## 10. Ejecución local

1. Ejecutar `database/VetCareDb.sql` y `database/Usuarios.sql` en SQL Server Management Studio.
2. Verificar que exista la base `VetCareDb` y configurar localmente la cadena de conexión.
3. Configurar la clave JWT con User Secrets (mínimo 32 caracteres), no en el repositorio:

```powershell
dotnet user-secrets set "Jwt:Key" "<clave-local-de-al-menos-32-caracteres>" --project .\VetCare.Api\VetCare.Api.csproj
```

4. Ejecutar la solución desde Visual Studio o con:

```powershell
dotnet run --project .\VetCare.Api\VetCare.Api.csproj
```

5. Abrir Swagger en la URL indicada por la aplicación, normalmente `/swagger`.

La configuración de desarrollo está excluida mediante `.gitignore` para no subir cadenas de conexión locales.

## 11. Observaciones y conclusiones

La API protege los recursos de VetCare mediante JWT y permisos diferenciados para recepción y veterinaria. Los hashes BCrypt y la validación del token reducen el riesgo de exposición de credenciales y de acceso no autorizado. La compilación se ha validado; las pruebas funcionales, las capturas y las conclusiones basadas en resultados reales deben completarse después de ejecutar manualmente los casos en Swagger.
