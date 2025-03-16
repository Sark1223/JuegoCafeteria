using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading;

namespace CafeteriaV2_object_pool_y_Builder_
{
    class Bebida
    {
        public string Tipo { get; set; }
        public string Tamaño { get; set; }
        public bool TieneLeche { get; set; }
        public int Azucar { get; set; }

        public string BebidaConstruida()
        {
            return $"cafe {Tipo} de tamaño {Tamaño} {(TieneLeche ? "con leche " : "")}y {Azucar} de azucar";
        }
    }
}
