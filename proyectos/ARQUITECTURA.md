# 🏛️ Arquitectura - Diccionarios App

Este documento contiene los diagramas de clases y componentes del proyecto.

---

## 📦 Diagramas de Clases por Componente

### Diccionarios.Api

Interfaces base del dominio:

```mermaid
classDiagram
    class IProveedorDiccionarios {
        <<interface>>
        +ObtenerIdiomas() IEnumerable~IIdioma~
        +ObtenerDiccionario(codigo) IDiccionario
    }
    
    class IIdioma {
        <<interface>>
        +Codigo string
        +Nombre string
    }
    
    class IDiccionario {
        <<interface>>
        +Idioma IIdioma
        +Buscar(palabra) IEnumerable~string~
    }
    
    IProveedorDiccionarios ..> IIdioma : devuelve
    IProveedorDiccionarios ..> IDiccionario : devuelve
    IDiccionario --> IIdioma : tiene
```

---

### Diccionarios.Ficheros

Implementación basada en archivos .txt:

```mermaid
classDiagram
    class IProveedorDiccionarios {
        <<interface>>
    }
    
    class IDiccionario {
        <<interface>>
    }
    
    class IIdioma {
        <<interface>>
    }
    
    class ProveedorDiccionariosFicheros {
        -rutaBase: string
        +ObtenerIdiomas() IEnumerable~IIdioma~
        +ObtenerDiccionario(codigo) IDiccionario
    }
    
    class DiccionarioFichero {
        -rutaFichero: string
        -idioma: IIdioma
        +Idioma IIdioma
        +Buscar(palabra) IEnumerable~string~
    }
    
    ProveedorDiccionariosFicheros ..|> IProveedorDiccionarios
    DiccionarioFichero ..|> IDiccionario
    ProveedorDiccionariosFicheros --> DiccionarioFichero : crea
```

---

### Diccionarios.BBDD

Implementación con Entity Framework + SQLite:

```mermaid
classDiagram
    class IProveedorDiccionarios {
        <<interface>>
    }
    
    class IDiccionario {
        <<interface>>
    }
    
    class IIdioma {
        <<interface>>
    }
    
    class ProveedorDiccionariosBBDD {
        -dbContext: DiccionariosDbContext
        +ObtenerIdiomas() IEnumerable~IIdioma~
        +ObtenerDiccionario(codigo) IDiccionario
    }
    
    class DiccionarioBBDD {
        -dbContext: DiccionariosDbContext
        -idiomaEntity: IdiomaEntity
        +Idioma IIdioma
        +Buscar(palabra) IEnumerable~string~
    }
    
    class IdiomaBBDD {
        +Codigo string
        +Nombre string
    }
    
    class DiccionariosDbContext {
        +Idiomas DbSet~IdiomaEntity~
        +Diccionarios DbSet~DiccionarioEntity~
        +Palabras DbSet~PalabraEntity~
        +Significados DbSet~SignificadoEntity~
    }
    
    class DatabaseInitializer {
        +Initialize()
    }
    
    ProveedorDiccionariosBBDD ..|> IProveedorDiccionarios
    DiccionarioBBDD ..|> IDiccionario
    IdiomaBBDD ..|> IIdioma
    
    ProveedorDiccionariosBBDD --> DiccionariosDbContext
    ProveedorDiccionariosBBDD --> DiccionarioBBDD : crea
    ProveedorDiccionariosBBDD --> IdiomaBBDD : crea
    DiccionarioBBDD --> DiccionariosDbContext
    DatabaseInitializer --> DiccionariosDbContext
```

#### Entidades de Base de Datos

