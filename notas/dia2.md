# Vocabulario

Causa raíz -> Error -> Defecto(BUG) -> Fallo

# Usos de las pruebas

- Asegurar el cumplimiento de unos requisitos (FUNCIONALES y NO FUNCIONALES)
        - Requisitos funcionales: lo que el sistema debe hacer / cómo debe comportarse
        - Requisitos no funcionales: cómo debe ser el sistema (rendimiento, seguridad, usabilidad, etc.)
- Encontrar defectos en el software:
  - Buscar defectos directamente
  - Intentar provocar fallos para descubrir defectos ocultos (+ Recopilar información que facilite ese proceso: depuración o debugging)
- Ayudarnos a saber qué tal va el desarrollo del software (especialmente relevante en metodologías ágiles)
- Aprender del sistema... para aumentar nuestro know how...
- Mediante un análisis de causas raíz, podemos plantear acciones preventivas que eviten nuevos errores (+Defectos y fallos) en el futuro
- ...

# Tipos de pruebas

Hay muchas formas de clasificar las pruebas, y son paralelas entre sí. Responden a diferentes preguntas:

- Qué están probando?                       En base al objeto de prueba:
                                              - Pruebas funciones 
                                              - Pruebas no funcionales
                                                - Estrés
                                                - Carga 
                                                - Rendimiento
                                                - Usabilidad
                                                - UX
                                                - Seguridad
                                                - Alta disponibilidad
                                                - ...
- En que contexto se están probando?        En base al scope o ámbito de la prueba:
                                              - Pruebas unitarias           Probar una característica del sistema/componente de forma AISLADA.
                                              - Pruebas de integración      Probar la COMUNICACION entre componentes del sistema.
                                              - Pruebas de sistema          Probar el COMPORTAMIENTO del sistema en su conjunto.
                                                - Pruebas de aceptación     Probar el cumplimiento de los requisitos del cliente.
- Cómo las hace?                            Dínamicas o estáticas
                                              - Pruebas dinámicas           Ejecutar el software para comprobar su comportamiento.
                                                    ---> Provocan FALLOS! --> Defectos
                                              - Pruebas estáticas           Analizar el software sin ejecutarlo (revisiones de código, análisis estático, etc.)
                                                    ---> Identificar DEFECTOS directamente.
- Quién las hace?                           En base a quién las realiza:
                                              - Pruebas manuales            Realizadas por personas.
                                              - Pruebas automatizadas       Realizadas por herramientas o scripts.
                                                Hoy en día hay una tendencia a automatizar las pruebas. No es necesario...
                                                Solo cuando es conveniente (repetitivas) **VER (1)


    Prueba al sillín: Aguanta una persona de 150Kgs <- Carga
        Aguanta mi programa si le subo un documento de 100Mbs
        Aguanta el programa cuando tengo en la BBDD 10M de registros
        Aguanta el programa cuando tiene 1000 usuarios conectados a la vez

    Pruebas de Estrés: Se va estropeando el cuero del sillín cuando me he subido mil y una veces?
        Con el paso del tiempo / con mucho uso hay una degradación?
            Cuando mi programa lleva 10 horas seguidas ejecutando tareas, empieza a acumular demasiada memoria (MEMORY LEAK) y se va degradando su rendimiento?

    Pruebas de rendimiento: Miden tiempos

            Estas pruebas las hacemos con herramientas como: JMeter, Gatling, Laodrunner, etc.

    Pruebas de usabilidad vs UX

        Usabilidad está definido en una norma ISO?
            -> Eficiencia: El sistema debe permitir al usuario realizar sus tareas con un mínimo esfuerzo.
            -> Eficacia: El sistema debe permitir al usuario realizar sus tareas con éxito.
        
        UX es más amplio... e incluye la usabilidad pero también otros aspectos como la estética, la satisfacción del usuario, accesibilidad, etc.
        Tiene más que ver con la Sensación que se le queda/genera al usuario después de usar el sistema.

    Pruebas unitaria = Prueba automatizada Funcional de una porción chiquita del código <-- FALSE! NOT TRUE!!!
    Prueba unitaria:
        Puede ser manual o automatizada.
        Puede ser funcional o no funcional.

    Prueba de rendimiento unitaria.
    Me dicen:
        R1: El sistema, instalado en un entorno con TALES características, cuando esta siendo sometido a tal carga de trabajo (X personas haciendo tales operaciones) debe ofrecer un tiempo de respuesta al usuario con un Percentil 95 inferior a 500ms.
        
            A qué tipo de prueba da lugar este requisito?
            - Unitaria          Hago una prueba unitaria de rendimiento manual para medir la latencia de comunicaciones a la BBDD = 100ms
            - Integración       Lanzo query desde mi programa.. y espero respuesta de BBDD y mido tiempo
                                    Cómo mi programa manda la query:           TEXTO
                                                                               PREPARED STATEMENT (parametrizada) 
            - Sistema           Ese requisito lo compruebo finalmente a nivel de sistema.


