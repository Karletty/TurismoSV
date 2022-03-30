using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurismoSV
{
    public class Lista
    {
        private Vertice elemento;
        private Lista subLista;
        private Double peso;

        public Lista()
        {
            this.elemento = null;
            this.subLista = null;
            this.peso = 0;
        }

        public Lista(Lista pLista)
        {
            if (pLista != null)
            {
                elemento = pLista.elemento;
                subLista = pLista.subLista;
                peso = pLista.peso;
            }
        }

        public Lista(Vertice elemento, Lista subLista, Double peso)
        {
            this.elemento = elemento;
            this.subLista = subLista;
            this.peso = peso;
        }

        public Vertice Elemento
        {
            get { return elemento; }
            set { elemento = value; }
        }
        public Lista SubLista
        { 
            get { return subLista; } 
            set { subLista = value; }
        }
        public Double Peso
        {
            get { return peso; }
            set { peso = value; }
        }

        public bool EsVacia()
        {
            return elemento == null;
        }
        public bool ExisteElemento(Vertice pElemento)
        {
            if(elemento != null && pElemento != null)
                return elemento.Equals(pElemento) || subLista.ExisteElemento(pElemento);
            else return false;
        }
        public void Agregar(Vertice pElemento, Double pPeso)
        {
            if(pElemento != null)
            {
                if(elemento == null)
                {
                    elemento=new Vertice(pElemento.ubicacion, pElemento.lat, pElemento.lng);
                    peso = pPeso;
                    subLista = new Lista();
                }
                else
                {
                    if(!ExisteElemento(pElemento))
                        subLista.Agregar(pElemento, pPeso);
                }
            }
        }
        public void Eliminar(Vertice pElemento)
        {
            if (elemento != null)
            {
                if (elemento.Equals(pElemento))
                {
                    elemento = subLista.elemento;
                    subLista = subLista.subLista;
                }
                else
                    subLista.Eliminar(pElemento);
            }
        }
        public int NroElementos()
        {
            if(elemento != null)
                return 1 + subLista.NroElementos();
            else return 0;
        }
        public object IesimoElemento(int pos)
        {
            if (pos > 0 && pos <= NroElementos())
            {
                if (pos == 1)
                    return elemento;
                else
                    return subLista.IesimoElemento(pos - 1);
            }
            else
                return null;
        }
        public object IesimoElementoPeso(int pos)
        {
            if (pos > 0 && pos <= NroElementos())
            {
                if (pos == 1)
                    return peso;
                else
                    return subLista.IesimoElementoPeso(pos - 1);
            }
            else
                return 0;
        }
        public int PosElemento(Vertice pElemento)
        {
            if (elemento != null || ExisteElemento(pElemento))
            {
                if (elemento.Equals(pElemento))
                    return 1;
                else
                    return 1 + SubLista.PosElemento(pElemento);
            }
            else
                return 0;
        }
    }
}
