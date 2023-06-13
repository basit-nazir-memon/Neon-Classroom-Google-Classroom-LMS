using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neon_Classroom
{
    public partial class PDF : Form
    {
        public PDF(string FileName)
        {
            InitializeComponent();
            string Destination = "E:\\Files Upload\\";
            var kl = FileName.Split('\\');
            string fname = kl[kl.Length - 1];
            axAcroPDF1.LoadFile(Destination + fname);
        }
    }
}