```mermaid
classDiagram
    class IdiomaEntity {
        +Id int
        +Codigo string
        +Nombre string
        +Diccionario DiccionarioEntity
    }
    
    class DiccionarioEntity {
        +Id int
        +IdiomaId int
        +Idioma IdiomaEntity
        +Palabras List~PalabraEntity~
    }
    
    class PalabraEntity {
        +Id int
        +Texto string
        +DiccionarioId int
        +Diccionario DiccionarioEntity
        +Significados List~SignificadoEntity~
    }
    
    class SignificadoEntity {
        +Id int
        +Definicion string
        +PalabraId int
        +Palabra PalabraEntity
    }
    
    IdiomaEntity "1" -- "1" DiccionarioEntity
    DiccionarioEntity "1" -- "*" PalabraEntity
    PalabraEntity "1" -- "*" SignificadoEntity
```

---

### Servicio.Api

Contrato del servicio de aplicación:

```mermaid
classDiagram
    class IServicio {
        <<interface>>
        +ObtenerIdiomas() IEnumerable~IdiomaDTO~
        +ObtenerDiccionario(codigo) DiccionarioDTO
        +Buscar(codigo, palabra) IEnumerable~SignificadoDTO~
    }
    
    class IdiomaDTO {
        +Codigo string
        +Nombre string
    }
    
    class DiccionarioDTO {
        +Idioma IdiomaDTO
    }
    
    class SignificadoDTO {
        +Definicion string
    }
    
    IServicio ..> IdiomaDTO : devuelve
    IServicio ..> DiccionarioDTO : devuelve
    IServicio ..> SignificadoDTO : devuelve
    DiccionarioDTO --> IdiomaDTO
```

---

### Servicio.Impl

Implementación con patrón Decorator para AOP:

```mermaid
classDiagram
    class IServicio {
        <<interface>>
    }
    
    class ServicioImpl {
        -proveedor: IProveedorDiccionarios
        -mapper: IMapper
        +ObtenerIdiomas() IEnumerable~IdiomaDTO~
        +ObtenerDiccionario(codigo) DiccionarioDTO
        +Buscar(codigo, palabra) IEnumerable~SignificadoDTO~
    }
    
    class ServicioLoggingDecorator {
        -inner: IServicio
        -logger: ILogger
        +ObtenerIdiomas() IEnumerable~IdiomaDTO~
        +ObtenerDiccionario(codigo) DiccionarioDTO
        +Buscar(codigo, palabra) IEnumerable~SignificadoDTO~
    }
    
    class DiccionariosMapperProfile {
        <<AutoMapper Profile>>
    }
    
    ServicioImpl ..|> IServicio
    ServicioLoggingDecorator ..|> IServicio
    ServicioLoggingDecorator --> IServicio : decora
    ServicioImpl --> IProveedorDiccionarios : usa
    
    note for ServicioLoggingDecorator "Patrón Decorator\npara logging AOP\n(registrado con Scrutor)"
```

---

### Rest.V1.Api

DTOs para la API REST:

```mermaid
classDiagram
    class IdiomaRestV1DTO {
        +Codigo string
        +Nombre string
    }
    
    class DiccionarioRestV1DTO {
        +Idioma IdiomaRestV1DTO
    }
    
    class SignificadoRestV1DTO {
        +Definicion string
    }
    
    DiccionarioRestV1DTO --> IdiomaRestV1DTO
```

---

### Rest.V1.Impl

Controllers de la API REST:

```mermaid
classDiagram
    class ControllerBase {
        <<ASP.NET Core>>
    }
    
    class IdiomasController {
        -servicio: IServicio
        -mapper: IMapper
        +Get() ActionResult~IEnumerable~
    }
    
    class DiccionariosController {
        -servicio: IServicio
        -mapper: IMapper
        +Get(codigo) ActionResult~DiccionarioRestV1DTO~
        +Buscar(codigo, palabra) ActionResult~IEnumerable~
    }
    
    class RestV1MapperProfile {
        <<AutoMapper Profile>>
    }
    
    IdiomasController --|> ControllerBase
    DiccionariosController --|> ControllerBase
    IdiomasController --> IServicio
    DiccionariosController --> IServicio
```

---

### UI.Api + UI.Consola

Interfaz de usuario:

