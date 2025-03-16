using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeteriaV2_object_pool_y_Builder_.Pool_Object
{
    class ClientePool
    {
        private Queue<Cliente> PoolClientes = new Queue<Cliente>();
        private int id = 1;
        private readonly object lockObject = new object();
        private static Random aleatorio = new Random();

        private string[] nombres = {
            "Maria", "Karla", "Edwin", "Angel", "Wills", "Viviana", "Pedro", "Manuel", "Berenice", "Jose", "Patricio", "Miguel" };


        public Cliente ObtenerCliente()
        {
            lock (lockObject)
            {
                string nombre = nombres[aleatorio.Next(nombres.Length)];
                if (PoolClientes.Count > 0)
                {
                    Cliente cliente = PoolClientes.Dequeue();
                    cliente.Nombre = nombre;
                    cliente.Pedido = GenerarBebidaAleatoria();
                    return cliente;
                }
                return new Cliente(id++, nombre, GenerarBebidaAleatoria());
            }
        }

        public void LiberarCliente(Cliente cliente)
        {
            lock (lockObject)
            {
                cliente.BebidaRecibida = null;
                cliente.Pedido = null;
                PoolClientes.Enqueue(cliente);
            }
        }

       
        private static BebidaDirector director = new BebidaDirector(new BebidaBuilder());

        //Asignar pedido del cliente
        static Bebida GenerarBebidaAleatoria()
        {
            string[] tipos = { "Americano", "Capuchino", "Espresso" };
            string[] tamaños = { "Pequeño", "Mediano", "Grande" };

            string BebidaSeleccionada = tipos[aleatorio.Next(tipos.Length)];

            if (aleatorio.Next(2) == 1 || BebidaSeleccionada == "Capuchino")

                return director.ConstruirBebida(BebidaSeleccionada,
                                                tamaños[aleatorio.Next(tamaños.Length)],
                                                true, 
                                                aleatorio.Next(4));
            else
                return director.ConstruirBebida(BebidaSeleccionada,
                                                tamaños[aleatorio.Next(tamaños.Length)],
                                                false, 
                                                aleatorio.Next(4));

        }
    }
}
