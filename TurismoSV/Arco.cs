using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurismoSV
{
    public class Arco
    {
        public Vertice destino;
        public Double peso;

        public Arco(Vertice destino, Double peso)
        {
            this.destino = destino;
            this.peso = peso;
        }
    }
}