---

(1) Automatización de pruebas.

Hoy automatizamos más pruebvas que antiguamente. Por qué?

Antiguamente trabajábamos con metodologías tradicionales (Waterfall, V, Espiral...)... en ellas, la fase de pruebas se realizaba cuando el proyecto estaba ya desarrollado... después del desarrollo. Cuántas veces se hacían pruebas? 1.. cuando acaba el proyecto.. antes de la entrega.

    Toma de requisitos -> Plan -> Diseño -> Desarrollo -> Pruebas -> Entrega
        Este proceso podía ser fácil 1 año, 2 años...

    Control de proyecto: 

        Hito 1:     10 Abril     **R1, R2, R3**
            Si llegado el 10 de Abril no estaba el R3:
            - Ostias pa' tos laos!
            - Sirenas
            - Se aplaza el HITO: 10 Abril -> 20 Abril
        Hito 2:     10 Mayo      R4, R5, R6
        Hito 3:     10 Junio     R7, R8, R9

Con las metodologías ágiles, el proceso es iterativo e incremental. Lo que buscamos es un feedback muy rápido por parte del cliente.
Y lo que hacemos para conseguir es instalar en producción cada mes (2-8 semanas): Iteraciones o Sprints

    Control de proyecto:

        Sprint 1:   **10 Abril**     R1, R2, R3
            Si llega el 10 de Abril y no está el R3:
            - Ostias pa' tos laos!
            - Sirenas
            - NO SE APLAZA EL SPRINT: LA FECHA NO SE TOCA. Es sagrada!
            - Y lo que ocurre es que el R3 se pasa al siguiente.

                Pruebas: Pruebo el R1, R2, R3

        Sprint 2:   10 Mayo      R4, R5, R6

                Pruebas: R1, R2, R3, R4, R5, R6

        Sprint 3:   10 Junio     ... ya veremos!

    Además de esto , hay otra diferencia importante:
        Los hitos eran para control interno
        Los sprints son para control interno y para obtener feedback del cliente: PASO A PRODUCCION!
        Paso a producción -> Pruebas a nivel de producción

    En las metodologías ágiles, las pruebas me crecen como enanos! Se multiplican.

    La pregunta es: De dónde sale la pasta para tanta prueba? Para tanto tester? Horas? Recursos? Dinero? Tiempo? No las hay!
    Ni hay pasta, ni hay recursos ni hay tiempo para tanta prueba. -> Automatización de pruebas!

---

# Ejemplo ya aplicado al mundo del software

Backend: Sistema de Animalitos Fermín.


    Frontal                     Backend
    ----------------------     ---------------------------------------------------------------------------------------------------------------
    Angular                     .net C#                                                                                     BBDD (SQL Server)
    App web  ----------------->
                                                !!altaAnimalito!!
                                  AnimalitosRestControllerV1    AnimalitosService                           AnimalitosRepository
                                                                     Animalito altaAnimalito(DatosDeAlta)   guardarAnimalito(Animalito)
                                                                            validarDatos()
                                                                            solicitarLaPersistencia()
                                                                            solicitarEnvíoDeEmail()
                                                                            devolverResultado()
                                  ^^^                                ^^^
                                  Lógica de exposición               Lógica de negocio                      Lógica de persistencia
                                  del servicio
                       <----      JSON                  <----        Animalito
                       <----      Status: 500           <----        Exception: DatabaseConnectionException

                                                                EmailsService


                Http REST

Este es mismo rollo que con la bici. Mi bici tenía manillar, sillíon, ruedas, cuadro, sistema de frenos.
Mi app backend tiene: Controladores, servicios, repositorios, mappers...etc.

