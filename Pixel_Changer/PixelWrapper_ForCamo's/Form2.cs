using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelWrapper_ForCamo_s
{
    public partial class Form2 : Form
    {
        public float hue, saturation, brightness;
        public int id = 1;

        public Form2()
        {
            InitializeComponent();
            label4.Text = hue.ToString();
            label5.Text = saturation.ToString();
            label6.Text = brightness.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
