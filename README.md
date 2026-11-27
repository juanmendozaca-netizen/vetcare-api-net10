# VetCare — Microservicios con API Gateway y Ocelot (Semana 15)

Solución distribuida para la gestión de una clínica veterinaria, desarrollada con **ASP.NET Core .NET 10**, **Ocelot 25.0.0** (API Gateway), **JWT Bearer**, **Entity Framework Core 10**, **SQL Server / Azure SQL Database**, **DTOs** y **AutoMapper**.

---

## 1. Explicación del Tema y Microservicios

La solución se compone de un **API Gateway** como único punto de entrada para los clientes y **dos microservicios independientes** que no comparten base de datos ni clases entre sí:

1. **Microservicio 1 (`VetCare.Api` — Puerto local `5169`)**:
   - API REST en arquitectura de 4 capas (`VetCare.Api -> VetCare.Negocio -> VetCare.Datos -> VetCare.Entidades`) conectada a **SQL Server (`VetCareDb`)**.
   - Gestiona el registro e inicio de sesión de usuarios (`Recepcionista` y `Veterinario`) con contraseñas hasheadas en **BCrypt** y emisión de tokens **JWT**, además del CRUD completo de **Propietarios** y **Mascotas** aplicando reglas de negocio (unicidad de correo y microchip, existencia de propietario y bloqueo de eliminación de propietarios con mascotas registradas).
2. **Microservicio 2 (`VetCare.VeterinariosApi` — Puerto local `5170`)**:
   - Microservicio autónomo con almacenamiento **en memoria** (sin base de datos ni referencias a las capas del Microservicio 1).
   - Gestiona el catálogo de **Médicos Veterinarios** de la clínica (`VeterinarioId`, `Nombres`, `Apellidos`, `Especialidad`, `NumeroColegiatura`, `Turno`, `Correo`) mediante los endpoints `GET /api/veterinarios` (con filtro opcional por especialidad) y `GET /api/veterinarios/{id}`.
   - Valida el mismo token **JWT** emitido por `VetCare.Api` (compartiendo `Jwt:Key`, `Jwt:Issuer = "VetCare.Api"` y `Jwt:Audience = "VetCare.Client"`) y exige autenticación (`[Authorize]`) en todos sus endpoints.
3. **API Gateway (`GatewayApi` — Puerto local `5168`)**:
   - Construido con **Ocelot 25.0.0** y **JwtBearer 10.0.12**.
   - Centraliza todas las peticiones del cliente bajo el prefijo `/gateway/...`, valida el token JWT antes de reenviar hacia las rutas protegidas y oculta los puertos/direcciones internas de los dos microservicios.

---

## 2. Arquitectura de la Solución

```text
Cliente (Postman / Gateway.http)
        │
        ▼  (Único punto de entrada: http://localhost:5168)
┌─────────────────────────┐
│       GatewayApi        │  -> Ocelot 25.0.0 + Validación JWT ("Bearer")
└───────────┬─────────────┘
            │
            ├────► Microservicio 1: VetCare.Api (:5169)
            │         ├── VetCare.Negocio (Servicios, Reglas, AutoMapper, JWT, BCrypt)
            │         ├── VetCare.Datos (EF Core 10, VetCareContext, Repositorios)
            │         ├── VetCare.Entidades (Entidades y DTOs)
            │         └── Base de Datos: SQL Server / Azure SQL (VetCareDb)
            │
            └────► Microservicio 2: VetCare.VeterinariosApi (:5170)
                      ├── Controllers (VeterinariosController con [Authorize])
                      ├── Services (VeterinarioMemoriaService en memoria)
                      └── Models (VeterinarioDto)
```

---

## 3. Tabla de Rutas (Upstream → Downstream)

### 3.1 Entorno Local

| Upstream (cliente → gateway `:5168`) | Método | Downstream (gateway → servicio) | Token |
|---|---|---|---|
| `/gateway/auth/{everything}` | `POST` | `localhost:5169` `/api/auth/{everything}` | No |
| `/gateway/auth/perfil` | `GET` | `localhost:5169` `/api/auth/perfil` | Sí |
| `/gateway/propietarios{everything}` | `GET` | `localhost:5169` `/api/propietarios{everything}` | No |
| `/gateway/mascotas{everything}` | `GET` | `localhost:5169` `/api/mascotas{everything}` | No |
| `/gateway/propietarios{everything}` | `POST, PUT, DELETE` | `localhost:5169` `/api/propietarios{everything}` | Sí |
| `/gateway/mascotas{everything}` | `POST, PUT, DELETE` | `localhost:5169` `/api/mascotas{everything}` | Sí |
| `/gateway/veterinarios{everything}` | `GET` | `localhost:5170` `/api/veterinarios{everything}` | Sí |

