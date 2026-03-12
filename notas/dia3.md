
App.Consola

    $ buscarPalabra "ES" "Melón"

    Buscar en los diccionarios que tenga configurados para ese idioma la palabra y devolver sus significados... caso que la palabra se encuentre.

        La palabra "Melón" existe en el diccionario de español y tiene los siguientes significados:
            - Fruto grande, redondo y de cáscara gruesa, que se cultiva en climas cálidos y se consume como alimento. 
            - Planta trepadora de la familia de las cucurbitáceas, que produce el melón.
            - Persona con pocas luces: Eres un melón, no te enteras de nada.

    $ buscarPalabra "ES" "Archilococo"

        La palabra "Archilococo" no existe en el diccionario de español.


Pregunta. Cuantos proyecto vamos a definir?

    - Gestor de diccionarios (API) + Implementacion a ficheros V1... má adelante veremos
    - UI Consola + UI API ... más adelante veremos
    - Lógica del programa API + Implementación (Procesar la petición, mostrar mensajes por pantalla...) + Lógica de negocio (Preguntar a los diccionarios por la palabra y el idioma...)
    - Programa: El que configura todo!
    - Test para :
      - Implementacion a ficheros del gestor de diccionarios
      - Test para la lógica del programa
      - Test para la UI de consola



```csharp
namespace App.Diccionarios.API ;


interface IDiccionario {

    bool existe(string palabra);

    IList<string>? getSignificados(string palabra);

}


interface ISuministradorDeDiccionarios {

    boolean tienesDiccionariosDe(string idioma);

    IList<IDiccionario>? getDiccionarios(string idioma);

}
```


