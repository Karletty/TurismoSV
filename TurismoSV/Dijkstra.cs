using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurismoSV
{
    internal class Dijkstra
    {
        private int rango = 0;
        private Double[,] matrizAdy;
        private List<Vertice> nodos;
        private Double[] dist;
        public IEnumerable<Vertice> rutaMasCorta;
        public Double dmin = 0;

        public Dijkstra(int rango, Double[,] matriz, List<Vertice> vertices)
        {
            nodos = new List<Vertice>();
            this.rango = rango;
            matrizAdy = new Double[rango, rango];
            foreach (Vertice v in vertices)
            {
                nodos.Add(v);
            }
            dist = new Double[rango];
            matrizAdy = matriz;

            nodos[0].visitado = true;
            Vertice inicio = nodos[0];
            nodos.RemoveAt(0);
            IEnumerable<IEnumerable<Vertice>> result = ObtenerPermutaiones(vertices, vertices.Count);
            int indiceDmenor;
            int vuelta = 1;
            Double d = 0;
            dmin = Double.MaxValue;
            indiceDmenor = 0;
            for (int i = 0; i < result.Count(); i++)
            {
                vuelta = 1;
                d = 0;
                int j = 0;
                IEnumerable<Vertice> vs = result.ElementAt(i);
                while (d <= dmin && j < vs.Count())
                {
                    if (vuelta == 1)
                    {
                        d = Distancia(inicio, vs.ElementAt(j));
                    }
                    else
                    {
                        d += Distancia(vs.ElementAt(j - 1), vs.ElementAt(j));
                    }
                    vs.ElementAt(j).visitado = true;
                    vuelta++;
                    j++;
                }
                if (d <= dmin && TodosVisitados(vs))
                {
                    dmin = d;
                    indiceDmenor = i;
                }
            }
            string txt = "";
            rutaMasCorta = result.ElementAt(indiceDmenor);
            foreach (Vertice v in result.ElementAt(indiceDmenor))
            {
                txt += $"{ v.ToString() }, ";
            }
        }
        public bool TodosVisitados(IEnumerable<Vertice> vertices)
        {
            for (int i = 0; i < vertices.Count(); i++)
            {
                if (!vertices.ElementAt(i).visitado)
                    return false;
            }
            return true;
        }

        static IEnumerable<IEnumerable<T>> ObtenerPermutaiones<T>(IEnumerable<T> list, int length)
        {
            if (length == 1)
                return list.Select(t => new T[] { t });
            return ObtenerPermutaiones(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
        }
        public Double Distancia(Vertice a, Vertice b)
        {
            for (int i = 0; i < a.listaAdyacencia.Count; i++)
            {
                if (a.listaAdyacencia[i].destino == b)
                    return a.listaAdyacencia[i].peso;
            }
            return -1;
        }
    }
}