Pregunta... a todo componente de nuestro sistema puedo hacerle pruebas unitarias? NO SIEMPRE!
Depende de la implementación. Como haya hecho una mala implementación NO VA A HABER FORMA HUMANA DE HACER UNA PRUEBA UNITARIA! Por definición va a ser imposible!

    Vamos a plantear las pruebas para altaDeAnimalito. Tipos de pruebas que vamos a hacer en base al scope?
        Sistema / componente

                AnimalitosService ----> AnimalitosRepository -> BBDD
                                  ----> EmailsService --> SMTP

                            Dado:          Que tengo los datos de una animalito para darlo de alta
                                            Y que esos datos están guay! (Nombre: Firulais= GUAY! VALIDO, EDAD= 12 meses = GUAY!..) No hay problemas con ellos.
                                            Y que tengo un servicio de animalitos   
                                            Y que ese servicio de animalitos tiene un Repository DE VERDAD DE LA BUENA!
                                            Y que ese servicio de animalitos tiene un EmailsService DE VERDAD DE LA BUENA!
                            Cuando:        llamo al servicio de animalitos a la función altaAnimalito con esos datos
                            Entonces:      Debe existir un animal en la BBDD con esos datos (NOMBRE: Firulais, EDAD: 12 meses) con un ID
                                            Y en mi bandeja de entrada POP3, debería recibir un email con el asunto "Nuevo animalito dado de alta: Firulais" y el cuerpo del email debería contener los datos del animalito (ID=33, NOMBRE: Firulais, EDAD: 12 meses).
                                            Y el email no debe tener más de 10 segundos de retraso de vida
                                            Y como me cuesta poco, de paso sigo verificando que el ID del animalito que me han devuelto coincide con el ID del animalito que me han dado alta en la BBDD


        Integración

            Prueba de integración con el Repositorio (EmailsServiceDummy)
                AnimalitosService ----> AnimalitosRepository -> BBDD
                                  -//-> EmailsService

                            Dado:           Que tengo los datos de una animalito para darlo de alta
                                            Y que esos datos están guay! (Nombre: Firulais= GUAY! VALIDO, EDAD= 12 meses = GUAY!..) No hay problemas con ellos.
                                            Y que tengo un servicio de animalitos
                                            Y que ese servicio de animalitos tiene un Repository DE VERDAD DE LA BUENA!
                                            Y que ese servicio de animalitos tiene un EmailsService Dummy
                            Cuando:         llamo al servicio de animalitos a la función altaAnimalito con esos datos
                            Entonces:       Debe existir un animal en la BBDD con esos datos (ID=33, NOMBRE: Firulais, EDAD: 12 meses)
                                            


            Prueba de integración con el EmailsService (Stub del Repository -> 33)
                AnimalitosService -//-> AnimalitosRepository -> BBDD
                                  ----> EmailsService --> SMTP

                            Dado:           Que tengo los datos de una animalito para darlo de alta
                                            Y que esos datos están guay! (Nombre: Firulais= GUAY! VALIDO, EDAD= 12 meses = GUAY!..) No hay problemas con ellos.
                                            Y que tengo un servicio de animalitos
                                            Y que ese servicio de animalitos tiene un Repository Stub que siempre devuelve ID= 33 y succcess!
                                            Y que ese servicio de animalitos tiene un EmailsService DE VERDAD DE LA BUENA!
                            Cuando:         llamo al servicio de animalitos a la función altaAnimalito con esos datos
                            Entonces:       En mi bandeja de entrada POP3, debería recibir un email con el asunto "Nuevo animalito dado de alta: Firulais" y el cuerpo del email debería contener los datos del animalito (ID=33, NOMBRE: Firulais, EDAD: 12 meses).
                            Y el email no debe tener más de 10 segundos de retrasode vida

        Unitarias
            AnimalitosService.altaAnimalito(DatosDeAlta). Esta prueba es unitaria? DEPENDE DEL CONTEXTO!


            Este es el subsistema que estoy probando ahora mismo (**2)

                AnimalitosService -//-> AnimalitosRepository -> BBDD
                                  -//-> EmailsService

                        PRUEBA QUE HAGO UNITARIA:   HAPPY PATH

                            Dado:           Que tengo los datos de una animalito para darlo de alta
                                            Y que esos datos están guay! (Nombre: Firulais= GUAY! VALIDO, EDAD= 12 meses = GUAY!..) No hay problemas con ellos.
                                            Y que tengo un servicio de animalitos
                                            Y que ese servicio de animalitos tiene un Repository que siempre devuelve ID= 33 y succcess!
                                            Y que ese servicio de animalitos tiene un EmailsService cuya función hace de ESPIA!!
                            Cuando:         llamo al servicio de animalitos a la función altaAnimalito con esos datos
                            Entonces:       √ No debe dar fallo por los datos del animalito (dado que son correctos)
                                            X Comprobar que se solicitó la persistencia del animalito al Repository   NO HACE FALTA
                                            X Comprobar que se captura la respuesta del Repository (ID=33)            NO HACE FALTA
                                                                                                                    Ya que si me devuelve 33, eso significa que se ha solicitado la persistencia al Repository y se ha capturado su respuesta.
                                            √ Y me debe dar de vuelta un DTO con los datos del animalito devueltos por el Repository:
                                                 (ID=33)
                                                 NOMBRE: Firulais
                                                 EDAD: 12 meses

                    Que tal ha quedado esa prueba? RUINA!
                        Hay una potencial implemenatción del código que siendo incorrecta daría la prueba por válida!
                        Si el envío de email no se solicita. Y AQUI HAY UN PROBLEMA... Soy capaz de saber si realmente se solicitó el envío del email?
                        Con el dummy NO. No me vale un Dummy en este caso... Los dummies los usamos para hacer pruebas más FAST (RAPIDAS)... pero en este caso no me sirve.. necesito asegurar que se ha solicitado el envío del email... y para eso necesito un Spy.
                                            √ Y cuando pregunto al SPY si se ha solicitado el envío del email, me dice que sí! 
                                            √ Y cuando pregunto al SPY por el animalito que se ha solicitado el envío del email, me dice que es el animalito con ID=33, NOMBRE: Firulais, EDAD: 12 meses

                Podría hacer en su lugar una prueba usando un mock.. y cambia ligeramente:

                        PRUEBA QUE HAGO UNITARIA:   HAPPY PATH

                            Dado:           Que tengo los datos de una animalito para darlo de alta
                                            Y que esos datos están guay! (Nombre: Firulais= GUAY! VALIDO, EDAD= 12 meses = GUAY!..) No hay problemas con ellos.
                                            Y que tengo un servicio de animalitos
                                            Y que ese servicio de animalitos tiene un Repository que siempre devuelve ID= 33 y succcess!
                                            Y que ese servicio de animalitos tiene un EmailsServiceMock.
                                            Y que a ese mock le he dicho que debe ser llamado con los datos del animalito con ID=33, NOMBRE: Firulais, EDAD: 12 meses
                            Cuando:         llamo al servicio de animalitos a la función altaAnimalito con esos datos
                            Entonces:       √ No debe dar fallo por los datos del animalito (dado que son correctos)
                                            √ Y me debe dar de vuelta un DTO con los datos del animalito devueltos por el Repository:
                                                 (ID=33)
                                                 NOMBRE: Firulais
                                                 EDAD: 12 meses

                                            √ Y cuando pregunto al MOCK si ha ido todo bien, me dice que si.


            Cuáles son las responsabilidades de la función altaAnimalito en el AnimaliotsService?
                    - Validar los datos de un animalito
                    - Solicitar su persistencia al Repository
                    - Capturar la respuesta del Repository
                    - Solicitar el envío de un email al EmailsService
                    - Devolver un DTO con los datos del animalito devueltos por el Repository


    AISLAR significa no conectar ese componente a otros componentes REALES de nuestro sistema... Sino conectarlo a componentes EN LOS QUE CONFIO... que sé que no van a fallar!


    Si alguna prueba falla, ahora tengo muy claro qué es lo que falla:
    -> Unitaria: Si falla la prueba unitaria, el fallo es exclusivamente achacable al Servicio de Animalitos: Su lógica o implementación.
    -> Integración con Repositorio: Si falla la prueba de integración con el Repository, el fallo es exclusivamente achacable a la comunicación entre el Servicio de Animalitos y el Repository.
    -> Integración con EmailsService: Si falla la prueba de integración con el EmailsService, el fallo es exclusivamente achacable a la comunicación entre el Servicio de Animalitos y el EmailsService.
    -> Sistema: Si falla la prueba de sistema, el comportamiento global del sistema completo no es adecuado.
        Quyizás por separado las cosas iban bien... pero al juntarlas no.


    Un código que tenga 10.000 líneas, fácilmente acabará con 50k de código de pruebas... y eso es normal! No es un problema! Es lo que hay! Es lo que se necesita para asegurar la calidad del software! Y no hay pasta ni recursos para tanto tester... ni tiempo para escribir tanto código de pruebas... por eso usamos librerías que nos generan ese código de pruebas por nosotros... pero el código está ahí! Y hay que entenderlo!

    Por suerte , hoy en día ni nos molestamos en escribir esas pruebas: COPILOT!

