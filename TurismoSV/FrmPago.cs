using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TurismoSV
{
    public partial class FrmPago : Form
    {
        public FrmPago()
        {
            InitializeComponent();
        }
        public void CambiarTexto(string txt)
        {
            textBox1.Text = txt;
        }
    }
}
