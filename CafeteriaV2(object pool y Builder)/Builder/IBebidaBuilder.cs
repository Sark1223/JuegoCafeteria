using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeteriaV2_object_pool_y_Builder_
{
    interface IBebidaBuilder
    {
        void SetTipo(string tipo);
        void SetTamaño(string tamaño);
        void AgregarLeche();
        void AgregarAzucar(int cantidad);
        Bebida GetBebida();
    }
}
