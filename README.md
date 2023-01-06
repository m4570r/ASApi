# ASApi
Este programa es un servidor que se ejecuta en un ordenador y que permite a otras aplicaciones o programas enviarle peticiones para obtener información o realizar ciertas tareas. El servidor procesa estas peticiones y envía una respuesta de vuelta a la aplicación o programa que la envió.

## Funciones
El servidor puede manejar cuatro tipos de peticiones: "GET", "POST", "PUT" y "DELETE". Las peticiones "GET" se utilizan para solicitar información al servidor, mientras que las peticiones "POST", "PUT" y "DELETE" se utilizan para enviar información al servidor y solicitar que realice ciertas tareas. Por ejemplo, podrías utilizar una petición "POST" para enviar al servidor información sobre un nuevo usuario y solicitar que se agregue a una base de datos, o podrías utilizar una petición "PUT" para enviar al servidor información actualizada sobre un usuario existente y solicitar que se modifiquen los datos almacenados en la base de datos.

Este servidor se comunica con una base de datos para almacenar y recuperar información. Utiliza el lenguaje de programación C# y la biblioteca de MySQL para conectarse y realizar consultas a la base de datos. El servidor también utiliza la biblioteca Json.NET para serializar y deserializar datos en formato JSON.

Este programa se ejecuta en un bucle infinito, esperando constantemente por peticiones entrantes. Cuando se recibe una petición, el servidor la procesa en un hilo separado para permitir que siga atendiendo a otras peticiones mientras procesa la primera. Al procesar la petición, el servidor puede enviar una respuesta de vuelta al programa o aplicación que la envió, utilizando el formato de respuesta y el código de estado HTTP apropiados.

## Instalacion
Para instalar y ejecutar esta aplicación, necesitarás tener Visual Studio instalado en tu ordenador. Si no tienes Visual Studio, puedes descargar la versión gratuita de Visual Studio Community desde la página de descargas de Microsoft.

Una vez que hayas instalado Visual Studio, sigue estos pasos para ejecutar esta aplicación:

  1. Abre Visual Studio y selecciona "Archivo" -> "Nuevo" -> "Proyecto" en el menú superior.
  2. En la ventana "Nuevo proyecto", selecciona "Aplicación de consola" en el menú de la izquierda y luego elige "Aplicación de consola .NET Core" en el menú de la derecha.
  3. Haz clic en el botón "Siguiente" y escribe un nombre para tu proyecto en el campo "Nombre". 
  4. Luego, haz clic en el botón "Crear" para crear el proyecto.
  5. Una vez creado el proyecto, se abrirá un archivo llamado "Program.cs" en el editor de código. 
  6. Copia y pega el código del servidor en este archivo, reemplazando el código existente.
  7. Haz clic en el botón "Ejecutar" o presiona F5 para ejecutar la aplicación. El servidor se iniciará y estará listo para recibir peticiones.
  8. Tambien debes reemplazar el codigo del archivo "Appsettings.json"

Es importante tener en cuenta que esta aplicación utiliza la librería MySql.Data para conectarse a una base de datos MySQL. Si no tienes una base de datos MySQL o no tienes el controlador de MySQL instalado, deberás instalar estas dependencias antes de ejecutar la aplicación. Puedes encontrar más información sobre cómo instalar el controlador de MySQL en la documentación de la librería.

## Paquetes Nugets

1. Microsoft.AspNetCore.Http.Abstractions(2.2.0)
2. Microsoft.AspNetCore.Http.Extensions(2.2.0)
3. Microsoft.AspNetCore.Http.Features(6.0.0-preview.4.21253.5)
4. Microsoft.AspNetCore.Mvc.Core(2.2.5)
5. Newtonsoft.Json(13.0.2)
6. SapientGuardian.MySql.Data(6.9.814)

Para instalar estas dependencias, deberás agregarlas a tu proyecto de Visual Studio utilizando el administrador de paquetes NuGet. Abre el administrador de paquetes haciendo clic derecho en tu proyecto en el Explorador de soluciones y seleccionando "Administrar paquetes NuGet...". A continuación, busca cada una de las dependencias mencionadas y selecciónalas para instalarlas.

También puedes instalar las dependencias a través de la consola de NuGet utilizando el siguiente comando para cada una de ellas:

```
Install-Package <nombre de la dependencia>
```

Por ejemplo, para instalar Microsoft.AspNetCore.Http.Abstractions, usarías el siguiente comando:

```
Install-Package Microsoft.AspNetCore.Http.Abstractions
```

Recuerda reemplazar <nombre de la dependencia> con el nombre de cada dependencia que desees instalar.