```csharp

class AnimalitosService
{
    private IAnimalitosRepository animalitosRepository;
    private IEmailsService emailsService;
    private IAnimalitosMapper mapeador;
    //...

    public AnimalitoDTO altaAnimalito(DatosDeAltaDTO datos)
    {
        validarDatos(datos);
        var animalito = new Animalito(datos);
        Animalito animalitoPersistido = animalitosRepository.solicitarLaPersistencia(animalito);
        //emailsService.solicitarEnvíoDeEmail(animalitoPersistido);
        return mapeador.mapToDTO(animalitoPersistido);
    }

    private void validarDatos(DatosDeAltaDTO datos)
    {
        // Validación de datos
    }
}

interface IAnimalitosRepository
{
    Animalito solicitarLaPersistencia(Animalito animalito);
    // ...
}

interface IEmailsService
{
    void solicitarEnvíoDeEmail(Animalito animalito);
    // ...
}

class IEmailsServiceDummy : IEmailsService
{
    public void solicitarEnvíoDeEmail(Animalito animalito)
    { } // Este código puede fallar? NO... no hace nada! No hace ni la hueva! -> CONFIO EN EL = que confio en el bastidor o el sensor de presión.
} // Una clase dummy (uno de los tipos de TestDoubles) es una implamentación que no hace nada. Devuelve los tipos más simples apra una función.
    // void -> no devuelve nada
    // int -> devuelve 0
    // objeto -> devuelve null
    // boolean -> devuelve false

class IEmailsServiceSpy : IEmailsService
{
    private Animalito animalitoRecibido;

    public void solicitarEnvíoDeEmail(Animalito animalito)
    { 
        animalitoRecibido = animalito;
    }

    public bool SeSolicitóEnvíoDeEmail()
    {
        return animalitoRecibido != null;
    }
    public Animalito ObtenerAnimalitoRecibido()
    {
        return animalitoRecibido;
    }
} // Una clase spy es una implementación que registra información sobre cómo se ha interactuado con ella. En este caso, registra si se ha solicitado el envío de un email o no. Es útil para verificar interacciones en pruebas unitarias.

class AnimalitosRepositoryStub : IAnimalitosRepository
{
    public Animalito solicitarLaPersistencia(Animalito animalito)
    {
        // Simulamos que la persistencia siempre es exitosa y devuelve un ID generado por la BBDD.
        animalito.Id = 33; // ID simulado
        return animalito;
    } // Este código puede fallar? NO... siempre devuelve el mismo ID y no hace nada más! -> CONFIO EN EL = que confio en el bastidor o el sensor de presión.

}// Un stub es una implementación que devuelve datos fijos. Sin lógica adicional. Es útil para simular respuestas de componentes externos o dependencias en pruebas unitarias.


class IEmailsServiceMock : IEmailsService
{
    private Animalito animalitoQueSeEsperaRecibir;
    private Animalito animalitoRecibido;

    public void setAnimalitoQueSeEsperaRecibir(Animalito animalito)
    {
        animalitoQueSeEsperaRecibir = animalito;
    }

    public void solicitarEnvíoDeEmail(Animalito animalito)
    { 
        animalitoRecibido = animalito;
        if (animalitoQueSeEsperaRecibir == null)
        {
            throw new Exception("No se ha configurado el animalito que se espera recibir en el mock.");
        }
        if (!animalitoRecibido.Equals(animalitoQueSeEsperaRecibir))
        {
            throw new Exception($"El animalito recibido no coincide con el animalito esperado. Recibido: {animalitoRecibido}, Esperado: {animalitoQueSeEsperaRecibir}");
        }
    }

    public bool haIdoiTodoBien()
    {
        return animalitoRecibido != null && animalitoRecibido.Equals(animalitoQueSeEsperaRecibir);
    }
}  // En un mock, la lógica de verificación está dentro de la propia clase. El mock se configura con las expectativas de cómo se debe interactuar con él, y luego verifica si esas expectativas se cumplen durante la prueba. Si no se cumplen, el mock puede lanzar excepciones o marcar la prueba como fallida.


```

