using CafeteriaV2_object_pool_y_Builder_.Pool_Object;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CafeteriaV2_object_pool_y_Builder_
{
    class Program
    {
        static ClientePool clientePool = new ClientePool();
        static List<Cliente> clientesEnEspera = new List<Cliente>();
        //static Timer timerClientes;
        static Thread hiloClientes;
        static byte Etapa = 0;
        static bool ejecutando = true;
        static int clientesAtendidos = 0;
        static int clientesPerdidos = 0;


        private static void Main(string[] args)
        {
            Console.WriteLine("Los clientes llegarán automáticamente cada 15 segundos...");
            IniciarHiloClientes();
            ManejarInterfazUsuario();

            if(clientesPerdidos > 4)
                Console.WriteLine($"Echale mas ganitas a la chamba >:c  \n" +
                    $"Clientes atendidos: {clientesAtendidos}\n" +
                    $"Clientes perdidos: {clientesPerdidos}\n");
            else
                Console.WriteLine($"Buena jornada de trabajo  \n" +
                    $"Clientes atendidos: {clientesAtendidos}\n" +
                    $"Clientes perdidos: {clientesPerdidos}\n");
            Console.ReadKey();
        }

        private static void IniciarHiloClientes()
        {
            hiloClientes = new Thread(GenerarClientes);
            hiloClientes.IsBackground = true;
            hiloClientes.Start();
        }

        private static void ManejarInterfazUsuario()
        {
            while (true)
            {
                string opcion = SolicitarOpcion();

                if (opcion == "1")
                {
                    if (clientesEnEspera.Count > 0)
                    {
                        Etapa = 1;
                        PrepararBebida();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No hay clientes en espera.");
                        Console.ResetColor();
                    }
                }
                else if (opcion == "2")
                {
                    Console.Clear();
                    Console.WriteLine("Saliendo...");

                    ejecutando = false;
                    hiloClientes.Join();  // Esperar que termine el hilo
                    break;
                }
            }
        }

        private static void GenerarClientes()
        {
            while (ejecutando)
            {
                lock (clientesEnEspera) // Evita problemas de concurrencia
                {
                    MostrarInterfaz();
                    if (clientesEnEspera.Count < 10) // Límite de 10 clientes en espera
                    {
                        Cliente nuevoCliente = clientePool.ObtenerCliente();
                        clientesEnEspera.Add(nuevoCliente);
                        Console.WriteLine($"\nCliente {nuevoCliente.Nombre} ha llegado: {nuevoCliente.MostrarPedido()}");

                        RetomarPreparacion();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n¡La cafetería está llena! No se pueden aceptar más clientes.");
                        Console.ResetColor();
                    }
                }
                Random aleatorio = new Random();
                Thread.Sleep(aleatorio.Next(5000,8000)); // Esperar 15 segundos antes de generar otro cliente
            }
        }
        
        private static string SolicitarOpcion()
        {
            RetomarPreparacion();
            return Console.ReadLine();
        }

        private static void PrepararBebida()
        {
            Cliente cliente = clientesEnEspera[0];
            MostrarInterfaz();

            try
            {
                string tipo = SolicitarTipoCafe();Etapa = 2;
                string tamaño = SolicitarTamaño(); Etapa = 3;
                bool leche = SolicitarLeche(); Etapa = 4;
                int azucar = SolicitarAzucar();
                Etapa = 0;

                Bebida bebida = ConstruirBebida(tipo, tamaño, leche, azucar);
                EntregarBebida(cliente, bebida); 
            }
            catch(Exception e)
            {
                Console.WriteLine("Se ha ingresado un dato incorrectamente.");
                Etapa = 0;
            }

        }

        private static string SolicitarTipoCafe()
        {
            RetomarPreparacion();
            return InterpretarValor(byte.Parse(Console.ReadLine()), "tipo");
        }

        private static string SolicitarTamaño()
        {
            RetomarPreparacion();
            return InterpretarValor(byte.Parse(Console.ReadLine()), "tamaño");
        }

        private static bool SolicitarLeche()
        {
            RetomarPreparacion();
            return Console.ReadLine().ToLower() == "s";
        }

        private static int SolicitarAzucar()
        {
            RetomarPreparacion();
            return int.Parse(Console.ReadLine());
        }

        private static Bebida ConstruirBebida(string tipo, string tamaño, bool leche, int azucar)
        {
            BebidaDirector director = new BebidaDirector(new BebidaBuilder());
            return director.ConstruirBebida(tipo, tamaño, leche, azucar);
        }

        private static void EntregarBebida(Cliente cliente, Bebida bebida)
        {
            cliente.RecibirBebida(bebida);

            if (cliente.CompararBebidas())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nCliente {cliente.Nombre} está feliz con su {bebida.BebidaConstruida()}.");
                Console.ResetColor();
                //clientesEnEspera.Remove(cliente);
                //clientePool.LiberarCliente(cliente);
                clientesAtendidos++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nERROR: La bebida no es la que el cliente pidió.");
                Console.ResetColor();
                Console.WriteLine($"El cliente quería: {cliente.MostrarPedido()}");
                Console.WriteLine($"Le preparaste    : {bebida.BebidaConstruida()}");
                Console.WriteLine("Vuelve a intentarlo.");
                clientesPerdidos++;
            }

            clientesEnEspera.Remove(cliente);
            clientePool.LiberarCliente(cliente);
        }

        private static void MostrarInterfaz()
        {
            Console.Clear();
            Console.WriteLine("- - - - Bienvenido a la Cafetería - - - -");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("        -> Tu eres el cafetero <-\n");
            Console.ResetColor();

            MostrarEstado();
        }

        private static string InterpretarValor(byte eleccion, string criterio)
        {
            if (criterio == "tipo")
            {
                if (eleccion == 1) return "Americano";
                if (eleccion == 2) return "Capuchino";
                else return "Espresso";
            }
            else
            {
                if (eleccion == 1) return "Pequeño";
                if (eleccion == 2) return "Mediano";
                else return "Grande";
            }
        }

        private static void MostrarEstado()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\nClientes en espera:");
            Console.ResetColor();

            if (clientesEnEspera.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  No hay clientes en la cafetería.");
                Console.ResetColor();

            }
            else
            {
                foreach (var cliente in clientesEnEspera)
                    Console.WriteLine($"\t - Cliente {cliente.Nombre} (ID: {cliente.Id}) espera {cliente.MostrarPedido()}");
            }
        }

        private static void RetomarPreparacion()
        {
            if(clientesEnEspera.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"\nPreparando bebida para {clientesEnEspera[0].Nombre} (Pedido: {clientesEnEspera[0].MostrarPedido()})");
                Console.ResetColor();
            }

            switch (Etapa)
            {
                case 0:
                    Console.Write("\n1. Preparar bebida" +
                                  "\n2. Salir" +
                                  "\n-> ");
                    break;
                case 1:
                    Console.Write("Ingrese el tipo de café \n" +
                                  "1) Americano\n" +
                                  "2) Capuchino\n" +
                                  "3) Espresso\n" +
                                  "-> ");
                    break;
                case 2:
                    Console.Write("Ingrese el tamaño\n" +
                                  "1) Pequeño\n" +
                                  "2) Mediano\n" +
                                  "3) Grande\n" +
                                  "-> ");
                    break;
                case 3:
                    Console.Write("¿Agregar leche? (s/n): ");
                    break;
                case 4:
                    Console.Write("¿Cuántas cucharadas de azúcar? (0-3): ");
                    break;
                default:
                    Console.WriteLine("Error: Estado desconocido.");
                    break;
            }
        }


    }
}