Esa interface que tenemos es un desastre! Que SonarQube me va a escupir (y con razón) a la cara en cuanto la suba !
Nadie presente es capaz de decirme cómo comunicarme con esa función!
    - Qué le paso? Un string, la palabra
    - Qué me devuelve? Una lista de strings, los significados de la palabra. SEGURO? Me temo que No es claro!
        "Melón" -> ["Fruto grande, redondo y de cáscara gruesa, que se cultiva en climas cálidos y se consume como alimento.", "Planta trepadora de la 
        familia de las cucurbitáceas, que produce el melón.", "Persona con pocas luces: Eres un melón, no te enteras de nada."]

        "Archilococo"
            - null
            - Lista vacía
            - NotSuchWordException
               Esto es un desastre! Computacionalmente hablando. Generar una exception es CARO. Lo primero que necesito es hacer un volcado del ThreadStack. Para devolver que no existe una palabra?
               Las excepciones están para controlar casos excepcionales, no para controlar el flujo normal de la aplicación.
               Sirven para avisar de que una situación que potencialmente podía ocurrir pero que no sabíamos si iba a ocurrir hasta no intentarla (TRY) ha ocurrido. No para controlar el flujo normal de la aplicación.

V1: Los diccionarios los tendrewmos en ficheros:
    \diccionarios\diccionario_es.txt
    \diccionarios\diccionario_en.txt
          Melón=Fruto grande, redondo y de cáscara gruesa, que se cultiva en climas cálidos y se consume como alimento. | Planta trepadora de la familia de las cucurbitáceas, que produce el melón. | Persona con pocas luces: Eres un melón, no te enteras de nada.

V2: Los diccionarios los tendremos en una base de datos. 

```csharp

namespace App.Diccionarios.Implementaciones.Fichero ;

class DiccionarioDesdeFichero : IDiccionario {

    private string rutaFichero;

    public DiccionarioDesdeFichero(string rutaFichero) {
        this.rutaFichero = rutaFichero;
        // Proceso de carga del fichero y almacenamiento de su contenido en una estructura de datos adecuada para su posterior consulta
    }

    public bool existe(string palabra) {
        // Implementación para verificar si la palabra existe en el fichero
    }

    public IList<string>? getSignificados(string palabra) {
        // Implementación para obtener los significados de la palabra desde el fichero
    }
}

class SuministradorDeDiccionariosDesdeFichero : ISuministradorDeDiccionarios {

    private string rutaDirectorioDiccionarios;

    public SuministradorDeDiccionariosDesdeFichero(string rutaDirectorioDiccionarios) {
        this.rutaDirectorioDiccionarios = rutaDirectorioDiccionarios;
        // Proceso de carga de los diccionarios disponibles en el directorio y almacenamiento de su información para su posterior consulta
    }

    public boolean tienesDiccionariosDe(string idioma) {
        // Implementación para verificar si hay diccionarios disponibles para el idioma especificado
    }

    public IList<IDiccionario>? getDiccionarios(string idioma) {
        // Implementación para obtener la lista de diccionarios disponibles para el idioma especificado
    }
}
```

Voy a montar la App de consola que usa esta implementación de los diccionarios desde fichero.

```csharp
namespace App.Diccionarios.Consola ;

Using App.Diccionarios.API;
//Using App.Diccionarios.Implementaciones.Fichero; // Y aqui la hemos cagado. Este es el mal! Dentro del proyecto
                                                 // Esta es la muerte del proyecto!
                                                 // Realmente nos hemos cagado en el principio de inversión de dependencias!
                                                 // Esta es la dependencia!

class Programa {

    static void Main(string[] args) {
        // Capturo la palabra y el idioma... y demás...
        // Muestro mensaje de bienvenida: Estás usando la aplicación de diccionarios. V1.1.0
        // En algún sitio llamaré a procesarPeticion(palabra, idioma);
        // Muestro mensaje de despedida: Gracias por usar la aplicación de diccionarios. Nos vemos pronto amigo culturizado! ;)
    }


    private void procesarPeticion(string palabra, string idioma, ISuministradorDeDiccionarios suministrador) {

        //ISuministradorDeDiccionarios suministrador = new SuministradorDeDiccionariosDesdeFichero(@"C:\diccionarios");
        if(suministrador.tienesDiccionariosDe(idioma)) {
            IList<IDiccionario>? diccionarios = suministrador.getDiccionarios(idioma);
            foreach(IDiccionario diccionario in diccionarios) {
                if(diccionario.existe(palabra)) {
                    IList<string>? significados = diccionario.getSignificados(palabra);
                    // Se los muestro en pantalla:
                    Console.WriteLine($"La palabra \"{palabra}\" existe en el diccionario de {idioma} y tiene los siguientes significados:");
                    foreach(string significado in significados) {
                        Console.WriteLine($"- {significado}");  
                    }
                    break;
                }
            }
        } else {
            // Devolver mensaje de que no hay diccionarios disponibles para el idioma especificado
            Console.WriteLine($"La palabra \"{palabra}\" no existe en el diccionario de {idioma}.");
        }

    }
}
```

---
Principios SOLID:

S: SRP: Single Responsibility Principle (Principio de responsabilidad única)

El SRP no va de planteamentos técnicos... va de personas!
Lo que dice no es que una clase deba tener una única responsabilidad técnica. Esto más o menos es lo que enunció la primera vez que lo definió (el el libro CleanCode)... Fue desastroso... pero la gente se quedó con ello. El nombre también se quedó!

El hecho de que una clase haga cosas de una naturaleza concreta.. y no mezcle tareas de distintas naturalezas (conectar a BBDD, escribir por pantalla...) atiende a otro CONCEPTO... establecido en los años 70: Cohesión!

El SRP lo que realmente dice es que dentro de una clase no puedo meter funciones cuyos cambios futuros puedan venir definidos por distintos ACTORES (personas, perfiles o departamentos de mi empresa que tengan la posibilidad de solicitar cambios a mi programa!)

En nuestro fichero, tenemos un problema de cohesión. Tenemos una clase que hace cosas de naturaleza diferente: 
- Procesar la petición (Solicitar que se muestre un mensaje de bienvenida, solicitar que se muestre un mensaje de despedida, capturar la palabra y el idioma..., preguntar a los diccionarios por ellas)
- UI PURA! Mostrar mensajes por pantalla (mensaje de bienvenida, mensaje de despedida, mensaje de que no hay diccionarios disponibles para el idioma especificado, mensaje de que la palabra existe y sus significados...)

Con esto en la cabeza, nos queda el tinglao así:

```csharp
namespace App.Diccionarios.Consola ;

interface IUIDelPrograma {

    void mostrarMensajeBienvenida();

    void mostrarMensajeDespedida();

    void mostrarMensajeNoHayDiccionariosDisponibles(string idioma);

    void mostrarSignificados(string palabra, string idioma, IList<string> significados);
}

interface ILogicaDelPrograma {

    void procesarPeticion(string palabra, string idioma, ISuministradorDeDiccionarios suministrador);
}

class LogicaDelPrograma : ILogicaDelPrograma {

    public void procesarPeticion(string palabra, string idioma, ISuministradorDeDiccionarios suministrador) {
        // Capturo la palabra y el idioma... y demás...
        // Solicitar que se muestre mensaje de bienvenida
        // procesar la petición (palabra, idioma);
        // Solicitar mensaje de despedida

    }
}

class Programa {

    private IUIDelPrograma ui;
    private ISuministradorDeDiccionarios suministrador;
    private ILogicaDelPrograma logica;

    static void Main(string[] args) {
        // Configurar qué UI quiero usar (consola, API, etc...)
        // Configurar qué suministrador de diccionarios quiero usar (fichero, base de datos, etc...)
        // Configurar qué lógica del programa quiero usar (V1, V2, etc...)
        // Ejecuta la lógica del programa (procesarPeticion(palabra, idioma, suministrador));
    }
}
```
---

# Principio de inversión de dependencias

Una clase no debe depender de implementaciones (otras clases) sino que debe depender de abstracciones (interfaces).

    Class: Programa ---> IDiccionario                           (Interfaz)
                    ---> ISuministradorDeDiccionarios           (Interfaz)
                                ^
                                |
                    ---> SuministradorDeDiccionariosDesdeFichero(Clase concreta)

Lo que me dice el principio de inversión de la dependencia es que debo darle la vuelta (invertir) a la flecha (dependencia) que llega a la clase SuministradorDeDiccionariosDesdeFichero.

A una clase NO LE LLEGAN FLECHAS. De las clases SALEN FLECHAS. A quién pueden llegar flechas es a los interfaces.


    Class: Programa ---> IDiccionario                           (Interfaz)
                    ---> ISuministradorDeDiccionarios           (Interfaz)
                                ^
                                |
                         SuministradorDeDiccionariosDesdeFichero(Clase concreta)

Lo que me asegura este principio es que si lo respeto.

En ciencias exactas, la palabra principio es un sinónimo de ley. 

    PRINCIPIO DE ARQUIMEDES: "Dadme un punto de apoyo y moveré el mundo" Se cumplen siempre.

Pero nosotros no estamos en una ciencia exacta: ESTAMOS EN INGENIERIA DE SOFTWARE... que es diferente de CIENCIAS DE LA COMPUTACION!
En ciencias no exactas, un principio es una regla general que puedo respectar o no. Si la respeto tendrá unas consecuencias.


Los principios SOLID los puedo respetar o no. Si los respeto, tendré unas consecuencias: Mi código será mucho más fácil de mantener, deevolucionar, de testear...

Esto está guay... pero.. cómo lo respeto.

Hay patrones que nos ayudan a respetar este principio. El más común a día de hoy es el patrón de inyección de dependencias (Dependency Injection).
Un patrón YA SI ME DA una forma de escribir el código.
En este caso, el patrón de inyección de dependencias me dice que:

    Cuando un objeto necesite una dependencia, en lugar de crearla él mismo, que le sea solicitada (inyectada) desde el exterior.



                                         UI API        GestorDeDiccionarios API
                                        ^       ^      ^              ^
                                        |       |      |              |
                             (*1) ---> UI Consola      |           GestorDeDiccionarios Implementación a ficheros
                                                |      |                    ^
                Lógica del programa API         |      |                    |
                        ^                       |      |                    |
                        |                       |      |                    |
                       Lógica del programa Implementación <------------ Programa ---> (*1)
                       

Acabamos de llevar el PPIO de inversión de pendencias a nivel no de clase... sino de aquitectura! A los proyectos

Tengo proyectos que definen APIs (interfaces)
Y tengo proyectos que definen implementaciones concretas de esas APIs. (clases)

De los proyectos de implementaciones salen flechas (dependencias) hacia los proyectos de APIs. 
Pero no les llega ninguna flecha a los proyectos de implementaciones.
Flechas solo llegan a los proyectos de APIs. 

Ésto es lo me me dará un programa con una buena mantenibilidad, evolutividad y testabilidad.

    UI API == Especificación del tipo de rueda que quiero para mi bici:
                                - Quiero una rueda de 26 pulgadas, con 2 pulgadas de sección, con cámara de aire, etc...
                                - Cubierta tipo mountain bike, con tacos, etc...

    UI Consola Implementación == La rueda ML-22 de Michelin, que cumple con las especificaciones que he definido en la API de la lógica del programa.

    Lógica del programa API == Especificación de cuadro que quiero para mi bici:
                                - Quiero un cuadro de aluminio, de tales medidas, con horquillas de tal ancho, etc...

    Lógica del programa Implementación == El cuadro de aluminio modelo AL-22 de la marca Aluminio S.A, que cumple con las especificaciones que he definido en la API de la lógica del programa. 

    GestorDeDiccionarios API == Especificación del sistema de frenos que quiero para mi bici:
                                - Quiero un sistema de frenos de disco, con pinzas de 4 pistones, con discos de 180mm, etc...

    GestorDeDiccionarios Implementación a ficheros == El sistema de frenos de disco modelo SD-22 de la marca Shimano, que cumple con las especificaciones que he definido en la API del gestor de diccionarios.

    Mi trabajo es definir Especificaciones (APIs) y luego elegir qué implementaciones concretas quiero usar para cada una de esas APIs.
    Por que no quiero atarme a las implementaciones concretas! Si lo hago, y un día ese fabricante cae, falla en las entregas o genera productos de baja calidad, no quiero estar atado a esa marca. Quiero poder cambiar de marca sin que eso me suponga un gran esfuerzo. Quiero poder cambiar de marca sin que eso me suponga tener que cambiar el resto de mi bici. 

    Siempre y cuando me asegure de tener contratos claros y bien definidos en mis APIs, podré cambiar de implementaciones concretas sin que eso me suponga un gran esfuerzo.

    Ahora bien... Al final, la bicicleta concreta que sale de la fabrica y llega al cliente no es un conjunto de especificaciones...
    Es un conjunto de IMPLMENTACIONES concretas de esas especificaciones.

    La v1 de la bicicleta que le entrego a mi cliente es la siguiente:
        - Rueda ML-22 de Michelin
        - Cuadro de aluminio modelo AL-22 de la marca Aluminio S.A
        - Sistema de frenos de disco modelo SD-22 de la marca Shimano
    Y a lo mejor el día de mañana tengo una v2 de la bicicleta, donde he cambiado al fabricante de las ruedas (Michelin -> Pirelli) :
        - Rueda ML-33 de Pirelli
        - Cuadro de aluminio modelo AL-22 de la marca Aluminio S.A
        - Sistema de frenos de disco modelo SD-22 de la marca Shimano


Esto es lo que me permite tener un programa con una buena mantenibilidad, evolutividad y testabilidad.

    Imaginad la versión 1 del programa que habíamos planteado:

```csharp
class Programa {

    static void Main(string[] args) {
        // Capturo la palabra y el idioma... y demás...
        // Muestro mensaje de bienvenida: Estás usando la aplicación de diccionarios. V1.1.0
        // En algún sitio llamaré a procesarPeticion(palabra, idioma);
        // Muestro mensaje de despedida: Gracias por usar la aplicación de diccionarios. Nos vemos pronto amigo culturizado! ;)
    }

    private void procesarPeticion(string palabra, string idioma) {

        SuministradorDeDiccionariosDesdeFichero suministrador = new SuministradorDeDiccionariosDesdeFichero(@"C:\diccionarios");
        if(suministrador.tienesDiccionariosDe(idioma)) {
            IList<IDiccionario>? diccionarios = suministrador.getDiccionarios(idioma);
            foreach(IDiccionario diccionario in diccionarios) {
                if(diccionario.existe(palabra)) {
                    IList<string>? significados = diccionario.getSignificados(palabra);
                    // Se los muestro en pantalla:
                    Console.WriteLine($"La palabra \"{palabra}\" existe en el diccionario de {idioma} y tiene los siguientes significados:");
                    foreach(string significado in significados) {
                        Console.WriteLine($"- {significado}");  
                    }
                    break;
                }
            }
        } else {
            // Devolver mensaje de que no hay diccionarios disponibles para el idioma especificado
            Console.WriteLine($"La palabra \"{palabra}\" no existe en el diccionario de {idioma}.");
        }

    }
}
```

A este programa le puedo hacer pruebas unitarias?
Mejor dicho... a la función procesarPeticion le puedo hacer pruebas unitarias? Puedo aislarla de otros componentes?
IMPOSIBLE, porque dentro de su código elige la dependencia: new SuministradorDeDiccionariosDesdeFichero(@"C:\diccionarios");. 

No me da la oportunidad de sustituir esa dependencia por un a de mentirijilla... por un test double.. que aisle, que me de confianza.

Dicho de otra forma:

1. Cuándo podré hacer la prueba de esa función? Cuando esté acabado el SuministradorDeDiccionariosDesdeFichero, porque necesito esa clase para poder ejecutar la función procesarPeticion.
   Y hasta entonces? A ciegas!  
    Aquí estamos en un caso ,muy evidente: new OBJECT().
    Un patrón singleton por ejemplo sería la misma mierda!
            ISuministradorDeDiccionarios suministrador = SuministradorDeDiccionariosSingleton.getInstance();
    La función sigue buscando ella la dependencia.. no me da opción a darle yo una (una de mentirijilla) desde el exterior.

    Podré hacer pruebas de solo esa función, automatizadas y funcionales... Pero eso ya concluimos que no es la definición de una prueba unitaria. 

    Esta prueba si o si es de sistema / componente. No puedo bajar de nivel (scope) y hacer pruebas unitarias o de integración.

2. Cuando monte la prueba (cuando dios quiera que hayan acabado el SuministradorDeDiccionariosDesdeFichero) y ejecute la función procesarPeticion, caso 
   que falle, que será lo más probable en primera iteración, no tendré NPI de qué falló? 
   - El fallo me puede venir del código de la función procesarPeticion
   - El fallo me puede venir del código del SuministradorDeDiccionariosDesdeFichero
   - O me puede venir de configuración: No han puesto el puñetero diccionario en la carpeta de marras.
     Mira que yo en mi máquina si que lo tengo puesto... claro.. por eso: "En mi máquina funciona!"... pero en la de los demás no! 

Todos los males, han salido de tener un mal diseño del sistema.
Y esto es algo con lo que las pruebas también me ayudan.
Si una prueba se complica, si no consigo aislar bien el componente que quiero probar, es que probablemente tenga un problema de diseño.


Llevamos ya mucho tiempo (SOLID es del 1999) creando y buscando arquitecturas de componentes desacoplados (BAJO ACOPLAMIENTO: Definido en 1970)
Ya no queremos montar MONOLITOS -> new OBJECT() en cualquier parte de nuestro código. Singletons... etc.

Hoy en día trabajamos con otros patrones, principios, diseños, arquitecturas.

Hace 25 años... cuando trabajabamos con metodologías tradicionales, usábamos arquitecturas MONOLÍTICAS, y hacíamos casi solo pruebas de sistema MANUALES... al fin y al cabo se hacian las pruebas:
- 1 sola vez, al final del proyecto
- Cuando el proyecto estaba acabado.. ya tenía todos los componentes acabados... 
Eso si.. íbamos a ciegas
Y la mantenibilidad, evolutividad y testabilidad de esos programas era una auténtica basura!

Hoy en día, con las metodologías ágiles, con la cultura DevOps, usamos arquitecturas de componentes desacoplados (CLEAN, HEXAGONAL, MICROSERVICIOS...) y hacemos pruebas AUTOMATIZADAS de distintos tipos (unitarias, de integración, funcionales...) a lo largo de todo el ciclo de vida del proyecto... Y la mantenibilidad, evolutividad y testabilidad de esos programas es una auténtica maravilla!
    - Voy a entregar el día 1 tres componentes.. y yo que si el retso están o no... no puedo permitirme el depender de ellos.
    - Y me la paso haciendo pruebas todo el día!

Necesito automatizar, y necesito tener distintos niveles de prueba.


GIT? La clave de GIT no es saber crear ramas, hacer merges o rebases o cherry picks... 
La clave es definir un buen flujo de trabajo. GITFLOW!

Como no lo tenga flipo... y me vienen por ahí 500 problemas.

--- main/master                                         C6 (v1.0.0)             ---> Entorno de pro!
                                                       /    Pruebas de aceptación
--- release                                          C6 (v1.0.0-RC1)            ---> Entorno de pre!
                                                     /    Pruebas de sistema
--- dev/dev/desa ---- C1 ------------> C3 --------> C6 (v1.0.0-desa)            ---> Entorno de integración / QA / Test
                        \            /   \        /
--- feature1            C1 -> C2 -> C3    \      /   Git lo debería poner cabrón... y solo permitir este merge si integración y unitarias pasan.
                        |                  \    /
--- feature2            C1 --> C4 --> C5 ---> C6 
                                       (1)    (2)

                                    Cuando en feature2 considero que mi código está apto para incluirlo en la base de código de la próxima releaseLo primero es hacerle sus pruebas unitarias (1)... el componente por si solo, sin estar integrado con el resto del código debe funcionar 
                                    Además, como voy a integrar mi código con el resto, deberé asegurar las pruebas unitarias (2)
* Rama main. Reglas de oro:
   1. Esta prohibido bajo pena de que te corten las uñas muy cortas y te metan la mano en un vaso lleno de zumo de limón hacer commits en esa rama 
   2. Lo que hay en esta rama se considera apto para producción.

* Rama desa: Reglas:
   1. Esta prohibido bajo pena de que te corten las uñas muy cortas y te metan la mano en un vaso lleno de zumo de limón hacer commits en esa rama 
   2. Lo que hay en esta rama se considera apto para el proximo despligué.
 Las formas de trabajo(metodologías), las herramientas (git), los frameworks(.net) los lenguajes (C#), 
 las pruebas, las arquitecturas TODO ESO EVOLUCIONA EN PARALELO en el tiempo... para resolver los problemas que hay en un momento dado.


---

