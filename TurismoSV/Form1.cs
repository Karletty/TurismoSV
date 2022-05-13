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
using GoogleApi;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Geolocation.Request;
using GoogleApi.Entities.Places.Common.Enums;
using GoogleApi.Entities.Places.Search.Common.Enums;
using GoogleApi.Entities.Places.Search.NearBy.Request;


namespace TurismoSV
{
    public partial class Form1 : Form
    {
        enum Categorias
        {
            Playa,
            Parque,
            Bosque,
            Lago,
            Zoo,
            Mall,
            Restaurante,
            Estadio,
            Museo,
            SitioArq,
            Iglesia,
            Teatro
        }

        private Grafo grafo = new Grafo();
        private bool hayRuta = false;
        IEnumerable<Vertice> rutaMasCorta;
        private double dist = 0;
        private FrmMatrizAdy frmMatriz;
        private FrmPago frmPago;
        private Vertice nodoOrigen;
        GMapOverlay markerOverlay = new GMapOverlay("Marcador");
        GMapOverlay ourPoints = new GMapOverlay("Puntos BD");
        GMapOverlay routesOverlay = new GMapOverlay("Rutas");
        GMapOverlay searchOverlay = new GMapOverlay("busq");
        DataTable data;
        List<Punto> puntos = new List<Punto>();
        int filaSeleccionada = -1;
        Double latInicial;
        Double lngInicial;
        GoogleApi geolocationTests = new GoogleApi();
        bool trazarRuta = false;
        int ContadorIndicaresRuta = 0;
        PointLatLng inicio;
        PointLatLng Final;
        private Categorias cat;

        public Form1()
        {
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
            frmMatriz = new FrmMatrizAdy();
            frmPago = new FrmPago();
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

            //Ingresa la posición actual a la tabla
            data.Rows.Add("Ubicación Actual", latInicial, lngInicial);
            dataGridView1.DataSource = data;
        }

        private void actualizarDgv()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = puntos;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[3].Visible = false;
        }

        private void Ruta()
        {
            SqlConnection conexion = Conexion.ObtenerConexion();
            conexion.Open();
            SqlCommand cmd = conexion.CreateCommand();
            int ini = 0, fnl = 0;
            if (dropdownRutas.SelectedIndex != -1)
            {

                if (dropdownRutas.Text == "Ruta de las Flores")
                {
                    ini = 1;
                    fnl = 5;
                }
                else if (dropdownRutas.Text == "Ruta Volcánica")
                {
                    ini = 6;
                    fnl = 8;
                }
                else if (dropdownRutas.Text == "Ruta Religiosa")
                {
                    ini = 12;
                    fnl = 15;
                }
                else if (dropdownRutas.Text == "Ruta Azul")
                {
                    ini = 9;
                    fnl = 11;
                }
                else if (dropdownRutas.Text == "Ruta Arqueológica")
                {
                    ini = 16;
                    fnl = 22;
                }

                for (int i = ini; i <= fnl; i++)
                {

                    cmd.CommandText = "SELECT Latitud FROM dbo.Puntoos WHERE IDPunto = @Latitud" + i;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@Latitud" + i, i));
                    SqlDataReader reader = cmd.ExecuteReader();

                    double lt = 0, lng = 0;
                    string refe = "";

                    if (reader.Read())
                    {
                        lt = double.Parse(reader["Latitud"] as string);
                    }
                    reader.Close();

                    cmd.CommandText = "SELECT Longitud FROM dbo.Puntoos WHERE IDPunto = @Longitud" + i;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@Longitud" + i, i));
                    SqlDataReader reader1 = cmd.ExecuteReader();

                    if (reader1.Read())
                    {
                        lng = double.Parse(reader1["Longitud"] as string);
                    }
                    reader1.Close();

                    cmd.CommandText = "SELECT PuntoDeReferencia FROM dbo.Puntoos WHERE IDPunto = @Punto" + i;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@Punto" + i, i));
                    SqlDataReader reader2 = cmd.ExecuteReader();

                    if (reader2.Read())
                    {
                        refe = reader2["PuntoDeReferencia"] as string;
                    }
                    reader2.Close();

                    GMarkerGoogle marcador = new GMarkerGoogle(new PointLatLng(lt, lng), GMarkerGoogleType.lightblue);
                    markerOverlay.Markers.Add(marcador);

                    marcador.ToolTipMode = MarkerTooltipMode.Always;
                    marcador.ToolTipText = refe;

                    gMapControl1.Overlays.Add(markerOverlay);
                    gMapControl1.Zoom++;
                    gMapControl1.Zoom--;
                    Punto punto = new Punto(lt, lng, refe, GMarkerGoogleType.lightblue, ref markerOverlay, ref gMapControl1);

                    puntos.Add(punto);
                    AddPunto(punto);
                    //Actualiza la tabla de puntos
                    actualizarDgv();
                }
            }
            cmd.Dispose();
            conexion.Dispose();
        }

