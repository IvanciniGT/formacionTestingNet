

SOLUCION: PruebasBasico

    PROYECTO: LibreriaMatematica

    PROYECTO: LibreriaMatematica.Test           <-- NUNIT
    
    PROYECTO: LibreriaMatematica.Test2          <-- XUNIT


Una cosa son los archivos de la solución : .slnx
Y los archivos de proyecto: .csproj

Y otra cosa es poner en marcha los proyectos, compilarlos etc.

Con los comandos dotnet new aourren varias cosas:
1. Crea unos archivos de solución y de proyecto
2. En concreto, los de proyecto los crea mediante una plantilla:
    "classlib"
    "nunit" o "xunit" dependiendo del tipo de proyecto que hayamos elegido. 
    Eso son plantillas. Esas plantillas lo que incluyen es una clase por defecto: Class1.cs, o para los poryectos de prueba: UnitTest1.cs, o para xunit: UnitTest1.cs.
    Y luego lo qye hacen también es meter en el fichero .csproj las referencias a los paquetes necesarios para cada tipo de proyecto.

    HASTA AQUI SE OS EJECUTA SIN PROBLEMA... eso ocurre todo en local => Creando 4 archivos de texto cutres!

3. Leen los ficehros .csproj y descargan las dependencias que se han declarado en el .csproj. Es decir, descargan los paquetes de Nuget que se han declarado en el .csproj.

    EL PROBLEMA OS DA AQUI! Porque aquí si trata de conectar con Internet para descargar los paquetes de Nuget. Y tenéis un proxy cuando conectaís por VPN que bloquea la conexión os dará un error.



---

SOLUCION: PruebasBasico

    PROYECTO: LibreriaMatematica
        ^
        |
    PROYECTO: LibreriaMatematica.Test           <-- NUNIT


Que pruebas son? 
- Unitarias = Sistema (componente)
- Unitarias = Componente
  Y son iguales porque el componente no tiene dependencias. Es decir, no depende de nada más. Es un componente aislado. 