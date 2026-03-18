

ProveedorDiccionariosBBDD : IProveedorDiccionarios
    //    DiccionariosDbContext              <- Sustuir este por uno de mentirijilla en el que confíe // FAKE
    //    IConfiguration                     <- Sustuir este por uno de mentirijilla en el que confíe // STUB
    //    ILogger<ProveedorDiccionariosBBDD> <- Sustuir este por uno de mentirijilla en el que confíe // FAKE





        Mock<DiccionariosDbContext>


    Proveedor // -> Contexto de BBDD -> odbc -> BBDD
                Mock<Contexto>                                  UNITARIA
                    Que cuendo le pregunte me devuelva 33

                En este caso, siendo puristas lo que vamos a hacer no es una prueba unitaria... aunque la vamos a llamar así.
                Es de integración. Estamos comunicándonos con otro componente. Con sus protocolos... etc.

    Proveedor -> Contexto de BBDD -> odbc -> // BBDD
                                                No poner la BBDD dde verdad de la buena, sino una de mentirijilla en la que confíe.
                                                BBDD InMemory = FAKE
                    ^^^
                    Quién hace la implementación de ese DBContext? EntityFramework. Confío en EntityFramework? Más vale!
    TestDoubles:
      - Stub
      - Mock
      - Fake
      - Dummy
      - Spy



---

    La implementación de las pruebas de ProveedorDiccionariosBBDD debe ser exactamenteigual a la de ProveedorDiccionariosFichero.

    Qué cambia solo? Setup y el TearDown.
    Las funciones de prueba deben ser exactamente iguales. Porque lo que queremos es comparar el comportamiento de ambos proveedores, no su implementación.

    Muchas veces definimos un proyecto con los test... y ese proyecto va a asociado al API.
    Y luego creamos proyectos de test para cada implementación concreta, que extienden las clases de test del proyecto de test común. Y ahí es donde hacemos el Setup y el TearDown concreto para cada implementación concreta.

    Esto es cierto... en parte; Esto es cierto para pruebas de sistema/componente... de todo lo que llamo mediante el API.

---

Pruebas de caja blanca:     La que defino teniendo en cuenta la impleentación. 
                            Y cada implementación lleva las suya, diferentes a las de otra implementación.

                            La implementación que hago a BBDD v2...
                            resulta que para no necesitar hacer tantas queries, pongo en medio una caché...
                            La primera vez que llamo a la función, de donde saca el dato? BBDD
                            Y la segunda? de la cache.
                            Estan probando las pruebas que hicmos este comportamiento? No.. solo llamo una vez.

Pruebas de caja negra:      Las que defino solo teniendo en cuenta el API.
                            Son transversales.. Me sirven para cualquier implementación posible de ese API.

                            Por ejemplo las que hemos definido en ProveedorDiccionariosTestsBase. Esas son de caja negra. Porque no me importa cómo se implementen, solo me importa que cumplan el contrato del API.


---
                                                                                               App Mobile Android
                                                                                               App Mobile IOS
                                                                                               Asistente de Voz 
                                                                                               IVR

    BBDD                        Servicio                       Controlador Rest                Angular
    Entities
                                                                                   ---json-->
    Persona                                                                                     Persona

    Id                           Id                                                                 Id
    Nombre                       Nombre                                                             Nombre
    FechaDeNacimiento -Mapper->  Edad                -Mapper->                                      Edad    
                                                               http://localhost:5000/api/personas/1
                                List<PersonaDTO> GetPersonas()    List<PersonaDTO> GetPersonas()

---


    ServicioImpl
        Repositorio MOCK    <--- El repo es parte de mi proyecto? NO , si lo es de la solución. (SoC)
                                    El repo quizás ni esté acabado.
                                    El repo... la implementaciuón del repo será 15 ficheros de código... con alta probabilidad de tener bugs
        Mapeador    REAL    <--- El mapeador es parte de mi proyecto? SI
                                    Y me fío de él? NO
                                    Y por eso al mapeador le hago sus pruebas unitarias.
                                    Y como las hago, aqui uso el real, porque si!
                                    Y si si falla, la prueba, tengo dudas de donde está el fallo?
                                    Si esta en el mapeador o si esta en el servicio? NO
                                        - Si la prueba unitaria del mapeador ha ido bien, entonces el fallo está en el servicio!
                                        - Si la prueba unitaria del mapeador ha ido mal, entonces no se.. de momento resolvere eso!