        private void AddPunto(Punto punto)
        {
            grafo.AgregarVertice(punto.nombre, punto.lat, punto.lng);
            foreach (Punto p in puntos)
            {
                var ruta = GoogleMapProvider.Instance.GetRoute(new PointLatLng(punto.lat, punto.lng), new PointLatLng(p.lat, p.lng), false, false, 14);
                if (ruta != null)
                {
                    grafo.AgregarArco(punto.nombre, p.nombre, ruta.Distance);
                    grafo.AgregarArco(p.nombre, punto.nombre, ruta.Distance);
                }
            }
        }

        private void Dibujar(PointLatLng punto, string nombre, Bitmap bmark)
        {
            GMapMarker mark = new GMarkerGoogle(punto, bmark);
            searchOverlay.Markers.Add(mark);
            mark.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            mark.ToolTipText = nombre;
            gMapControl1.Overlays.Add(searchOverlay);
        }

        private void RecorrerLugares(List<PointLatLng> puntos, List<string> nombres)
        {
            for (int i = 0; i < puntos.Count; i++)
            {
                Bitmap bmark;
                Size t = new Size(25, 25);
                switch (cat)
                {
                    case Categorias.Playa:
                        bmark = new Bitmap(Properties.Resources.markPlaya, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.Parque:
                        bmark = new Bitmap(Properties.Resources.markParque, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.Bosque:
                        bmark = new Bitmap(Properties.Resources.markBosque, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.Lago:
                        bmark = new Bitmap(Properties.Resources.markLago, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.Zoo:
                        bmark = new Bitmap(Properties.Resources.markZoologico, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.Mall:
                        bmark = new Bitmap(Properties.Resources.markCentroComercial, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.Restaurante:
                        bmark = new Bitmap(Properties.Resources.markRestaurante, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.Estadio:
                        bmark = new Bitmap(Properties.Resources.markEstadio, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.Museo:
                        bmark = new Bitmap(Properties.Resources.markMuseo, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.SitioArq:
                        bmark = new Bitmap(Properties.Resources.markSitioArqueologico, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.Iglesia:
                        bmark = new Bitmap(Properties.Resources.markIglesia, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                    case Categorias.Teatro:
                        bmark = new Bitmap(Properties.Resources.markTeatro, t);
                        Dibujar(puntos[i], nombres[i], bmark);
                        break;
                }
            }
        }

        private void HacerBusquedaCultura()
        {
            int radio = (int)(nudRadio.Value * 1000);
            string keyword;
            for (int i = 0; i < clbxCultura.CheckedIndices.Count; i++)
            {
                keyword = "";
                int indice = clbxCultura.CheckedIndices[i];
                switch (indice)
                {
                    case 0:
                        keyword = "Museo";
                        cat = Categorias.Museo;
                        break;
                    case 1:
                        keyword = "Sitio Arqueológico";
                        cat = Categorias.SitioArq;
                        break;
                    case 2:
                        keyword = "Iglesia";
                        cat = Categorias.Iglesia;
                        break;
                    case 3:
                        keyword = "Teatro";
                        cat = Categorias.Teatro;
                        break;
                    default:
                        break;
                }
                geolocationTests.PlacesTextSearchKeyword(keyword, latInicial, lngInicial, radio);
                RecorrerLugares(geolocationTests.lugares, geolocationTests.nombresLugares);
            }
        }

        private void HacerBusquedaNaturaleza()
        {
            int radio = (int)(nudRadio.Value * 1000);
            string keyword;
            for (int i = 0; i < clbxNaturaleza.CheckedIndices.Count; i++)
            {
                keyword = "";
                int indice = clbxNaturaleza.CheckedIndices[i];
                switch (indice)
                {
                    case 0:
                        keyword = "Playa";
                        cat = Categorias.Playa;
                        break;
                    case 1:
                        keyword = "Parque";
                        cat = Categorias.Parque;
                        break;
                    case 2:
                        keyword = "Bosque";
                        cat = Categorias.Bosque;
                        break;
                    case 3:
                        keyword = "Lago";
                        cat = Categorias.Lago;
                        break;
                    default:
                        break;
                }
                geolocationTests.PlacesTextSearchKeyword(keyword, latInicial, lngInicial, radio);
                RecorrerLugares(geolocationTests.lugares, geolocationTests.nombresLugares);
            }
        }

        private void HacerBusquedaEntretenimiento()
        {
            int radio = (int)(nudRadio.Value * 1000);
            string keyword;
            for (int i = 0; i < clbxEntretenimiento.CheckedIndices.Count; i++)
            {
                keyword = "";
                int indice = clbxEntretenimiento.CheckedIndices[i];
                switch (indice)
                {
                    case 0:
                        keyword = "Zoologico";
                        cat = Categorias.Zoo;
                        break;
                    case 1:
                        keyword = "Centro Comercial";
                        cat = Categorias.Mall;
                        geolocationTests.PlacesTextSearchKeyword("Mall", latInicial, lngInicial, radio);
                        RecorrerLugares(geolocationTests.lugares, geolocationTests.nombresLugares);
                        break;
                    case 2:
                        keyword = "Restaurante";
                        cat = Categorias.Restaurante;
                        break;
                    case 3:
                        keyword = "Estadio";
                        cat = Categorias.Estadio;
                        break;
                    default:
                        break;
                }
                geolocationTests.PlacesTextSearchKeyword(keyword, latInicial, lngInicial, radio);
                RecorrerLugares(geolocationTests.lugares, geolocationTests.nombresLugares);
            }
        }

        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Consigue la latitud y longitud según donde la persona clickeo en el mapa para luego crear un punto y dibujarlo en el mapa
            Double lat = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat;
            Double lng = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng;
            Punto punto = new Punto(lat, lng, $"Ubicación {puntos.Count}", GMarkerGoogleType.lightblue, ref markerOverlay, ref gMapControl1);
            puntos.Add(punto);
            AddPunto(punto);
            //Añade el punto a la lista y actualiza la tabla de puntos
            actualizarDgv();
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //Selecciona una fila
            filaSeleccionada = e.RowIndex;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> cultura = new List<string>();
            if (clbxCultura.CheckedItems.Count > 0)
            {
                HacerBusquedaCultura();
            }
            if (clbxNaturaleza.CheckedItems.Count > 0)
            {
                HacerBusquedaNaturaleza();
            }
            if (clbxEntretenimiento.CheckedItems.Count > 0)
            {
                HacerBusquedaEntretenimiento();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            gMapControl1.Overlays.Clear();
            gMapControl1.Overlays.Add(markerOverlay);
            gMapControl1.Overlays.Add(routesOverlay);
            gMapControl1.Overlays.Add(ourPoints);
            searchOverlay.Clear();
            clbxCultura.ClearSelected();
            clbxNaturaleza.ClearSelected();
            clbxEntretenimiento.ClearSelected();
        }

        private void btnEstimarPrecio_Click(object sender, EventArgs e)
        {
            Double precio = 0.55;
            Double costo;
            if (hayRuta)
            {
                costo = dist * precio;
                string txt = $"La ruta que pasa por: ";
                for (int i = 0; i < rutaMasCorta.Count(); i++)
                {
                    txt += $" la {rutaMasCorta.ElementAt(i).ToString()}";
                    if (i + 1 < rutaMasCorta.Count())
                    {
                        txt += $", ";
                    }
                }
                txt += $". Tiene una distancia de {dist} kilómetros y por lo tanto un costo de ${costo:N2}";
                frmPago.CambiarTexto(txt);
                frmPago.ShowDialog();
            }
            else
            {
                MessageBox.Show("No hay una ruta disponible para realizar una estimación");
            }
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
                gMapControl1.Overlays.Clear();
                gMapControl1.Overlays.Add(markerOverlay);
                gMapControl1.Overlays.Add(searchOverlay);
                routesOverlay.Clear();
                hayRuta = false;
            }
            else
                MessageBox.Show("No ha seleccionado ninguna ubicación");
        }

        private void btnHacerRuta_Click(object sender, EventArgs e)
        {
            if (grafo.nodos.Count > 1)
            {
                Double[,] matriz = grafo.crearMatriz();
                int totNodos = grafo.nodos.Count;
                Dijkstra ruta = new Dijkstra(totNodos, matriz, grafo.nodos);
                //frmMatriz.MostrarMatriz(matriz, totNodos, grafo.nodos);
                //frmMatriz.ShowDialog();
                rutaMasCorta = ruta.rutaMasCorta;
                dist = ruta.dmin;
                for (int i = 1; i < rutaMasCorta.Count(); i++)
                {
                    PointLatLng inicio = new PointLatLng();
                    PointLatLng final = new PointLatLng();
                    for (int j = 0; j < puntos.Count; j++)
                    {
                        if (puntos[j].nombre == rutaMasCorta.ElementAt(i - 1).ToString())
                        {
                            inicio = new PointLatLng(puntos[j].lat, puntos[j].lng);
                        }
                        if (puntos[j].nombre == rutaMasCorta.ElementAt(i).ToString())
                        {
                            final = new PointLatLng(puntos[j].lat, puntos[j].lng);
                        }
                    }
                    var pedazoRuta = GoogleMapProvider.Instance.GetRoute(inicio, final, false, false, 14);
                    var r = new GMapRoute(pedazoRuta.Points, "Ruta");
                    routesOverlay.Routes.Add(r);
                    gMapControl1.Overlays.Add(routesOverlay);

                }
                hayRuta = true;
                gMapControl1.Zoom++;
                gMapControl1.Zoom--;
            }
            else
            {
                MessageBox.Show("Necesita al menos dos puntos para hacer una ruta");
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        private void dropdownRutas_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ruta();
        }
    }
}
