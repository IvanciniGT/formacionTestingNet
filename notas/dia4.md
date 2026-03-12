

// Dia 1
interface ISuministradorDeDiccionarios {
    IDiccionario? getDiccionario(string idioma);
}

// Dia 2 al 100 gente hace implementaciones
Impl 1: SuministradorDesdeFicheros              getDiccionario
Impl 2: SuministradorDesdeBaseDeDatos           getDiccionario
..

// Dia 101: Y quiero cambiar la interface, para que ahora tenga una nueva funcion llamada getDiccionarios(idioma);
interface ISuministradorDeDiccionarios {
    IDiccionario? getDiccionario(string idioma);
    IEnumerable<IDiccionario>? getDiccionarios(string idioma){
        //throw new NotImplementedException();
        // Genero una lista donde incluyo el que devuelva getDiccionario(idioma) y el resto de diccionarios que pueda encontrar en el sistema.
        List<IDiccionario> diccionarios = new List<IDiccionario>();
        var diccionario = getDiccionario(idioma);
        if(diccionario != null){
            diccionarios.Add(diccionario);  
            return diccionarios;
        }
        // Si no hay ninguno, devuelvo una lista vacía.
        return null;
    }
}
// Dia 102: Tengo a 300k personas Kalasnikovs en mano con un código que no compila.

// Hoy en día demás, no trabajamos con met. tradicionales... Donde cuando se hace un cambio se hace al sistema completo!
//Trabajo con met. agiles, donde se hacen cambios pequeños, por equipos pequeños... que se van liberando.



Las clases abstracta incorporan lógica común cuando aún hay lógica sin definir.
Las interfaces pueden tener código pero no con la idea de agrupar lógica común, sino con la idea de facilitar la evolución de las interfaces sin romper a los clientes que ya las usan.


---

py
js/ts
java
C#

Variable:

    string texto = "hola";  // Asigna la variable texto al valor "hola"

                ->

                "hola"  Crear en RAM un objeto de tipo string con el valor "hola"
                Hemos creado una variable que puede apuntar a objetos de tipo (o subtipo) string
                Asignar la variable texto a ese objeto de tipo string
            
            texto = "adios"; // Esto se escribiría en otro sitio

            El follón de la gestión de la RAM ahora se lo come el RECOLECTOR DE BASURA!
            Cuando un dato se queda huerfano de variable, el RECOLECTOR DE BASURA potencialmente lo puede eliminar de la RAM, liberando ese espacio para otros datos.

C, Fortran, Ada, una variables es un espacio de memoria que se reserva para almacenar un valor, y ese valor puede cambiar a lo largo del tiempo. Como una caja donde pongo y saco cosas.

Pero esa definición no aplica a lenguajes como Python, JavaScript, Java o C#, donde una variable es una referencia a un dato que tengo en RAM. 

---

Pradigma de programación:

Imperativo
Procedural
Programación funcional                                  Cuando una variable puede apuntar a una funcion y posteriormente puedo ejecutar la funcion desde la variable, digo que el lenguaje soporta programación funcional.
La gracia no es lo que es.. que es mu simple. La gracia es lo que puedo llegar a hacer cuando el lenmguaje soporta eso:
- Puedo crear funciones que acepten funciones como parámetros, o que devuelvan funciones como resultado (closure)
Orientado a objetos
Declarativo [Anotaciones, Decoradores, Atributos, etc]


Felipe, pon una silla debajo de la ventana.     IMPERATIVO
Felipe, debajo de la ventana debe haber una silla. Es tu responsabilidad. DECLARATIVO


Las pruebas tienen 3 partes:
- Preparación del escenario
- Ejecución de la acción a probar
- Comprobación de los resultados

Todas las librerias de test: JUNIT, NUNIT, XUNIT, UNITTEST me ayudan con:
- Configuración/ejecución de las pruebas
- comprobaciones(asserts)

Realmente hoy en día tenemos 3 sintaxis que usamos mucho al definir comprobaciones:
- Assertions clásicas:                      Assert.AreEqual(expected, actual)
- Expectativas:                             actual.expect().toBe(expected) 
- Assertions fluentes/verificaciones:       actual.Should().Be(expected)

Las pruebas pueden acabar en 3 estados:
- Success: Verde = La prueba ha pasado, el resultado obtenido es el esperado.
  - Si todos los asserts se verifican
- Failure: Rojo = La prueba ha fallado, el resultado obtenido no es el esperado.
  - Si un assert no se verifica
- Error: Amarillo = La prueba no ha podido ejecutarse... ha explotado... No está bien!
  - Si hay una Exception

---

Tiene sentido hacer las pruebas del SuministradorDesdeFicheros con ficheros reales? los que mando a producción?

- SI pero a nivel de cada proyecto de cada fichero de diccionarios.

    Porque mi solución debería tener 1 proyecto ADICIONAL
    por cada diccionario de cada idioma!
    Qué locura? en serio Iván? Claro:
    - Tiene versionado independiente! GIT PROPIO!
        App diccionarios v1.1.0 y ES v1.0.0
                         v1.1.0      v1.5.0 
    - Además son gestionados por distintas personas!
      - App? Desarrollo
      - Diccionarios? Lexicógrafos, lingüistas, traductores, etc
  
    En ese proyecto si haré pruebas con el fichero real...
        Es el entregable, y tendré que asegurar que funciona con el programa!

    O no!  porque a lo mejor el programa no esperaba ficheros tan largos... o con tantas definiciones por palabra!

    Necesitaré prueba de integración!

- NO a nivel del componente DiccionariosFicheros

    Pregunta... ese componente está atado a un conjunto cerrado de ficheros? DICCIONARIOS. NO!!!

    Está acabado ese fichero? NPI
    Cuánto tardarían las pruebas en ejecutarse? La hueva!
        FAST!!!
    

