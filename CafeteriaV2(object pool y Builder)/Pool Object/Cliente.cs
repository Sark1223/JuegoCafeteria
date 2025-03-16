using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeteriaV2_object_pool_y_Builder_.Pool_Object
{
    class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Bebida Pedido { get; set; }
        public Bebida BebidaRecibida { get; set; }

        public Cliente(int id, string nombre, Bebida pedido)
        {
            Id = id;
            Nombre = nombre;
            Pedido = pedido;
        }

        public string MostrarPedido()
        {
            return $"Quiero un cafe {Pedido.Tipo} de tamaño {Pedido.Tamaño} {(Pedido.TieneLeche ? "con leche " : "")}y {Pedido.Azucar} de azucar";
        }

        public void RecibirBebida(Bebida bebida)
        {
            BebidaRecibida = bebida;
        }

        public bool CompararBebidas()
        {
            return Pedido.Tipo == BebidaRecibida.Tipo &&
                   Pedido.Tamaño == BebidaRecibida.Tamaño &&
                   Pedido.TieneLeche == BebidaRecibida.TieneLeche &&
                   Pedido.Azucar == BebidaRecibida.Azucar;
        }
    }
}