Para instalar estas dependencias, primero debes asegurarte de que tienes el administrador de paquetes NuGet instalado en Visual Studio. Luego, abre el proyecto en Visual Studio y haz clic derecho en el proyecto en el explorador de soluciones. Selecciona "Administrar paquetes NuGet" en el menú contextual. En la página "Administrar paquetes NuGet", haz clic en el botón "Consola del administrador de paquetes" en la parte inferior derecha. Esto abrirá la consola del administrador de paquetes en la parte inferior de la ventana. A continuación, puedes usar los siguientes comandos para instalar cada una de las dependencias que mencionaste:

Para instalar Microsoft.AspNetCore.Http.Abstractions(2.2.0):

```
Install-Package Microsoft.AspNetCore.Http.Abstractions -Version 2.2.0
```

Para instalar Microsoft.AspNetCore.Http.Extensions(2.2.0):

```
Install-Package Microsoft.AspNetCore.Http.Extensions -Version 2.2.0
```

Para instalar Microsoft.AspNetCore.Http.Features(6.0.0-preview.4.21253.5):

```
Install-Package Microsoft.AspNetCore.Http.Features -Version 6.0.0-preview.4.21253.5
```

Para instalar Microsoft.AspNetCore.Mvc.Core(2.2.5):

```
Install-Package Microsoft.AspNetCore.Mvc.Core -Version 2.2.5
```

## METODO GET

Para utilizar el método GET en Postman, primero debes seleccionar GET como el método de la petición en la pestaña de la izquierda. Luego, en la barra de direcciones de la parte superior, debes escribir la dirección del servidor seguida del endpoint correspondiente. Por ejemplo, si quieres hacer una petición GET para obtener todos los usuarios, debes escribir la dirección del servidor seguida de "/". Si quieres hacer una petición GET para obtener un usuario específico, debes escribir la dirección del servidor seguida de "?id=1" o "?nombre=Juan" o "?edad=45", dependiendo de lo que estés buscando. Una vez que hayas escrito la dirección correctamente, simplemente presiona "Send" para enviar la petición y ver la respuesta del servidor.

Para acceder al endpoint /version, puedes enviar una petición GET a la dirección:

```
http://localhost/version
```

Para acceder al endpoint / (todos los usuarios), puedes enviar una petición GET a la dirección:

```
http://localhost/
```

Para acceder al endpoint /?id=1, puedes enviar una petición GET a la dirección:

```
http://localhost/?id=1
```

Para acceder al endpoint /?nombre=Juan, puedes enviar una petición GET a la dirección:

```
http://localhost/?nombre=Juan
```

Para acceder al endpoint /?edad=45, puedes enviar una petición GET a la dirección:

```
http://localhost/?edad=45
```

## METODO POST

Para utilizar el método POST en Postman, primero debes seleccionar POST en la lista de métodos HTTP en la parte superior de la ventana de Postman. Luego, debes especificar la URL de la petición en la barra de direcciones de Postman. Por último, debes especificar el cuerpo de la petición en el formato JSON en el panel "Body" de Postman. El cuerpo de la petición debe incluir los valores para el nombre y la edad del nuevo usuario. A continuación se muestra un ejemplo de cuerpo de petición para POST:

```
{
  "nombre": "Nombre del nuevo usuario",
  "edad": 25
}
```

## METODO PUT

Para utilizar el método PUT en Postman, primero debes seleccionar PUT en la lista de métodos HTTP en la parte superior de la ventana de Postman. Luego, debes especificar la URL de la petición en la barra de direcciones de Postman. Por último, debes especificar el cuerpo de la petición en el formato JSON en el panel "Body" de Postman. El cuerpo de la petición debe incluir el ID del usuario al que se está haciendo la actualización y los nuevos valores para el nombre y la edad del usuario. A continuación se muestra un ejemplo de cuerpo de petición para PUT:

```
{
  "id": 1,
  "nombre": "Nuevo nombre",
  "edad": 30
}
```

## METODO DELETE

Para utilizar el método DELETE en Postman, primero debes seleccionar DELETE en la lista de métodos HTTP en la parte superior de la ventana de Postman. Luego, debes especificar la URL de la petición en la barra de direcciones de Postman. Por último, debes especificar el cuerpo de la petición en el formato JSON en el panel "Body" de Postman. El cuerpo de la petición debe incluir el ID del usuario que se desea eliminar. A continuación se muestra un ejemplo de cuerpo de petición para DELETE:

```
{
  "id": 1
}
```


Eso por el momento, estare actualizando el codigo saludos ante cualquier duda o consulta miguel.php@gmail.com