En la práctica, e 99% de las veces, no me pongo yo a crear estas clases (Dummy, Stub,...) lo que uso son librerias que crean ese código por mi.
Pero el código está ahí! Y hay que entender lo que esas librerías están haciendo!

            Esa prueba la puedo hacer de forma unitaria... pero para ello necesito aislar al Servicio del AnimalitosRespotory y del EmailsService.
                El objetivo es que si la prueba falla, el fallo no pueda achacarse al Repository ni al EmailsService... sino que el fallo sea exclusivamente achacable al Servicio de Animalitos.


En ocasiones usamos un FAKE!
Eso es un componente que simula con cierta lógica el comportamiento de un componente real. 
Por ejemplo, puedo usar para pruebas de un frontal un fake del backend... que en lugar de trabajar contra una BBDD, meta los datos en un fichero json.
Y no aplique toda la lógica de negocio/validaciones que haría el componente real.
    json-server: es una herramienta que nos permite crear un fake de un backend REST a partir de un fichero JSON. Es útil para pruebas de frontend cuando el backend aún no está disponible o para simular respuestas específicas del backend.

Ojo... Cuando uso un FAKE, Si tengo comunicación con un componente REAL.. aunque sea simulado:

    Frontal ----> Backend de mentira
            http
            ^^^
            Esto ya se está produciendo. Es un nivel más de prueba


