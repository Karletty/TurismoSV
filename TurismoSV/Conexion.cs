using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace TurismoSV
{
    public class Conexion
    {
        public static SqlConnection ObtenerConexion()
        {
            //Retorna el string de conexión a la base de datos
            //Conectar es la cadena de conexión almacenada en la configuracion de propiedades
            SqlConnection Conn = new SqlConnection(Properties.Settings.Default.Conectar);
            return Conn;
        }
        public static void IngresarDatos()
        {
            IngresarCategorias();
        }

        public static byte[] toByteArray(Image img, ImageFormat format)
        {
            //Convierte Image al formato establecido a través de MemoryStream
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, format);
                return ms.ToArray();
            }
        }

        private static void IngresarCategorias()
        {
            //Ingresa las categorías que se usarán para los filtros
            //Todas las imágenes están guardadas en los recursos de propiedades
            byte[] playa = toByteArray(Properties.Resources.markPlaya, ImageFormat.Png);
            byte[] parque = toByteArray(Properties.Resources.markParque, ImageFormat.Png);
            byte[] bosque = toByteArray(Properties.Resources.markBosque, ImageFormat.Png);
            byte[] lago = toByteArray(Properties.Resources.markLago, ImageFormat.Png);

            byte[] zoo = toByteArray(Properties.Resources.markZoologico, ImageFormat.Png);
            byte[] centroComercial = toByteArray(Properties.Resources.markCentroComercial, ImageFormat.Png);
            byte[] restaurante = toByteArray(Properties.Resources.markRestaurante, ImageFormat.Png);
            byte[] estadio = toByteArray(Properties.Resources.markEstadio, ImageFormat.Png);

            byte[] museo = toByteArray(Properties.Resources.markMuseo, ImageFormat.Png);
            byte[] sitioArqueologico = toByteArray(Properties.Resources.markSitioArqueologico, ImageFormat.Png);
            byte[] iglesia = toByteArray(Properties.Resources.markIglesia, ImageFormat.Png);
            byte[] teatro = toByteArray(Properties.Resources.markTeatro, ImageFormat.Png);

            SqlConnection conexion = Conexion.ObtenerConexion();
            conexion.Open();
            try
            {
                string insertar = "INSERT INTO dbo.Categoria (IdCategoria, Nombre, Imagen) VALUES " +
                $"(1, 'Playa', @playa)," +
                $"(2, 'Parque', @parque), " +
                $"(3, 'Bosque', @bosque), " +
                $"(4, 'Lago', @lago), " +
                $"(5, 'Zoologico', @zoologico), " +
                $"(6, 'CentroComercial', @centroComercial), " +
                $"(7, 'Restaurante', @restaurante), " +
                $"(8, 'Estadio', @estadio), " +
                $"(9, 'Museo', @museo), " +
                $"(10, 'SitioArqueologico', @sitioArqueologico), " +
                $"(11, 'Iglesia', @iglesia), " +
                $"(12, 'Teatro', @teatro)";
                SqlCommand comando = new SqlCommand(insertar, conexion);

                comando.Parameters.Add(new SqlParameter("@playa", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@parque", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@bosque", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@lago", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@zoologico", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@centroComercial", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@restaurante", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@estadio", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@museo", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@sitioArqueologico", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@iglesia", SqlDbType.Image));
                comando.Parameters.Add(new SqlParameter("@teatro", SqlDbType.Image));

                comando.Parameters["@playa"].Value = playa;
                comando.Parameters["@parque"].Value = parque;
                comando.Parameters["@bosque"].Value = bosque;
                comando.Parameters["@lago"].Value = lago;
                comando.Parameters["@zoologico"].Value = zoo;
                comando.Parameters["@centroComercial"].Value = centroComercial;
                comando.Parameters["@restaurante"].Value = restaurante;
                comando.Parameters["@estadio"].Value = estadio;
                comando.Parameters["@museo"].Value = museo;
                comando.Parameters["@sitioArqueologico"].Value = sitioArqueologico;
                comando.Parameters["@iglesia"].Value = iglesia;
                comando.Parameters["@teatro"].Value = teatro;
                comando.ExecuteNonQuery();
                conexion.Close();
            }
            catch (Exception)
            {
                throw;
            }
            conexion.Close();
        }
    }
}
