
def generar_saludo_formal(nombre):
    return "Buenos días " + nombre

def saluda(nombre):
    saludo = "Hola " + nombre
    print( saludo )

# Creamos funciones para:
# - Reutilizar código
# - Mejorar la estructura del código: mnto, legibilidad, organización, etc.
# - Cuando no me queda más remedio porque quiero invocar a una función que me pide una función como argumento (ejemplo: map, filter, etc.)

saluda("Ivan")

persona = "Menchu"

saluda(persona)

miFuncion = saluda # Asiganr mi variable a una función

miFuncion("ivan")  # Ejecutar la función desde la variable


def imprimir_saludo(funcion_generadora_de_saludos, nombre):
    saludo = funcion_generadora_de_saludos(nombre)
    print(saludo)

def generar_saludo_informal(nombre):
    return "Hola " + nombre


imprimir_saludo(generar_saludo_informal, "Federico")


imprimir_saludo(generar_saludo_formal, "Felipe")
imprimir_saludo(lambda nombre: "Bienvenido " + nombre, "Felipe")




def generar_saludo_otro(nombre):
    return "Bienvenido " + nombre



 nombre => "Bienvenido " + nombre