### 3.2 Entorno en la Nube (Microsoft Azure)

*(Reemplazar los subdominios `.azurewebsites.net` por los nombres finales asignados en tu suscripción de Azure; la plantilla lista para producción se encuentra en `GatewayApi/ocelot.azure.json`)*.

| Upstream (cliente → gateway HTTPS) | Método | Downstream (gateway → servicio HTTPS `:443`) | Token |
|---|---|---|---|
| `/gateway/auth/{everything}` | `POST` | `vetcare-api-s15.azurewebsites.net:443` `/api/auth/{everything}` | No |
| `/gateway/auth/perfil` | `GET` | `vetcare-api-s15.azurewebsites.net:443` `/api/auth/perfil` | Sí |
| `/gateway/propietarios{everything}` | `GET` | `vetcare-api-s15.azurewebsites.net:443` `/api/propietarios{everything}` | No |
| `/gateway/mascotas{everything}` | `GET` | `vetcare-api-s15.azurewebsites.net:443` `/api/mascotas{everything}` | No |
| `/gateway/propietarios{everything}` | `POST, PUT, DELETE` | `vetcare-api-s15.azurewebsites.net:443` `/api/propietarios{everything}` | Sí |
| `/gateway/mascotas{everything}` | `POST, PUT, DELETE` | `vetcare-api-s15.azurewebsites.net:443` `/api/mascotas{everything}` | Sí |
| `/gateway/veterinarios{everything}` | `GET` | `vetcare-veterinarios-s15.azurewebsites.net:443` `/api/veterinarios{everything}` | Sí |

---

## 4. Roles y Matriz de Permisos

- **`Recepcionista`**: asignado por defecto en el servidor al registrarse; puede crear, actualizar y eliminar propietarios, y registrar mascotas.
- **`Veterinario`**: puede actualizar y eliminar mascotas.
- Ambos roles autenticados pueden consultar su perfil (`/gateway/auth/perfil`) y el catálogo del Microservicio 2 (`/gateway/veterinarios`).

| Endpoint por el Gateway | Sin token | Recepcionista | Veterinario |
|---|---|---|---|
| `POST /gateway/auth/registro` y `POST /gateway/auth/login` | Público (`201` / `200`) | Público | Público |
| `GET /gateway/auth/perfil` | `401` | `200` | `200` |
| `GET /gateway/propietarios` y `GET /gateway/propietarios/{id}` | Público (`200`) | `200` | `200` |
| `GET /gateway/mascotas` y `GET /gateway/mascotas/{id}` | Público (`200`) | `200` | `200` |
| `GET /gateway/propietarios/{id}/mascotas` | Público (`200`) | `200` | `200` |
| `POST/PUT/DELETE /gateway/propietarios` | `401` | Sí (`201`/`204`) | `403` |
| `POST /gateway/mascotas` | `401` | Sí (`201`) | `403` |
| `PUT/DELETE /gateway/mascotas/{id}` | `401` | `403` | Sí (`204`) |
| `GET /gateway/veterinarios` y `GET /gateway/veterinarios/{id}` | `401` | `200` | `200` |

---

## 5. Ejecución Local de los 3 Servicios

1. **Base de datos SQL Server**:
   - Ejecutar `database/VetCareDb.sql` y `database/Usuarios.sql` en SQL Server Management Studio 2022.
2. **Clave secreta JWT compartida (User Secrets)**:
   - Los tres proyectos ejecutables (`VetCare.Api`, `VetCare.VeterinariosApi` y `GatewayApi`) comparten el mismo `<UserSecretsId>VetCare.Api-Development</UserSecretsId>`, por lo que basta con configurar la clave una sola vez:
   ```powershell
   dotnet user-secrets set "Jwt:Key" "ClaveSecretaSuperSeguraParaVetCare2026_Minimo32Caracteres!" --project .\VetCare.Api\VetCare.Api.csproj
   ```
3. **Levantar los 3 proyectos simultáneamente** (cada uno en su propia terminal o con *Inicio de varios proyectos* en Visual Studio):
   - **Terminal 1 (Microservicio 1 — Puerto `5169`)**:
     ```powershell
     dotnet run --project .\VetCare.Api\VetCare.Api.csproj --launch-profile http
     ```
   - **Terminal 2 (Microservicio 2 — Puerto `5170`)**:
     ```powershell
     dotnet run --project .\VetCare.VeterinariosApi\VetCare.VeterinariosApi.csproj --launch-profile http
     ```
   - **Terminal 3 (API Gateway Ocelot — Puerto `5168`)**:
     ```powershell
     dotnet run --project .\GatewayApi\GatewayApi.csproj --launch-profile http
     ```
