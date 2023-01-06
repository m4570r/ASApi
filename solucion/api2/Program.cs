using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

class Server
{
    static void Main(string[] args)
    {
        Console.WriteLine("\u001b[0mBienvenido, ASApi");
        Logger logger = new Logger();
        logger.WriteLog("Bienvenido, ASApi.");
        Server server = new Server();
        //Console.WriteLine("Servidor Iniciado");
        logger.WriteLog("Servidor Iniciado");
        server.Start();
    }
    private readonly HttpListener _listener = new HttpListener();
    public void Start()
    {
        Logger logger = new Logger();
        settings conf = settings.config();
        //Console.WriteLine($"Se han habilitado las siguientes rutas para el servidor: {conf.url}.");
        _listener.Prefixes.Add(conf.url);
        //Console.WriteLine("Servidor Escuchando");
        _listener.Start();
        Console.WriteLine("Servidor iniciado en http://localhost:80/");
        logger.WriteLog("en http://localhost:80/");
        while (true)
        {
            // Espera a que llegue una solicitud
            Console.WriteLine("________________________________________________");
            logger.WriteLog("________________________________________________");
            var context = _listener.GetContext();
            // Manda a manejar la solicitud en otro hilo
            var thread = new System.Threading.Thread(HandleRequest);
            thread.Start(context);
        }
    }
    private void HandleRequest(object context)
    {
        Logger logger = new Logger();
        var ctx = (HttpListenerContext)context;
        settings conf = settings.config();
        string userAgent = ctx.Request.UserAgent;
        string browserOs = UserAgent(userAgent);

        if (ctx.Request.RawUrl != "/favicon.ico")
        {
            try
            {
                // Conecta a la base de datos MySQL
                var connString = conf.dbConexion;
                using (var conn = new MySqlConnection(connString))
                {
                    //Console.WriteLine($"Conectando Base de datos");
                    logger.WriteLog("..........:::::: Nueva Solicitud ::::::..........");
                    conn.Open();
                    var request = ctx.Request;
                    //Console.WriteLine($"METODO : {ctx.Request.HttpMethod}.");
                    //logger.WriteLog($"METODO : {ctx.Request.HttpMethod}.");
                    // Si la solicitud es un GET a la ruta "/version"
                    if (ctx.Request.HttpMethod == "GET" && ctx.Request.RawUrl == conf.endpoint)
                    {
                        // Crea un objeto con la información solicitada
                        var data = new { version = conf.mensaje };
                        // Console.WriteLine($"mensaje: {conf.mensaje}");
                        //logger.WriteLog($"\nRESPUESTA: \n\n{conf.mensaje} \n");
                        // Convierte el objeto a formato JSON
                        var json = JsonConvert.SerializeObject(data, Formatting.Indented);                        
                        // Crea una respuesta para enviar al navegador
                        var response = Encoding.UTF8.GetBytes(json);
                        ctx.Response.StatusCode = 200;
                        ctx.Response.ContentType = "application/json";
                        ctx.Response.ContentLength64 = response.Length;
                        ctx.Response.OutputStream.Write(response, 0, response.Length);
                        // Cierra la respuesta al navegador
                        ctx.Response.OutputStream.Close();
                        //Console.WriteLine("Cerrando conexion a bd.");
                        //logger.WriteLog($"Cerrando conexion a bd.");
                        Console.WriteLine("\u001b[0m["+browserOs);
                        Console.WriteLine($"\x1b[34m[REQUEST]:\x1b[0m \u001b[32m[{ctx.Request.HttpMethod}] - \u001b[0m[{ctx.Request.Url}] - \u001b[31m[RESPONSE]:\u001b[0m \u001b[32m[{ctx.Response.StatusCode}]");
                        //Console.WriteLine($"\x1b[0m {json}");                     
                        logger.WriteLog("NAVEGADOR: " + browserOs);
                        logger.WriteLog($"DETALLE:[{ctx.Request.HttpMethod}] - [{ctx.Request.Url}] - [RESPONSE]:[{ctx.Response.StatusCode}]");
                        logger.WriteLog($"RESPUESTA: \n{json}");

                    }
                    else if (ctx.Request.HttpMethod == "GET")
                    {
                        //Console.WriteLine($"Metodo: {ctx.Request.HttpMethod}.");
                        try
                        {
                            // Obtener los parámetros de la solicitud
                            string id = request.QueryString["id"];
                            string nombre = request.QueryString["nombre"];
                            string edad = request.QueryString["edad"];
                            // Crear la consulta a la base de datos
                            string query = "SELECT * FROM usuarios";
                            if (id != null)
                            {
                                query += " WHERE id = @id";
                            }
                            else if (nombre != null)
                            {
                                query += " WHERE nombre = @nombre";
                            }
                            else if (edad != null)
                            {
                                query += " WHERE edad = @edad";
                            }
                            // Ejecutar la consulta
                            var cmd = new MySqlCommand(query, conn);
                            if (id != null)
                            {
                                cmd.Parameters.AddWithValue("@id", id);
                            }
                            else if (nombre != null)
                            {
                                cmd.Parameters.AddWithValue("@nombre", nombre);
                            }
                            else if (edad != null)
                            {
                                cmd.Parameters.AddWithValue("@edad", edad);
                            }
                            var reader = cmd.ExecuteReader();
                            // Crear una lista para almacenar los resultados
                            var results = new List<Dictionary<string, object>>();
                            // Recorrer los resultados y agregarlos a la lista
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row.Add(reader.GetName(i), reader.GetValue(i));
                                }
                                results.Add(row);
                            }
                            // Cerrar la conexión a la base de datos
                            reader.Close();
                            // Convertir la lista a formato JSON
                            var json = JsonConvert.SerializeObject(results, Formatting.Indented);
                            // Crear una respuesta para enviar al navegador
                            var response = Encoding.UTF8.GetBytes(json);
                            ctx.Response.StatusCode = 200;
                            ctx.Response.ContentType = "application/json";
                            ctx.Response.ContentLength64 = response.Length;
                            Console.WriteLine("\u001b[0m["+browserOs);
                            Console.WriteLine($"\x1b[34m[REQUEST]:\x1b[0m \u001b[32m[{ctx.Request.HttpMethod}] - \u001b[0m[{ctx.Request.Url}] - \u001b[31m[RESPONSE]:\u001b[0m \u001b[32m[{ctx.Response.StatusCode}]");
                            //Console.WriteLine($"\x1b[0m {json}");
                            logger.WriteLog("NAVEGADOR: " + browserOs);
                            logger.WriteLog($"DETALLE:[{ctx.Request.HttpMethod}] - [{ctx.Request.Url}] - [RESPONSE]:[{ctx.Response.StatusCode}]");
                            logger.WriteLog($"RESPUESTA: \n{json}");
                            using (var writer = new StreamWriter(ctx.Response.OutputStream))
                            {
                                writer.Write(json);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Devolver una respuesta de error
                            var responseData = new
                            {
                                status = "error",
                                message = ex.Message
                            };
                            var json = JsonConvert.SerializeObject(responseData, Formatting.Indented);
                            var response = Encoding.UTF8.GetBytes(json);
                            ctx.Response.StatusCode = 500;
                            Console.WriteLine("\u001b[0m["+browserOs);
                            Console.WriteLine($"\x1b[34m[REQUEST]:\x1b[0m \u001b[32m[{ctx.Request.HttpMethod}] - \u001b[0m[{ctx.Request.Url}] - \u001b[31m[RESPONSE]:\u001b[0m \u001b[32m[{ctx.Response.StatusCode}]");
                            //Console.WriteLine($"\x1b[0m {json}");
                            logger.WriteLog("NAVEGADOR: " + browserOs);
                            logger.WriteLog($"DETALLE:[{ctx.Request.HttpMethod}] - [{ctx.Request.Url}] - [RESPONSE]:[{ctx.Response.StatusCode}]");
                            logger.WriteLog($"RESPUESTA: \n{json}");
                            ctx.Response.ContentType = "application/json";
                            ctx.Response.ContentLength64 = response.Length;
                            using (var writer = new StreamWriter(ctx.Response.OutputStream))
                            {
                                writer.Write(json);
                            }
                        }
                    }
                    if (request.HttpMethod == "POST")
                    {
                        try
                        {
                            // Obtener el cuerpo de la solicitud
                            string body = new StreamReader(request.InputStream).ReadToEnd();
                            // Deserializar el cuerpo de la solicitud a un objeto
                            var data = JsonConvert.DeserializeObject<dynamic>(body);
                            // Insertar el recurso en la base de datos
                            string query = "INSERT INTO usuarios (nombre, edad) VALUES (@nombre, @edad)";
                            var cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@nombre", data.nombre);
                            cmd.Parameters.AddWithValue("@edad", data.edad);
                            cmd.ExecuteNonQuery();
                            // Devolver una respuesta de éxito con el ID del nuevo recurso y sus datos
                            var responseData = new
                            {
                                status = "success",
                                id = cmd.LastInsertedId,
                                nombre = data.nombre,
                                edad = data.edad,
                                message = "Se ha creado un nuevo usuario"
                            };
                            var json = JsonConvert.SerializeObject(responseData, Formatting.Indented);
                            var response = Encoding.UTF8.GetBytes(json);
                            ctx.Response.StatusCode = 200;
                            ctx.Response.ContentType = "application/json";
                            ctx.Response.ContentLength64 = response.Length;
                            Console.WriteLine("\u001b[0m["+browserOs);
                            Console.WriteLine($"\x1b[34m[REQUEST]:\x1b[0m \u001b[32m[{ctx.Request.HttpMethod}] - \u001b[0m[{ctx.Request.Url}] - \u001b[31m[RESPONSE]:\u001b[0m \u001b[32m[{ctx.Response.StatusCode}]");
                            //Console.WriteLine($"[PAYLOAD]: \n {body}");
                            logger.WriteLog("NAVEGADOR: " + browserOs);
                            logger.WriteLog($"DETALLE:[{ctx.Request.HttpMethod}] - [{ctx.Request.Url}] - [RESPONSE]:[{ctx.Response.StatusCode}]");
                            logger.WriteLog($"PAYLOAD: \n{body}");
                            logger.WriteLog($"RESPUESTA: \n{json}");
                            using (var writer = new StreamWriter(ctx.Response.OutputStream))
                            {
                                writer.Write(json);
                                //Console.WriteLine($"\x1b[0m {json}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Devolver una respuesta de error
                            var responseData = new
                            {
                                status = "error",
                                message = ex.Message
                            };
                            var json = JsonConvert.SerializeObject(responseData, Formatting.Indented);
                            var response = Encoding.UTF8.GetBytes(json);
                            ctx.Response.StatusCode = 500;
                            ctx.Response.ContentType = "application/json";
                            ctx.Response.ContentLength64 = response.Length;
                            Console.WriteLine("\u001b[0m["+browserOs);
                            Console.WriteLine($"\x1b[34m[REQUEST]:\x1b[0m \u001b[32m[{ctx.Request.HttpMethod}] - \u001b[0m[{ctx.Request.Url}] - \u001b[31m[RESPONSE]:\u001b[0m \u001b[32m[{ctx.Response.StatusCode}]");
                            //Console.WriteLine($"\x1b[0m {json}");
                            logger.WriteLog("NAVEGADOR: " + browserOs);
                            logger.WriteLog($"DETALLE:[{ctx.Request.HttpMethod}] - [{ctx.Request.Url}] - [RESPONSE]:[{ctx.Response.StatusCode}]");
                            logger.WriteLog($"RESPUESTA: \n{json}");
                            using (var writer = new StreamWriter(ctx.Response.OutputStream))
                            {
                                writer.Write(json);
                            }
                        }
                    }
                    if (request.HttpMethod == "PUT")
                    {
                        try
                        {
                            // Obtener el cuerpo de la solicitud
                            string body = new StreamReader(request.InputStream).ReadToEnd();
                            // Deserializar el cuerpo de la solicitud a un objeto
                            var data = JsonConvert.DeserializeObject<dynamic>(body);
                            // Obtener el ID del recurso a actualizar
                            string id = data.id;
                            // Actualizar el recurso en la base de datos
                            string query = "UPDATE usuarios SET nombre = @nombre, edad = @edad WHERE id = @id";
                            var cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@nombre", data.nombre);
                            cmd.Parameters.AddWithValue("@edad", data.edad);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                            // Devolver una respuesta de éxito
                            var responseData = new
                            {
                                status = "success",
                                data = new
                                {
                                    id = id,
                                    nombre = data.nombre,
                                    edad = data.edad
                                }
                            };
                            var json = JsonConvert.SerializeObject(responseData, Formatting.Indented);
                            var response = Encoding.UTF8.GetBytes(json);
                            ctx.Response.StatusCode = 200;
                            ctx.Response.ContentType = "application/json";
                            ctx.Response.ContentLength64 = response.Length;
                                                        Console.WriteLine("\u001b[0m["+browserOs);
                            Console.WriteLine($"\x1b[34m[REQUEST]:\x1b[0m \u001b[32m[{ctx.Request.HttpMethod}] - \u001b[0m[{ctx.Request.Url}] - \u001b[31m[RESPONSE]:\u001b[0m \u001b[32m[{ctx.Response.StatusCode}]");
                            //Console.WriteLine($"[PAYLOAD]: \n {body}";
                            logger.WriteLog("NAVEGADOR: " + browserOs);
                            logger.WriteLog($"DETALLE:[{ctx.Request.HttpMethod}] - [{ctx.Request.Url}] - [RESPONSE]:[{ctx.Response.StatusCode}]");
                            logger.WriteLog($"PAYLOAD: \n{body}");
                            logger.WriteLog($"RESPUESTA: \n{json}");
                            using (var writer = new StreamWriter(ctx.Response.OutputStream))
                            {
                                writer.Write(json);
                                //Console.WriteLine($"\x1b[0m {json}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Devolver una respuesta de error
                            var responseData = new
                            {
                                status = "error",
                                message = ex.Message
                            };
                            var json = JsonConvert.SerializeObject(responseData);
                            var response = Encoding.UTF8.GetBytes(json);
                            ctx.Response.StatusCode = 500;
                            ctx.Response.ContentType = "application/json";
                            ctx.Response.ContentLength64 = response.Length;
                            Console.WriteLine("\u001b[0m["+browserOs);
                            Console.WriteLine($"\x1b[34m[REQUEST]:\x1b[0m \u001b[32m[{ctx.Request.HttpMethod}] - \u001b[0m[{ctx.Request.Url}] - \u001b[31m[RESPONSE]:\u001b[0m \u001b[32m[{ctx.Response.StatusCode}]");
                            //Console.WriteLine($"\x1b[0m {json}");
                            logger.WriteLog("NAVEGADOR: " + browserOs);
                            logger.WriteLog($"DETALLE:[{ctx.Request.HttpMethod}] - [{ctx.Request.Url}] - [RESPONSE]:[{ctx.Response.StatusCode}]");
                            logger.WriteLog($"RESPUESTA: \n{json}");
                            using (var writer = new StreamWriter(ctx.Response.OutputStream))
                            {
                                writer.Write(json);
                            }
                        }
                    }
                    if (request.HttpMethod == "DELETE")
                    {
                        try
                        {
                            // Obtener el cuerpo de la solicitud
                            string body = new StreamReader(request.InputStream).ReadToEnd();
                            // Deserializar el cuerpo de la solicitud a un objeto
                            var data = JsonConvert.DeserializeObject<dynamic>(body);
                            // Obtener el ID del recurso a eliminar
                            string id = data.id;
                            // Eliminar el recurso de la base de datos
                            string query = "DELETE FROM usuarios WHERE id = @id";
                            var cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                            // Devolver una respuesta de éxito
                            var responseData = new
                            {
                                status = "success"
                            };
                            var json = JsonConvert.SerializeObject(responseData);
                            var response = Encoding.UTF8.GetBytes(json);
                            ctx.Response.StatusCode = 200;
                            ctx.Response.ContentType = "application/json";
                            ctx.Response.ContentLength64 = response.Length;
                            Console.WriteLine("\u001b[0m["+browserOs);
                            Console.WriteLine($"\x1b[34m[REQUEST]:\x1b[0m \u001b[32m[{ctx.Request.HttpMethod}] - \u001b[0m[{ctx.Request.Url}] - \u001b[31m[RESPONSE]:\u001b[0m \u001b[32m[{ctx.Response.StatusCode}]");
                            //Console.WriteLine($"[PAYLOAD]: \n {body}");
                            logger.WriteLog("NAVEGADOR: " + browserOs);
                            logger.WriteLog($"DETALLE:[{ctx.Request.HttpMethod}] - [{ctx.Request.Url}] - [RESPONSE]:[{ctx.Response.StatusCode}]");
                            logger.WriteLog($"PAYLOAD: \n{body}");
                            logger.WriteLog($"RESPUESTA: \n{json}");
                            using (var writer = new StreamWriter(ctx.Response.OutputStream))
                            {
                                writer.Write(json);
                                //Console.WriteLine($"\x1b[0m {json}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Devolver una respuesta de error
                            var responseData = new
                            {
                                status = "error",
                                message = ex.Message
                            };
                            var json = JsonConvert.SerializeObject(responseData);
                            var response = Encoding.UTF8.GetBytes(json);
                            ctx.Response.StatusCode = 500;
                            ctx.Response.ContentType = "application/json";
                            ctx.Response.ContentLength64 = response.Length;
                            Console.WriteLine("\u001b[0m[" + browserOs);
                            Console.WriteLine($"\x1b[34m[REQUEST]:\x1b[0m \u001b[32m[{ctx.Request.HttpMethod}] - \u001b[0m[{ctx.Request.Url}] - \u001b[31m[RESPONSE]:\u001b[0m \u001b[32m[{ctx.Response.StatusCode}]");
                            //Console.WriteLine($"\x1b[0m {json}");
                            logger.WriteLog("NAVEGADOR: " + browserOs);
                            logger.WriteLog($"DETALLE:[{ctx.Request.HttpMethod}] - [{ctx.Request.Url}] - [RESPONSE]:[{ctx.Response.StatusCode}]");
                            logger.WriteLog($"RESPUESTA: \n{json}");
                            using (var writer = new StreamWriter(ctx.Response.OutputStream))
                            {
                                writer.Write(json);
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // En caso de que ocurra un error, devuelve una respuesta con el código de error y la descripción
                ctx.Response.StatusCode = 500;
                ctx.Response.ContentType = "application/json";
                using (var writer = new StreamWriter(ctx.Response.OutputStream))
                {
                    writer.Write(JsonConvert.SerializeObject(new { error = ex.Message }));
                }
            }
        }
    }
    public class Logger
    {
        public void WriteLog(string logMessage)
        {
            string logPath = "C:\\api\\ASApi\\ASApi.log";

            DateTime currentDate = DateTime.Now;
            string dateString = currentDate.ToString("yyyy-MM-dd HH:mm:ss");
            string logLine = dateString + ": " + logMessage;

            StreamWriter sw = new StreamWriter(logPath, true);
            sw.WriteLine(logLine);
            sw.Close();
        }
    }
    public string UserAgent(string userAgent)
    {
        // Usamos la expresión regular para buscar el navegador
        string browserPattern = @"([^\s]*)\/";
        Match browserMatch = Regex.Match(userAgent, browserPattern);
        string browser = browserMatch.Groups[1].Value;

        // Usamos la expresión regular para buscar el sistema operativo
        string osPattern = @"\(([^\)]*)\)";
        Match osMatch = Regex.Match(userAgent, osPattern);
        string os = osMatch.Groups[1].Value;

        // Devolvemos la información en formato "navegador (sistema operativo)"
        return browser + " (" + os + ")";
    }
    public class settings
    {
        public static settings config()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            settings conf = new settings();
            configuration.GetSection(settings.configuraciones).Bind(conf);
            return conf;
        }
        public const string configuraciones = "configuraciones";
        public string url
        {
            get; set;
        }
        public string dbConexion
        {
            get; set;
        }
        public string endpoint
        {
            get; set;
        }
        public string mensaje
        {
            get; set;
        }
    }
}
