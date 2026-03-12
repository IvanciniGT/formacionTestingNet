
# Vocabulario en Testing

- Causa Raiz    El motivo por el que el humano cometió el error: 
               - Falta de conocimiento
               - Falta de atención
               - Falta de experiencia
               - Falta de descanso
               - Falta de motivación
- Error         Los humanos comentemos errores (errar es humano). Las máquinas cometen errores? NO
- Defecto       Al cometer un error, podemos introducir un defecto en el producto. (BUGS)
- Fallo         Es la manifestación de un defecto, cuando se comienza a usar el producto. 
                Desviación con respecto al comportamiento esperado.

---

# Para qué sirve el Testing?

- Asegurar que el producto cumple con los requisitos  (ojo: funcionales y NO funcionales)
- Tratar de minimizar los defectos en el producto antes de su entrega!
  -> Buscaremos fallos al usar el producto (LO MAS HABITUAL)
     Del fallo buscamos el defecto que lo origina (es lo que quiero RESOLVER, REMOVER del producto): DEBUGGING
     - Las pruebas deben proveer información que facilite el debugging (logs, capturas de pantalla, etc) (1)
  -> Buscaremos directamente defectos en el código: REVISIÓN
- Para saber qué tal va el proyecto (Sobre todo si usamos una metodología ágil, donde el software funcionando es la medida principal de progreso) (2)
- Saber de mi producto y aprender de él... quizás ni para aplicarlo al proyecto actual
- Haciendo un análisis de causas raiz de los defectos, evitar nuevos errores (DEFECTOS > FALLOS) en el futuro.
- ...

## Tipos de pruebas:

Hay muchas formas de clasificar las pruebas (varias taxonomías), todas importantes y paralelas entre si!

Toda prueba, por definición se centra en un único aspecto del producto... veáse 1

## Si miro en base al objeto de prueba:

- Pruebas funcionales:          Son las que se definen en Requisitos funcionales (lo que el producto debe hacer)
- Pruebas no funcionales:       Son las que se definen en Requisitos no funcionales (lo que el producto debe ser)
    - Rendimiento   
    - Carga
    - Seguridad
    - HA
    - Estrés
    - Usabilidad

## En base al scope (ámbito) de la prueba:

- Unitarias             Se centra en una característica de un componente AISLADO del sistema.
- Integración           Se centra en la COMUNICACION entre 2 componentes del sistema.
- Sistema (end2end)     Se centran en el comportameinto del sistema completo, en su conjunto.
  - Aceptación 

## En base al conocimiento acerca del objeto probado:
- Caja negra: No tengo ni idea de cómo funciona el objeto probado, ni de su código fuente. Solo sé lo que se supone que debe hacer (requisitos funcionales y no funcionales)
- Caja blanca: Tengo acceso al código fuente, y lo conozco. Puedo diseñar pruebas que me permitan cubrir el código fuente (cobertura de código) y así aumentar la probabilidad de encontrar defectos en el código.

## En base al procedimiento de ejecución:

- Manuales: Las pruebas las ejecuta un humano, que interactúa con el producto.
- Automatizadas: Las pruebas las ejecuta una máquina, a través de un script o programa que interactúa con el producto.

## En base a la forma de realziar la prueba:

- Dinámicas: Se ejecutan el producto, y se observa su comportamiento.
- Estáticas: No se ejecuta el producto, se analiza su código fuente, documentación, etc, para encontrar defectos.

---



Soy un fabricante de Bicicletas!
- Fabrico los neumáticos? NO
- Fabrico el sistema de frenado? NO
- Fabrico el sillín? NO

Y que pinto yo en todo esto? Diseño(Especifico)/Integro los componentes!

-  Me llega la rueba
    (DADO) La monto en un bastidor, (CUANDO) le pego un viaje y (ENTONCES) miro a ver si gira... 
     - UNITARIA: Estoy probando este componente AISLADO, sin tener en cuenta el resto del sistema (la bicicleta) 
     - FUNCIONAL: De funcionamiento

-  Me llega el sistema de frenos
    (DADO) Lo monto en un bastidor, (CUANDO) aprieto la palanca y (ENTONCES) miro a ver si las pinzas de frenos se cierran con la presión suficiente
    Cómo mido la presión? Puedo montar un sensor de presión (TEST DOUBLE)
     - UNITARIA: Estoy probando este componente AISLADO, sin tener en cuenta el resto del sistema (la bicicleta) 
     - FUNCIONAL: De funcionamiento

- Me llegan los sillines.. y les hago pruebas / revisiones. Cuáles?
  > Prueba 1 
     - Dado que tengo un sillín!
        Y que lo monto en 4 hierros mal soldados (bastidor)  <<< TEST DOUBLES
     - Cuando me siento encima de él!
     - Entonces no me caigo al suelo... aguanta mi peso.
        UNITARIA: Estoy probando este componente AISLADO, sin tener en cuenta el resto del sistema (la bicicleta) 
        NO FUNCIONAL: De carga

  > Prueba 2
     - Dado que tengo un sillín!
        Y que lo monto en 4 hierros mal soldados (bastidor)  <<< TEST DOUBLES
     - Cuando me siento y me bajo 5000 veces de él
     - Entonces el cuero no se desgarra ni desgasta.
        UNITARIA: Estoy probando este componente AISLADO, sin tener en cuenta el resto del sistema (la bicicleta) 
        NO FUNCIONAL: De estrés

  > Prueba 3
     - Dado que tengo un sillín!
        Y que lo monto en 4 hierros mal soldados (bastidor)  <<< TEST DOUBLES
     - Cuando me siento 4 horas en él
     - Entonces el culo no me duele
        UNITARIA: Estoy probando este componente AISLADO, sin tener en cuenta el resto del sistema (la bicicleta) 
        NO FUNCIONAL: Usabilidad / Experiencia de usuario
    
  > Prueba 4
     - Dado que tengo un sillín!
        Y que lo monto en 4 hierros mal soldados (bastidor)  <<< TEST DOUBLES
     - Cuando giro mucho
     - Entonces me sujeta bien (no me resbalo)
        UNITARIA: Estoy probando este componente AISLADO, sin tener en cuenta el resto del sistema (la bicicleta) 
        NO FUNCIONAL: Seguridad