4. **Ejecutar las pruebas**:
   - Abrir `GatewayApi/Gateway.http` (o Postman) y ejecutar todos los casos (`200`, `201`, `204`, `401`, `403`, `404` y `502`) apuntando únicamente al puerto del Gateway (`http://localhost:5168`).

---

## 6. Guía de Despliegue en la Nube (Microsoft Azure)

### 6.1 Justificación de Proveedores Gratuitos Elegidos
- **Hosting de Aplicaciones**: **Microsoft Azure App Service (Plan F1 Free)**. Permite alojar aplicaciones web y APIs en **.NET 10** de forma nativa con certificado HTTPS gratuito (`*.azurewebsites.net`) e integración directa desde Visual Studio o GitHub Actions.
- **Base de Datos en la Nube**: **Azure SQL Database (Serverless Free Offer)**. Se eligió porque es el motor SQL Server oficial en la nube de Microsoft, lo que permite conservar el proveedor `Microsoft.EntityFrameworkCore.SqlServer` sin modificar la capa `VetCare.Datos` y ejecutar exactamente los mismos scripts T-SQL (`VetCareDb.sql`, `Usuarios.sql` y `AsignarRolesUsuarios.sql`).

### 6.2 Pasos para Publicar en Azure
1. **Crear la Base de Datos en Azure SQL**:
   - En Azure Portal crear un recurso **SQL Database** (activando el *Free Offer* / Serverless) llamado `VetCareDb`.
   - En **Networking / Firewall** del servidor SQL, activar *"Allow Azure services and resources to access this server"* y agregar tu IP actual del cliente.
   - Conectarse desde SSMS 2022 o el *Query Editor* de Azure Portal y ejecutar `database/VetCareDb.sql` y `database/Usuarios.sql`.
2. **Publicar los 3 Proyectos en Azure App Service**:
   - Crear 3 Web Apps (Plan **F1 Free** o Plan compartido):
     1. `vetcare-api-s15` (Microservicio 1: `VetCare.Api`)
     2. `vetcare-veterinarios-s15` (Microservicio 2: `VetCare.VeterinariosApi`)
     3. `vetcare-gateway-s15` (API Gateway: `GatewayApi`)
   - Antes de publicar `GatewayApi`, copiar el contenido de `GatewayApi/ocelot.azure.json` en `GatewayApi/ocelot.json` colocando los nombres exactos `.azurewebsites.net` de tus tres Web Apps (`DownstreamScheme: "https"`, `Port: 443`).
3. **Configurar Secretos y Variables de Entorno en Azure Portal**:
   - En cada una de las **3 Web Apps** (`Configuración` -> `Variables de entorno` / `App Settings`), agregar:
     - `Jwt__Key` = `<tu-clave-secreta-de-minimo-32-caracteres>` (exactamente la misma en las 3 apps).
   - En la Web App del **Microservicio 1 (`VetCare.Api`)** (`Cadenas de conexión` / `Connection Strings`), agregar:
     - Nombre: `VetCareConnection` (Tipo: `SQLAzure`), Valor: la cadena de conexión de tu Azure SQL Database.

---

## 7. Respuestas a las Preguntas de Sustentación

### 1. ¿Qué cambia para una aplicación cliente al usar el gateway en lugar de llamar directamente a cada servicio?
Para la aplicación cliente (frontend web, app móvil o Postman) **desaparece la complejidad de conocer múltiples direcciones, dominios y puertos**. En lugar de saber que Propietarios/Mascotas viven en el puerto `5169` y Veterinarios en el puerto `5170`, el cliente únicamente conoce y consume una sola URL base: la del API Gateway (`http://localhost:5168` o `https://vetcare-gateway-s15.azurewebsites.net`). Además, si en el futuro dividimos un microservicio, cambiamos su puerto o lo migramos a otro servidor, solo se actualiza `ocelot.json` en el Gateway sin romper ni recompilar la aplicación cliente.

### 2. En tu tabla de rutas elige una ruta y explica su upstream y su downstream: qué parte cambia entre uno y otro y por qué.
Tomemos la ruta del Microservicio 2:
- **Upstream**: `GET /gateway/veterinarios{everything}` en el puerto `5168` (`GatewayApi`).
- **Downstream**: `GET /api/veterinarios{everything}` hacia `http://localhost:5170` (o `https://vetcare-veterinarios-s15.azurewebsites.net:443` en la nube).
- **Qué cambia y por qué**:
  1. Cambia el **host y puerto de destino** (del Gateway `:5168` al Microservicio 2 `:5170`), porque el Gateway actúa como proxy inverso que enruta la solicitud al proceso donde realmente se ejecuta el servicio de veterinarios.
  2. Cambia el **prefijo de la ruta** de `/gateway/...` a `/api/...`, desacoplando la URL pública expuesta al consumidor externo de la estructura interna de controladores (`[Route("api/veterinarios")]`) del microservicio.
  3. La parte `{everything}` se conserva igual en ambos lados como comodín para que una sola regla en `ocelot.json` sirva tanto para listar (`/gateway/veterinarios`), filtrar (`/gateway/veterinarios?especialidad=Cirugía`) como para buscar por ID (`/gateway/veterinarios/1`).

