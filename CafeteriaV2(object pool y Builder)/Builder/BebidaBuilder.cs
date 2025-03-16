using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeteriaV2_object_pool_y_Builder_
{
    class BebidaBuilder : IBebidaBuilder
    {
        private Bebida bebida = new Bebida();

        public void SetTipo(string tipo) => bebida.Tipo = tipo;
        public void SetTamaño(string tamaño) => bebida.Tamaño = tamaño;
        public void AgregarLeche() => bebida.TieneLeche = true;
        public void AgregarAzucar(int cantidad) => bebida.Azucar = cantidad;

        public Bebida GetBebida()
        {
            Bebida resultado = bebida;
            bebida = new Bebida(); // Reiniciar el builder para la próxima bebida
            return resultado;
        }
    }
}
