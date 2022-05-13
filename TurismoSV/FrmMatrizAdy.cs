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
    public partial class FrmMatrizAdy : Form
    {
        public FrmMatrizAdy()
        {
            InitializeComponent();
        }

        private void FrmMatrizAdy_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        public void MostrarMatriz(Double[,] matriz, int tamanio, List<Vertice> nodos)
        {
            dgv.ColumnCount = tamanio;
            dgv.RowCount = tamanio;
            for (int i = 0; i < tamanio; i++)
            {
                dgv.Columns[i].HeaderText = nodos[i].ToString();
                for (int j = 0; j < tamanio; j++)
                {
                    dgv.Rows[j].HeaderCell.Value = nodos[j].ToString();
                    dgv.Rows[i].Cells[j].Value = matriz[i, j];
                }
            }
        }
    }
}
