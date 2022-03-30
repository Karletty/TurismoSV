using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurismoSV
{
    public class Vertice
    {
        public string ubicacion;
        public Double lat;
        public Double lng;
        public List<Arco> listaAdyacencia;

        public Vertice(string ubicacion, Double lat, Double lng)
        {
            //Crea un vértice
            this.ubicacion = ubicacion;
            this.lat = lat;
            this.lng = lng;
            this.listaAdyacencia = new List<Arco>();
        }
        public Vertice(): this("", 0, 0)
        {
        }
        public override string ToString()
        {
            return this.ubicacion;
        }
    }
}
