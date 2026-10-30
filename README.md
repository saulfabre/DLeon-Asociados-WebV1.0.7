# DLeon Asociados Web

Sistema web para la gestión y administración de vehículos, seguros, garantías, clientes y repuestos de DLeon Asociados. La aplicación está desarrollada con ASP.NET Core 10, Blazor Server y Entity Framework Core, y usa SQL Server como motor de persistencia.

## Descripción general

Este proyecto permite gestionar el catálogo de vehículos disponibles, registrar clientes, asociar seguros y garantías, controlar repuestos y mantener una base de datos centralizada para operaciones de negocio del concesionario.

## Tecnologías

- ASP.NET Core 10
- Blazor Server
- Entity Framework Core
- SQL Server
- Razor Components

## Funcionalidades principales

- Catálogo y administración de vehículos
- Registro y consulta de clientes
- Gestión de seguros por vehículo y cliente
- Control de garantías
- Administración de repuestos
- Búsqueda y filtros por marca, modelo, tipo, año y precio
- Persistencia con base de datos relacional

## Estructura del proyecto

- `Components/`: páginas y layouts de la interfaz Blazor
- `Context/`: configuración del contexto de Entity Framework
- `Models/`: entidades del dominio
- `Services/`: lógica de negocio y acceso a datos
- `Database/`: archivos y recursos relacionados con la base de datos
- `Migrations/`: migraciones de Entity Framework
- `wwwroot/`: contenido estático del frontend
- `Program.cs`: configuración de la aplicación

## Requisitos previos

- .NET SDK 10
- SQL Server disponible localmente o en red
- Git

## Configuración de la base de datos

La aplicación usa la cadena de conexión `SqlConStr` definida en `appsettings.json` o en secretos locales.

Ejemplo con `dotnet user-secrets`:

```bash
dotnet user-secrets set "ConnectionStrings:SqlConStr" "Server=(localdb)\\MSSQLLocalDB;Database=DLeonAsociadosDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

## Ejecución

1. Clona el repositorio.
2. Restaura dependencias:

```bash
dotnet restore
```

3. Aplica las migraciones de la base de datos (si aplica en tu entorno):

```bash
dotnet ef database update
```

4. Ejecuta la aplicación:

```bash
dotnet run
```

La aplicación quedará disponible típicamente en:

```text
https://localhost:5001
```

## Notas

- El proyecto está pensado para uso empresarial interno y puede requerir ajustes según el entorno de despliegue y la estructura real de la base de datos.
- Si el proyecto se ejecuta con SQL Server remoto, verifica que la cadena de conexión y las credenciales sean correctas antes de lanzar la aplicación.

## Licencia

Este proyecto se entrega con fines de desarrollo y administración interna del negocio. Ajusta la licencia según el uso final que le des.
