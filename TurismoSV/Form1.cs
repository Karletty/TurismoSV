using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Device.Location;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;


namespace TurismoSV
{
    public partial class Form1 : Form
    {
        private Grafo grafo = new Grafo();
        private Vertice nodoOrigen;
        GMapOverlay markerOverlay = new GMapOverlay("Marcador");
        DataTable data;
        List<Punto> puntos = new List<Punto>();
        int filaSeleccionada = -1;
        Double latInicial;
        Double lngInicial;

        public Form1()
        {
            GeolocationTests geolocationTests = new GeolocationTests();
            geolocationTests.ObtenerUbicacion();
            InitializeComponent();
            latInicial = geolocationTests.ubicacion.Lat;
            lngInicial = geolocationTests.ubicacion.Lng;
                nodoOrigen = new Vertice("Ubicación Actual", latInicial, lngInicial);
            //Añade el punto a la lista de puntos, al mapa y al grafo, conecta al punto con él mismo
            Punto punto = new Punto(latInicial, lngInicial, "Ubicación actual", GMarkerGoogleType.blue, ref markerOverlay, ref gMapControl1);
            puntos.Add(punto);
            grafo.AgregarVertice(punto.nombre, punto.lat, punto.lng);
            grafo.AgregarArco(punto.nombre, punto.nombre, 0);
            //Actualiza la tabla de puntos
            actualizarDgv();

            //Al iniciar el formulario comienza a obtener la ubicación
            //ObtenerUbicacion();
        }

        private void ConfigurarMapa()
        {
            //Configura el mapa con una apiKey, con el proovedor de  de google, permite que el mapa se arrastre con el botón izquierdo del mouse y lo ubica centrando el mapa en la ubicación actual, pone el zoom máximo y mínimo
            GMapProviders.GoogleMap.ApiKey = AppConfig.Key;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(latInicial, lngInicial);
            gMapControl1.MinZoom = 10;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 12;
            gMapControl1.AutoScroll = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Crea la información que irá en la tabla con sus columnas
            data = new DataTable();
            data.Columns.Add(new DataColumn("Descripción", typeof(string)));
            data.Columns.Add(new DataColumn("Lat", typeof(double)));
            data.Columns.Add(new DataColumn("lng", typeof(double)));

            //Configura el mapa y añade los registros a la tabla
            //Los registros se usarán para las rutas propias y los filtros
            ConfigurarMapa();
            Conexion.IngresarDatos();

            //Ingresa la posición actual a la tabla
            data.Rows.Add("Ubicación Actual", latInicial, lngInicial);
            dataGridView1.DataSource = data;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void actualizarDgv()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = puntos;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[3].Visible = false;
        }

        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Consigue la latitud y longitud según donde la persona clickeo en el mapa para luego crear un punto y dibujarlo en el mapa
            Double lat = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat;
            Double lng = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng;
            Punto punto = new Punto(lat, lng, $"Ubicación {puntos.Count}", GMarkerGoogleType.lightblue, ref markerOverlay, ref gMapControl1);
            //Agrega el punto en el grafo y lo conecta con él mismo
            grafo.AgregarVertice(punto.nombre, punto.lat, punto.lng);
            grafo.AgregarArco(punto.nombre, punto.nombre, 0);
            //Conecta el punto con los otros puntos que existan en la lista
            foreach (Punto p in puntos)
            {
                var ruta = GoogleMapProvider.Instance.GetRoute(new PointLatLng(punto.lat, punto.lng), new PointLatLng(p.lat, p.lng), false, false, 14);
                grafo.AgregarArco(punto.nombre, p.nombre, ruta.Distance);
                grafo.AgregarArco(p.nombre, punto.nombre, ruta.Distance);
            }
            //Añade el punto a la lista y actualiza la tabla de puntos
            puntos.Add(punto);
            actualizarDgv();
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //Selecciona una fila
            filaSeleccionada = e.RowIndex;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (filaSeleccionada >= 0 && filaSeleccionada < puntos.Count)
            {
                //Elimina el marcador del mapa
                puntos[filaSeleccionada].EliminarMarcador(ref markerOverlay, ref gMapControl1, filaSeleccionada);
                //Elimina el vértice del grafo
                grafo.EliminarVertice(grafo.BuscarVertice(puntos[filaSeleccionada].nombre));
                //Remueve el punto de la lista
                puntos.RemoveAt(filaSeleccionada);
                //Actualiza la tabla de puntos
                actualizarDgv();
                //Quita la selección de la tabla
                filaSeleccionada = -1;
            }
            else
                MessageBox.Show("No ha seleccionado ninguna ubicación");
        }

        private void btnHacerRuta_Click(object sender, EventArgs e)
        {
            /* PARA DIBUJAR LA RUTA, SE USARÁ AL TENER LA RUTA ARMADA
            var ruta = GoogleMapProvider.Instance.GetRoute(new PointLatLng(puntos[0].lat, puntos[0].lng), new PointLatLng(puntos[1].lat, puntos[1].lng), false, false, 14);
            var r = new GMapRoute(ruta.Points, "Ruta");
            var rutas = new GMapOverlay("rutas");
            rutas.Routes.Add(r);
            gMapControl1.Overlays.Add(rutas);
            gMapControl1.Zoom++;
            gMapControl1.Zoom--; */

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int zoom = Convert.ToInt32(gMapControl1.Zoom);
            if (zoom >= 10 && zoom <= 20)
            {
                //Actualiza la barra del zoom según el mapa
                trackZoom.Value = zoom;
            }
        }

        private void trackZoom_ValueChanged(object sender, EventArgs e)
        {
            //Actualiza el zoom del mapa según la barra
            gMapControl1.Zoom = trackZoom.Value;
        }
    }
}
