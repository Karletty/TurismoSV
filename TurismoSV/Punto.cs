using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

namespace TurismoSV
{
    public class Punto
    {
        public string nombre { get; set; }
        public Double lat { get; set; }
        public Double lng { get; set; }
        public GMarkerGoogleType tipo { get; set; }

        public void EliminarMarcador(ref GMapOverlay capaMarcadores, ref GMapControl gmap, int pos)
        {
            //Elimina el marcador del gMapControl
            capaMarcadores.Markers.RemoveAt(pos);
            gmap.Overlays.Remove(capaMarcadores);
            gmap.Overlays.Add(capaMarcadores);
        }

        private void CrearMarcador(ref GMapOverlay capaMarcadores, ref GMapControl gmap)
        {
            //Pone el marcador en el gMapControl
            GMarkerGoogle marcador = new GMarkerGoogle(new PointLatLng(lat, lng), tipo);
            capaMarcadores.Markers.Add(marcador);

            marcador.ToolTipMode = MarkerTooltipMode.Always;
            marcador.ToolTipText = nombre;

            gmap.Overlays.Add(capaMarcadores);
        }

        public Punto(Double lat, Double lng, string nombre, GMarkerGoogleType tipo, ref GMapOverlay capaMarcadores, ref GMapControl gmap)
        {
            //Construye el punto y manda a ponerlo en el mapa
            this.lat = lat;
            this.lng = lng;
            this.nombre = nombre;
            this.tipo = tipo;
            CrearMarcador(ref capaMarcadores, ref gmap);
        }
    }
}
