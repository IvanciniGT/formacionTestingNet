# 📚 Diccionarios App

Aplicación multiidioma de diccionarios desarrollada en **.NET 10** siguiendo principios **SOLID**, patrones de diseño y buenas prácticas de arquitectura.

## 🏗️ Arquitectura

El proyecto sigue una arquitectura en capas con separación clara de responsabilidades.

👉 **Ver diagramas completos en [ARQUITECTURA.md](ARQUITECTURA.md)** (clases, componentes, Mermaid)

```
┌─────────────────────────────────────────────────────────────┐
│                        APPS                                  │
│  ┌─────────┐ ┌──────────────┐ ┌────────┐ ┌────────────────┐ │
│  │ Consola │ │ ConsolaConDI │ │  Host  │ │     WebApi     │ │
│  └────┬────┘ └──────┬───────┘ └───┬────┘ └───────┬────────┘ │
└───────┼─────────────┼─────────────┼──────────────┼──────────┘
        │             │             │              │
┌───────┴─────────────┴─────────────┴──────────────┴──────────┐
│                         REST                                 │
│              ┌─────────────┬─────────────┐                  │
│              │   V1/Api    │   V1/Impl   │                  │
│              │   (DTOs)    │(Controllers)│                  │
│              └──────┬──────┴──────┬──────┘                  │
└─────────────────────┼─────────────┼─────────────────────────┘
                      │             │
┌─────────────────────┴─────────────┴─────────────────────────┐
│                       SERVICIO                               │
│    ┌─────────┐    ┌──────────┐    ┌───────────────────┐     │
│    │   Api   │◄───│   Impl   │◄───│ LoggingDecorator  │     │
│    │(IServ.) │    │(Servicio)│    │      (AOP)        │     │
│    └────┬────┘    └────┬─────┘    └───────────────────┘     │
└─────────┼──────────────┼────────────────────────────────────┘
          │              │
┌─────────┴──────────────┴────────────────────────────────────┐
│                     DICCIONARIOS                             │
│  ┌─────────┐    ┌──────────────┐    ┌──────────────────┐    │
│  │   Api   │◄───│   Ficheros   │    │       BBDD       │    │
│  │(Interf.)│◄───│(ProveedorF.) │    │  (ProveedorBBDD) │    │
│  └─────────┘    └──────────────┘    └──────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

## 📁 Estructura de Proyectos

```
proyectos/
├── Diccionarios/
│   ├── Api/                    # Interfaces: IDiccionario, IIdioma, IProveedorDiccionarios
│   ├── Ficheros/               # Implementación desde archivos .txt
│   ├── Ficheros.Tests/         # Tests unitarios
│   ├── BBDD/                   # Implementación con Entity Framework + SQLite
│   └── BBDD.Tests/             # Tests unitarios
│
├── Servicio/
│   ├── Api/                    # IServicio + DTOs
│   ├── Impl/                   # ServicioImpl + ServicioLoggingDecorator (AOP)
│   └── Tests/                  # Tests unitarios con Moq
│
├── Rest/V1/
│   ├── Api/                    # DTOs para REST API
│   └── Impl/                   # Controllers + Mappers
│
├── UI/
│   ├── Api/                    # IUIApp
│   └── Consola/                # UIConsola
│
├── Apps/
│   ├── Consola/                # App básica sin DI
│   ├── ConsolaConDI/           # App con inyección de dependencias manual
│   ├── Host/                   # App con Microsoft.Extensions.Hosting + auto-discovery
│   └── WebApi/                 # API REST con NSwag + Swagger
│
└── DiccionariosApp.sln
```

## � Compilar

```bash
# Restaurar dependencias
dotnet restore

# Compilar toda la solución
dotnet build

# Compilar en modo Release
dotnet build -c Release
```

## 🚀 Ejecutar

### Aplicaciones de Consola

```bash
# App básica (sin inyección de dependencias)
dotnet run --project Apps/Consola/App.Consola.csproj -- ES melón

# App con DI manual
dotnet run --project Apps/ConsolaConDI/App.ConsolaConDI.csproj -- ES melón

# App con Host (IoC completo)
cd Apps/Host && dotnet run -- ES melón
```

### API REST

```bash
cd Apps/WebApi && dotnet run
```

**Endpoints disponibles:**
- `GET /api/v1/idiomas` - Lista de idiomas
- `GET /api/v1/diccionarios/{codigo}` - Obtener diccionario
- `GET /api/v1/diccionarios/{codigo}/buscar?palabra=X` - Buscar palabra

**Swagger UI:** http://localhost:5161/swagger

## 🧪 Tests

```bash
# Ejecutar todos los tests (34 tests)
dotnet test

# Tests por proyecto individual
dotnet test Diccionarios/Ficheros.Tests/Diccionarios.Ficheros.Tests.csproj
dotnet test Diccionarios/BBDD.Tests/Diccionarios.BBDD.Tests.csproj
dotnet test Servicio/Tests/Servicio.Tests.csproj

# Con salida detallada
dotnet test --logger "console;verbosity=detailed"

# Con cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

**34 tests** distribuidos en:
| Proyecto | Tests |
|----------|-------|
| `Diccionarios.Ficheros.Tests` | 13 |
| `Diccionarios.BBDD.Tests` | 13 |
| `Servicio.Tests` | 8 |

## 🛠️ Tecnologías

| Categoría | Tecnología |
|-----------|------------|
| Framework | .NET 10 |
| Testing | xUnit + Moq |
| ORM | Entity Framework Core + SQLite |
| Mapping | AutoMapper |
| DI Decorator | Scrutor |
| API Docs | NSwag (OpenAPI/Swagger) |
| Logging | Microsoft.Extensions.Logging |

## 📐 Patrones y Principios

- **SOLID** - Especialmente Dependency Inversion
- **Decorator** - AOP para logging (ServicioLoggingDecorator)
- **Repository** - Abstracción de acceso a datos
- **Adapter** - Implementaciones intercambiables (Ficheros ↔ BBDD)
- **Clean Architecture** - Capas desacopladas con dependencias hacia el centro

## 🔧 Configuración

### WebApi (`Apps/WebApi/appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=diccionarios.db"
  }
}
```

### Host (`Apps/Host/appsettings.json`)
```json
{
  "DiccionariosConfig": {
    "ConnectionString": "Data Source=diccionarios.db"
  }
}
```

## 📝 Ejemplo de Uso

```csharp
// Inyección de dependencias con Scrutor (AOP)
services.AddScoped<IServicio, ServicioImpl>();
services.Decorate<IServicio, ServicioLoggingDecorator>();

// El decorator envuelve automáticamente y añade logging
// sin modificar el código de ServicioImpl
```

## 📄 Licencia

MIT