Imaginad que defino todas las pruebas unitarias que se me ocurran... PAsarlas garantiza que la bicileta está apta para producción?
NO...
    De hecho,... qué bici? No tengo aún bici.. tengo componentes que me van llegando.
    Para que las hago entonces?     CONFIANZA + 1
    - Voy bien (VEASE 2)


---


-  Me llega la rueba y el sistema de frenos... les he hecho sus pruebas unitarias... y van guay!
-  Ahora:
     - DADO
       - Que tengo un sistema de frenos montado en un bastidor
       - Y que tengo una rueda montada en el mismo bastidor entre las pinzas del sistema de frenos
       - Y dado que la rueda le he pegado un viaje y está girando
     - CUANDO:
       - Aprieto la palanca de frenos (sistemaFrenos.apretarPalanca();)
     - ENTONCES:
       - Que la rueda se para
     Y MIRA QUE NO SE PARA !
     Resulta que las pinzas cierran.. y lo hacen con fuerza.. pero no cierran lo suficiente (poco recorrido) como para llegar a tocar la rueda y frenarla.
        - Está mal el sistema de frenos? NO... funciona bien
        - Está mal la rueda? NO... funciona bien
        - Lo que tengo es un problema de integración de componentes.El sistema de frenos no es capaz de COMUNICAR la energía de rozamiento que genera al apretar la palanca, a la rueda.
        - O bien el sistema de frenos cierra poco o bien necesito una rueda más ancha para ese sistema de frenos. 


Imaginad que defino todas las pruebas de integración que se me ocurran... Pasarlas garantiza que la bicileta está apta para producción?
NO...
    De hecho,... qué bici? No tengo aún bici.. tengo componentes que me van llegando.
    Para que las hago entonces?     CONFIANZA + 1
    - Voy bien (VEASE 2). Estoy dando pasos en firme!

---
Cojo la bicileta.. Siento a un tio encima.. Le pongo una mochila con agua y bocadillo de chorizo.. y alé a Cuenca (400kms)
Y llega el tio sano y salvo.


Imaginad que defino todas las pruebas de integración que se me ocurran... Pasarlas garantiza que la bicileta está apta para producción?
SI... si ya tengo a un tio disfrutando la bici!

- Necesito entonces si no hubiera hecho primero las pruebas unitarias y de integración... Y hago directamente las de sistema y van bien, hacer las  pruebas unitarias y de integración? NO... ya tengo bici.. ahora para qué? NO SON OPORTUNAS!
  - En esta pregunta el truco está en 2 cosas:
    - Cuándo puedo hacer las pruebas de sistema? Cuando tengo bici! Y hasta entonces qué? A ciegas
    - Y si van mal las de sistema, qué esta fallando? NPI

- Esto significa que la bici ya es apta para mi cliente? NO... eso lo dirá mi cliente.
- Mi bici funciona (supera las pruebas de sistema) pero, es la bici adecuada para mi cliente? Eso lo dirán las pruebas de acpetación!
---

Test dobles:
- Stubs
- Spies
- Fake
- Dummies
- Mocks


---

FIRST
F - Fast
I - Independent ***
R - Repeatable  ***
S - Self-validating
T - Timely : Oportuna, que se hace en el momento adecuado, ni antes ni después.

---


Para definir una prueba vamos aseguir el siguiente esquema:
- Dado          Given 
- Cuando        When
- Entonces      Then



---

Pruebas automatizadas != Pruebas unitarias
---

> Un producto de software por definición es un producto sujeto a cambios y mantenimiento. 

Voy a un taller, a llevar un coche que compré el año pasado. Le toca la primera revisión (cambio de aceite)
- Cuánto me va a doler? ¿Cuánto van a tardar? ESTAFADOS!

Por definición un coche es un producto sujeto a mantenimiento.

---

# Cuál es la característica principal de una Metodología ágil de desarrollo?

- Qué busca en primera instancia?               Feedback más rápido por parte de mi cliente (comparado con los tiempos que teníamos al usar metodologías tradicionales: Waterfall, V, etc)
- Como me sugiere trabajar para conseguirlo?    Entregar el producto de forma incremental (SPRINTS / Iteraciones)

> Extraído del manifiesto ágil: 

    El software funcionando es la MEDIDA principal de progreso. > DEFINIR UN INDICADOR PARA UN CUADRO DE MANDO!

                                                ATRIBUTO
                                       -------------------------
    la MEDIDA principal de progreso es el "software funcionando"
       ------
       NUCLE
    ------------------------------- ----------------------------
    SUJETO                                  PREDICADO

    Cómo mido (que uso para medir) el progreso (el cómo va mi proyecto)? El software funcionando

    > "El software funcionando" = Software que se comportan como se espera que se comporte (requisitos funcionales y no funcionales)
    
    Quién dice eso? Quién dice que el software funciona?
    - ~~El cliente~~ NO... 
    - PRUEBAS
    
    El cliente (y/o usuario) ayuda en la definición de requisitos.
    Las pruebas se hacen para garantizar que el producto cumple con los requisitos.