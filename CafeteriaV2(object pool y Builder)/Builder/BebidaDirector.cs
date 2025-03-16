using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeteriaV2_object_pool_y_Builder_
{
    class BebidaDirector
    {
        private IBebidaBuilder _builder;

        public BebidaDirector(IBebidaBuilder builder)
        {
            _builder = builder;
        }

        public Bebida ConstruirBebida(string tipo, string tamaño, bool leche, int azucar)
        {
            _builder.SetTipo(tipo);
            _builder.SetTamaño(tamaño);
            if (leche) _builder.AgregarLeche();
            _builder.AgregarAzucar(azucar);

            return _builder.GetBebida();
        }
    }
}