```mermaid
classDiagram
    class IUIApp {
        <<interface>>
        +Run(args) void
    }
    
    class UIConsola {
        -servicio: IServicio
        +Run(args) void
    }
    
    UIConsola ..|> IUIApp
    UIConsola --> IServicio : usa
```

---

## 🎯 Diagramas de Componentes por Aplicación

### App.Consola

Aplicación mínima sin inyección de dependencias:

```mermaid
flowchart TB
    subgraph App.Consola
        Program[Program.cs]
    end
    
    subgraph Diccionarios.Ficheros
        ProveedorF[ProveedorDiccionariosFicheros]
        DiccionarioF[DiccionarioFichero]
    end
    
    subgraph Diccionarios.Api
        IProvD[IProveedorDiccionarios]
        IDic[IDiccionario]
    end
    
    Program --> ProveedorF
    ProveedorF -.->|implementa| IProvD
    DiccionarioF -.->|implementa| IDic
    ProveedorF --> DiccionarioF
    
    style App.Consola fill:#e1f5fe
    style Diccionarios.Ficheros fill:#fff3e0
    style Diccionarios.Api fill:#f3e5f5
```

---

### App.ConsolaConDI

Aplicación con inyección de dependencias manual:

```mermaid
flowchart TB
    subgraph App.ConsolaConDI
        Program[Program.cs]
        DIConfig[DependencyInjectionConfig]
    end
    
    subgraph UI.Consola
        UIConsola[UIConsola]
    end
    
    subgraph UI.Api
        IUIApp[IUIApp]
    end
    
    subgraph Servicio.Impl
        ServImpl[ServicioImpl]
    end
    
    subgraph Servicio.Api
        IServ[IServicio]
    end
    
    subgraph Diccionarios.Ficheros
        ProveedorF[ProveedorDiccionariosFicheros]
    end
    
    subgraph Diccionarios.Api
        IProvD[IProveedorDiccionarios]
    end
    
    Program --> DIConfig
    DIConfig --> UIConsola
    DIConfig --> ServImpl
    DIConfig --> ProveedorF
    
    UIConsola -.->|implementa| IUIApp
    ServImpl -.->|implementa| IServ
    ProveedorF -.->|implementa| IProvD
    
    UIConsola --> IServ
    ServImpl --> IProvD
    
    style App.ConsolaConDI fill:#e1f5fe
    style UI.Consola fill:#c8e6c9
    style Servicio.Impl fill:#ffecb3
    style Diccionarios.Ficheros fill:#fff3e0
```

---

### App.Host

Aplicación con Microsoft.Extensions.Hosting y auto-discovery:

```mermaid
flowchart TB
    subgraph App.Host
        Program[Program.cs]
        Extensions[ServiceCollectionExtensions]
        Aplicacion[Aplicacion]
        Config[appsettings.json]
    end
    
    subgraph UI.Consola
        UIConsola[UIConsola]
    end
    
    subgraph UI.Api
        IUIApp[IUIApp]
    end
    
    subgraph Servicio.Impl
        ServImpl[ServicioImpl]
        Decorator[ServicioLoggingDecorator]
    end
    
    subgraph Servicio.Api
        IServ[IServicio]
    end
    
    subgraph Diccionarios.BBDD
        ProveedorBBDD[ProveedorDiccionariosBBDD]
        DbContext[DiccionariosDbContext]
    end
    
    subgraph Diccionarios.Api
        IProvD[IProveedorDiccionarios]
    end
    
    Program --> Extensions
    Program --> Config
    Extensions -->|auto-discovery| UIConsola
    Extensions -->|auto-discovery| ServImpl
    Extensions -->|Scrutor| Decorator
    Extensions -->|auto-discovery| ProveedorBBDD
    
    Aplicacion --> IUIApp
    
    UIConsola -.->|implementa| IUIApp
    ServImpl -.->|implementa| IServ
    Decorator -.->|decora| IServ
    ProveedorBBDD -.->|implementa| IProvD
    ProveedorBBDD --> DbContext
    
    style App.Host fill:#e1f5fe
    style Servicio.Impl fill:#ffecb3
    style Diccionarios.BBDD fill:#ffcdd2
```

