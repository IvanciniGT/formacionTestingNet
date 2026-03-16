
# Crear solución:

$ dotnet new sln -n PruebasBasico 
Eso genera el fichero PruebasBasico.sln o PruebasBasico.slnx dependiendo de la versión de .NET que tengamos instalada. Desde 10 -> .slnx
El fichero inicialmente vacío.

# Crear proyectos:

$ dotnet new classlib -n LibreriaMatematica -o LibreriaMatematica
$ dotnet new nunit -n LibreriaMatematica.Test -o LibreriaMatematica.Test
$ dotnet new xunit -n LibreriaMatematica.Test2 -o LibreriaMatematica.Test2

Esos crean una carpeta para el proyecto, dentro:
- un fichero de proyecto: .csproj (casi vacío)
- un fichero de clase: Class1.cs o UnitTest1.cs dependiendo del tipo de proyecto.
- En los de pruebas, además, dentro del .csproj ya incluyen las referencias a los paquetes necesarios para cada tipo de proyecto.

Esos comandos también tratan de descargar esos paquetes de Nuget, lo que puede dar un error si estamos detrás de un proxy.

# Añadir los proyectos a la solución:

$ dotnet sln PruebasBasico.slnx add LibreriaMatematica/LibreriaMatematica.csproj
$ dotnet sln PruebasBasico.sln add LibreriaMatematica/LibreriaMatematica.csproj

$ dotnet sln PruebasBasico.slnx add LibreriaMatematica.Test/LibreriaMatematica.Test.csproj
$ dotnet sln PruebasBasico.sln add LibreriaMatematica.Test/LibreriaMatematica.Test.csproj

Con x o sin ella, dependiendo de la versión de .NET que tengamos instalada, el resultado es el mismo. El proyecto se añade a la solución.

# Añadir dependencias entre proyectos.
El proyecto de .Test va a probar el otro proyecto. Necesita acceder a su código:
$ dotnet add LibreriaMatematica.Test/LibreriaMatematica.Test.csproj reference LibreriaMatematica/LibreriaMatematica.csproj


# Compilamos solución

dotnet build PruebasBasico.slnx 
dotnet build PruebasBasico.sln

# Ejecutar pruebas del proyecto de pruebas 1:

dotnet test LibreriaMatematica.Test/LibreriaMatematica.Test.csproj 


# Creo nuevo proyecto: AppConsola

$ dotnet new console -n AppConsola -o AppConsola
$ dotnet sln PruebasBasico.slnx add AppConsola/AppConsola.csproj
$ dotnet sln PruebasBasico.sln add AppConsola/AppConsola.csproj

Y le meto dependencia a la librería matemática:
$ dotnet add AppConsola/AppConsola.csproj reference LibreriaMatematica/LibreriaMatematica.csproj


# Compilo todo:
dotnet build PruebasBasico.slnx

# Ejecuta el proyecto de consola:
dotnet run --project AppConsola/AppConsola.csproj


Que pruebas le puedo hacer autoamtizadas al proyecto de consola?
NINGUNA!, al menos ninguna desde C# con un framework de pruebas

Al meter el código en ua clase con una función, ya puedo hacer pruebas... ya tengo alguien a quien llamar: LogicaDelPrograma.ejecutar() y puedo hacer pruebas a esa función.

    LogicaDelPrograma --> LibreriaMatematica

    Qué tipo de pruebas hago?
    - Unitarias? SI
    - Integración? SI
    - Sistema? Si.. pero en este caso, las pruebas de sistema son igual a las de INTEGRACION.

    Qué tipos de pruebas puedo hacer?
    - Unitarias? NO, porque el componente LogicaDelPrograma esta TOTALMENTE SOLDADO a la librería matemática.
    - Puedo dessoldarlo? Puedo decirle que use en lugar de la Librería matemática, una simulación de la librería matemática de mentirijilla, que sepa que no falla? NO
    - Tal y como está solo podría. hacer pruebas de integreación = pruebas de sistema. 

// Al aplicar un patrón de inyección de dependencias, ya puedo hacer pruebas unitarias, porque puedo dessoldar el componente de la librería matemática y meterle una simulación de la librería matemática que sepa que no falla. Y entonces ya puedo hacer pruebas unitarias a la lógica del programa, sin que me fallen por culpa de la librería matemática.