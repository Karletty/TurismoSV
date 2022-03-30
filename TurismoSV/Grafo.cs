using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurismoSV
{
    public class Grafo
    {
        public List<Vertice> nodos;
        public Grafo()
        {
            nodos = new List<Vertice>();
        }
        public void AgregarVertice(string valor, Double lat, Double lng)
        {
            Vertice vertice = new Vertice(valor, lat, lng);
            nodos.Add(vertice);
        }
        public void AgregarVertice(Vertice nodoNuevo)
        {
            nodos.Add(nodoNuevo);
        }
        public Vertice BuscarVertice(string valor)
        {
            return nodos.Find(v => v.ubicacion == valor);
        }
        public bool AgregarArco(string origen, string destino, Double peso)
        {
            Vertice vOrigen, vDestino;
            if ((vOrigen = nodos.Find(v => v.ubicacion == origen)) == null)
                throw new Exception($"El nodo {origen} no existe en el grafo");
            if ((vDestino = nodos.Find(v => v.ubicacion == destino)) == null)
                throw new Exception($"El nodo {destino} no existe en el grafo");
            return AgregarArco(vOrigen, vDestino, peso);
        }
        public bool AgregarArco(Vertice origen, Vertice destino, Double peso)
        {
            if(origen.listaAdyacencia.Find(v=>v.destino == destino) == null)
            {
                origen.listaAdyacencia.Add(new Arco(destino, peso));
                return true;
            }
            return false;
        }
        public Double [,] crearMatriz()
        {
            Double[,] matriz = new Double[nodos.Count, nodos.Count];
            string[,] matrizUb = new string[nodos.Count, nodos.Count];
            for (int i = 0; i < nodos.Count; i++)
            {
                Vertice nodo = nodos[i];
                for (int j = 0; j < nodos.Count; j++)
                {
                    Arco arco = nodo.listaAdyacencia.Find(v => v.destino == nodos[j]);
                    Double peso = arco.peso;
                    matriz[i, j] = peso;
                }
            }
            return matriz;
        }
    }
}