Os suenan las bases de datos InMemory: H2, HSQLDB, SQLite...? Básicamente son un FAKE de una base de datos real. Simulan el comportamiento de una base de datos real, pero se ejecutan en memoria y no requieren configuración ni mantenimiento. Eso si, me dan la capacidad de probar la comunicación con una BBDD (jdbc, entity framework, etc.) pero sin necesidad de tener una BBDD real configurada. Son muy útiles para pruebas de integración o incluso para pruebas unitarias cuando necesito probar la comunicación con la base de datos.

Si a un fake le sigo implementado lógica, acabo con un componente que se parece mucho a un componente real.

---

Nota **2

Esto de las taxonomía de pruebas hay que entenderlo.. porque nos ayuda.. pero no hay que volverse loco con ello, ni ser un fanático.

    Soy un fabricante de bicicletas... y a mi bici le voy a montar una dinamo con una luz.
    Para mi... fabricante de la bicicleta, el poner la dinamo y la bombilla en un bastidor y darle vueltas a la dinamo a ver si la luz luce, qué tipo de prueba es?  UNITARIA! Pruebo este componente aislado del resto de componentes de mi bicicleta.

    Pregunta. Para el fabricante de la dinamo esa misma prueba que tipo de prueba es? SISTEMA!

    La prueba de integración sería poner en un bastidor la dinamo y la rueda... a ver si cuando la rueda gira, la bombilla luce.
    Otra prueba de integración es poner la luz y la dinamo en el cuadro ... a ver si los agujeros encajan para los tornillos.

    Depende de para quíen la misma prueba puede cambiar de categoría.. y está bien!

    De hecho, lo que habitualmente llamamos pruebas de componente, no son sino pruebas de sistema aplicadas a subcomponentes de nuestro sistema.Es decir, si la dinamo la fabrico yo... le haré pruebas de "sistema" (completas) a la dinamo... pero realmente no es mi "sistema" mi sistema es la "bici" ... por eso las llamo, pruebas de componente





----

Principios FIRST: 
F - Fast: Las pruebas unitarias deben ser rápidas de ejecutar. Si una prueba tarda mucho en ejecutarse, es menos probable que se ejecute con frecuencia, lo que puede llevar a que los defectos pasen desapercibidos.
I - Independent: Las pruebas unitarias deben ser independientes entre sí. Cada prueba debe poder ejecutarse de forma aislada, sin depender del resultado de otras pruebas. Esto facilita la identificación de defectos y mejora la mantenibilidad de las pruebas.
R - Repeatable: Las pruebas unitarias deben ser repetibles. Deben producir los mismos resultados cada vez que se ejecutan, independientemente del entorno o el orden en que se ejecuten. Esto garantiza la confiabilidad de las pruebas y facilita la detección de defectos.
S - Self-validating: Las pruebas unitarias deben ser autovalidantes. Deben incluir aserciones claras que verifiquen el comportamiento esperado del código bajo prueba. Esto permite identificar rápidamente si una prueba ha pasado o ha fallado, sin necesidad de intervención manual.
T - Timely: Las pruebas unitarias deben escribirse en el momento adecuado. Idealmente, las pruebas unitarias deben escribirse antes o durante el desarrollo del código que están probando. Esto permite detectar defectos de manera temprana y facilita la refactorización del código sin temor a introducir nuevos errores.