### 3. ¿Dónde se valida el token en tu solución? ¿Por qué se valida en el gateway y también en los servicios, y qué desventaja trae repetirlo?
- **Dónde se valida**: El token JWT se valida en **dos niveles**: primero en la entrada del **API Gateway (`GatewayApi`)** mediante `AuthenticationOptions` con el esquema `"Bearer"`, y luego nuevamente dentro de cada **microservicio (`VetCare.Api` y `VetCare.VeterinariosApi`)** mediante su middleware `UseAuthentication()` y `UseAuthorization()`.
- **Por qué se valida en ambos (Defensa en profundidad)**:
  1. Validar en el **Gateway** permite rechazar peticiones sin token (`401 Unauthorized`) en el borde de la arquitectura antes de gastar ancho de red o recursos de los microservicios internos.
  2. Validar también en los **microservicios** evita que un atacante o proceso que logre acceder directamente a la dirección del microservicio (saltándose el Gateway) pueda consumir los datos sin autenticarse (principio *Zero Trust*), y además permite que cada microservicio lea los *claims* del token (como el claim `"rol"`) para aplicar autorización específica (`403 Forbidden` entre `Recepcionista` y `Veterinario`).
- **Desventaja de repetirlo**:
  Introduce **acoplamiento de configuración y sobrecarga operativa**: los tres proyectos deben mantener sincronizados la misma clave secreta (`Jwt:Key`), emisor (`Issuer`) y audiencia (`Audience`). Si se rota la clave JWT, hay que actualizarla simultáneamente en el Gateway y en todos los microservicios; además, la firma criptográfica del token se verifica dos veces por cada petición.

### 4. Al detener el microservicio 2, ¿qué respondió el gateway y qué siguió funcionando? ¿Qué desventaja de los microservicios (complejidad, autenticación o transacciones) notaste al hacer este laboratorio?
- **Qué respondió el Gateway y qué siguió funcionando**:
  Al apagar `VetCare.VeterinariosApi` (`:5170`) e invocar `GET /gateway/veterinarios`, el API Gateway respondió **`502 Bad Gateway`**, indicando que como puerta de enlace recibió la petición válidamente pero no pudo conectarse con el servidor downstream. Sin embargo, **todo el Microservicio 1 (`VetCare.Api` en `:5169`) siguió funcionando con total normalidad** a través del Gateway (login, consulta y registro de propietarios y mascotas), demostrando el **aislamiento de fallos** propio de los microservicios.
- **Desventaja de los microservicios notada en el laboratorio**:
  Se evidenció claramente el aumento de **complejidad operativa y de configuración distribuida**: en lugar de compilar, configurar secretos y desplegar una sola aplicación monolítica, ahora es necesario levantar 3 procesos en puertos distintos, mantener sincronizada la clave JWT en 3 servicios independientes, gestionar el enrutamiento en `ocelot.json` (diferenciando entre `localhost` y las URLs de Azure) y diagnosticar fallos de red entre procesos como el `502 Bad Gateway`.

---

## 8. Observaciones y Conclusiones

1. **Punto único de entrada y desacoplamiento**: La implementación de **Ocelot** como API Gateway permitió unificar bajo un solo puerto (`5168` en local / `443` en Azure) el acceso a dos microservicios completamente independientes (`VetCare.Api` con persistencia en SQL Server y `VetCare.VeterinariosApi` con catálogo en memoria), utilizando comodines `{everything}` para simplificar la tabla de rutas.
2. **Seguridad distribuida (JWT + Roles)**: La separación de responsabilidades de seguridad demostró ser efectiva: Ocelot actúa como primera barrera verificando la autenticidad y vigencia del token JWT (`401 Unauthorized`), mientras que los microservicios aplican defensa en profundidad y validan las reglas de autorización por rol (`403 Forbidden` para `Recepcionista` y `Veterinario`).
3. **Resiliencia y tolerancia a fallos**: Las pruebas del código `502 Bad Gateway` comprobaron que la caída del microservicio de veterinarios no afecta la disponibilidad del microservicio principal de propietarios y mascotas.
4. **Alineación con Microsoft Azure**: El uso de **Azure SQL Database** y **Azure App Service** permitió llevar la arquitectura a la nube manteniendo el motor relacional SQL Server sin alterar la capa de acceso a datos de Entity Framework Core, gestionando las credenciales de forma segura mediante variables de entorno fuera del repositorio Git.
