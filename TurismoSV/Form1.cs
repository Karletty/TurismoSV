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
            InitializeComponent();
            grafo = new Grafo();
            ObtenerUbicacion();
            Thread.Sleep(1000);
        }

        private void ConfigurarMapa()
        {
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

        private void ObtenerUbicacion()
        {
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            watcher.Start();
            Thread.Sleep(1000);

            GeoCoordinate coord = watcher.Position.Location;
            if (coord.IsUnknown != true)
            {
                latInicial = coord.Latitude;
                lngInicial = coord.Longitude;
                nodoOrigen = new Vertice("Ubicación Actual", latInicial, lngInicial);
                Punto punto = new Punto(latInicial, lngInicial, "Ubicación actual", GMarkerGoogleType.blue, ref markerOverlay, ref gMapControl1);
                puntos.Add(punto);
                grafo.AgregarVertice(punto.nombre, punto.lat, punto.lng);
                grafo.AgregarArco(punto.nombre, punto.nombre, 0);
                actualizarDgv();
            } 
            else
            {
                if (MessageBox.Show("No se pudo ubicar, desea reintentar", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ObtenerUbicacion();
                }
                else
                {
                    Application.Exit();
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            data = new DataTable();
            data.Columns.Add(new DataColumn("Descripción", typeof(string)));
            data.Columns.Add(new DataColumn("Lat", typeof(double)));
            data.Columns.Add(new DataColumn("lng", typeof(double)));

            ConfigurarMapa();
            Conexion.IngresarDatos();


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
            Double lat = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat;
            Double lng = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng;
            Punto punto = new Punto(lat, lng, $"Ubicación {puntos.Count}", GMarkerGoogleType.lightblue, ref markerOverlay, ref gMapControl1);
            grafo.AgregarVertice(punto.nombre, punto.lat, punto.lng);
            grafo.AgregarArco(punto.nombre, punto.nombre, 0);
            foreach (Punto p in puntos)
            {
                var ruta = GoogleMapProvider.Instance.GetRoute(new PointLatLng(punto.lat, punto.lng), new PointLatLng(p.lat, p.lng), false, false, 14);
                var r = new GMapRoute(ruta.Points, "Ruta");
                var rutas = new GMapOverlay("rutas");
                rutas.Routes.Add(r);
                gMapControl1.Overlays.Add(rutas);
                grafo.AgregarArco(punto.nombre, p.nombre, ruta.Distance);
                grafo.AgregarArco(p.nombre, punto.nombre, ruta.Distance);
            }
            puntos.Add(punto);
            actualizarDgv();
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            filaSeleccionada = e.RowIndex;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (filaSeleccionada >= 0 && filaSeleccionada < puntos.Count)
            {
                puntos[filaSeleccionada].EliminarMarcador(ref markerOverlay, ref gMapControl1, filaSeleccionada);
                puntos.RemoveAt(filaSeleccionada);
                actualizarDgv();
                filaSeleccionada = -1;
            }
            else
                MessageBox.Show("No ha seleccionado ninguna ubicación");
        }

        private void btnHacerRuta_Click(object sender, EventArgs e)
        {
            var ruta = GoogleMapProvider.Instance.GetRoute(new PointLatLng(puntos[0].lat, puntos[0].lng), new PointLatLng(puntos[1].lat, puntos[1].lng), false, false, 14);

            GMapRoute r = new GMapRoute(ruta.Points, "Mi ruta");
            GMapOverlay capaRutas = new GMapOverlay("rutas");
            capaRutas.Routes.Add(r);
            gMapControl1.Overlays.Add(capaRutas);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int zoom = Convert.ToInt32(gMapControl1.Zoom);
            if (zoom >= 10 && zoom <= 20)
            {
                trackZoom.Value = zoom;
            }
        }

        private void trackZoom_ValueChanged(object sender, EventArgs e)
        {
            gMapControl1.Zoom = trackZoom.Value;
        }
    }
}