---

### App.WebApi

API REST con NSwag/Swagger:

```mermaid
flowchart TB
    subgraph App.WebApi
        Program[Program.cs]
        Config[appsettings.json]
    end
    
    subgraph Rest.V1.Impl
        IdiomasCtrl[IdiomasController]
        DiccionariosCtrl[DiccionariosController]
        RestMapper[RestV1MapperProfile]
    end
    
    subgraph Rest.V1.Api
        RestDTOs[DTOs REST]
    end
    
    subgraph Servicio.Impl
        ServImpl[ServicioImpl]
        Decorator[ServicioLoggingDecorator]
        ServMapper[DiccionariosMapperProfile]
    end
    
    subgraph Servicio.Api
        IServ[IServicio]
        ServDTOs[DTOs Servicio]
    end
    
    subgraph Diccionarios.BBDD
        ProveedorBBDD[ProveedorDiccionariosBBDD]
        DbContext[DiccionariosDbContext]
        DBInit[DatabaseInitializer]
    end
    
    subgraph Diccionarios.Api
        IProvD[IProveedorDiccionarios]
    end
    
    subgraph External
        Swagger[Swagger UI]
        SQLite[(SQLite DB)]
    end
    
    Program --> IdiomasCtrl
    Program --> DiccionariosCtrl
    Program --> Config
    
    IdiomasCtrl --> IServ
    DiccionariosCtrl --> IServ
    IdiomasCtrl --> RestDTOs
    DiccionariosCtrl --> RestDTOs
    
    ServImpl -.->|implementa| IServ
    Decorator -.->|decora| IServ
    ServImpl --> IProvD
    ServImpl --> ServDTOs
    
    ProveedorBBDD -.->|implementa| IProvD
    ProveedorBBDD --> DbContext
    DBInit --> DbContext
    DbContext --> SQLite
    
    Program --> Swagger
    
    style App.WebApi fill:#e1f5fe
    style Rest.V1.Impl fill:#dcedc8
    style Servicio.Impl fill:#ffecb3
    style Diccionarios.BBDD fill:#ffcdd2
    style External fill:#cfd8dc
```

---

## 🔄 Flujo de Dependencias Global

```mermaid
flowchart LR
    subgraph Apps
        A1[Consola]
        A2[ConsolaConDI]
        A3[Host]
        A4[WebApi]
    end
    
    subgraph UI
        UI1[UI.Consola]
        UI2[UI.Api]
    end
    
    subgraph REST
        R1[Rest.V1.Impl]
        R2[Rest.V1.Api]
    end
    
    subgraph Servicio
        S1[Servicio.Impl]
        S2[Servicio.Api]
    end
    
    subgraph Diccionarios
        D1[Diccionarios.Ficheros]
        D2[Diccionarios.BBDD]
        D3[Diccionarios.Api]
    end
    
    A1 --> D1
    A2 --> UI1 --> S1 --> D1
    A3 --> UI1 --> S1 --> D2
    A4 --> R1 --> S1 --> D2
    
    UI1 -.-> UI2
    UI1 -.-> S2
    R1 -.-> R2
    R1 -.-> S2
    S1 -.-> S2
    S1 -.-> D3
    D1 -.-> D3
    D2 -.-> D3
    
    style Apps fill:#e1f5fe
    style REST fill:#dcedc8
    style Servicio fill:#ffecb3
    style Diccionarios fill:#fff3e0
```

---

## 📋 Leyenda

| Símbolo | Significado |
|---------|-------------|
| `-->` | Dependencia directa |
| `-.->` | Implementa interfaz |
| `<<interface>>` | Interfaz |
| Colores por capa | Apps (azul), REST (verde), Servicio (amarillo), Diccionarios (naranja/rojo) |
