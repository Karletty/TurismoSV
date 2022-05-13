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
        public static byte[] toByteArray(Image img, ImageFormat format)
        {
            //Convierte Image al formato establecido a través de MemoryStream
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
}
