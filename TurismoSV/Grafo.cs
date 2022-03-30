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
            //Añade el vértice
            Vertice vertice = new Vertice(valor, lat, lng);
            nodos.Add(vertice);
        }

        public void AgregarVertice(Vertice nodoNuevo)
        {
            //Añade el vértice
            nodos.Add(nodoNuevo);
        }

        public void EliminarVertice(Vertice nodoEliminar)
        {
            for (int i = 0; i < nodos.Count; i++)
            {
                //Elimina las conexiones del nodo a eliminar con los demás nodos
                Arco arcoEliminar = nodos[i].listaAdyacencia.Find(v => v.destino == nodoEliminar);
                nodos[i].listaAdyacencia.Remove(arcoEliminar);
            }
            //Elimina el nodo de la lista
            nodos.Remove(nodoEliminar);
        }

        public Vertice BuscarVertice(string valor)
        {
            //Encuentra el nodo por su nombre
            return nodos.Find(v => v.ubicacion == valor);
        }

        public bool AgregarArco(string origen, string destino, Double peso)
        {
            //Busca si el nodo origen y destino existen en la lista de nodos
            Vertice vOrigen, vDestino;
            if ((vOrigen = nodos.Find(v => v.ubicacion == origen)) == null)
                throw new Exception($"El nodo {origen} no existe en el grafo");
            if ((vDestino = nodos.Find(v => v.ubicacion == destino)) == null)
                throw new Exception($"El nodo {destino} no existe en el grafo");
            //Llama a la función agregar arco con los vértices de parámetros
            return AgregarArco(vOrigen, vDestino, peso);
        }

        public bool AgregarArco(Vertice origen, Vertice destino, Double peso)
        {
            //Revisa si no ya hay una conexión entre el nodo origen y el nodo destino
            if(origen.listaAdyacencia.Find(v=>v.destino == destino) == null)
            {
                //Añade la conexión entre los dos nodos a la lista de adyacencia
                origen.listaAdyacencia.Add(new Arco(destino, peso));
                return true;
            }
            return false;
        }

        public Double [,] crearMatriz()
        {
            //Crea una matriz de adyacencia con todos los puntos del grafo
            Double[,] matriz = new Double[nodos.Count, nodos.Count];
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