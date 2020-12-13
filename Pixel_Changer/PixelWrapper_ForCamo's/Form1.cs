using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace PixelWrapper_ForCamo_s
{
    public partial class Form1 : Form
    {
        Image first;
        Image secondary;

        List<Color_RGB_HSV> colors;

        bool _1 = false;
        bool _2 = false;
        bool blocked = false;

        public Form1()
        {
            InitializeComponent();
            //button10.Enabled = false;
            //button11.Enabled = false;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Image|*.png|All files|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string file = openFileDialog.FileName;
                    first = Bitmap.FromFile(file);
                    pictureBox1.Image = first;
                    _2 = true;
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            if(_1 & _2 & !blocked)
            {
                blocked = true;
                new Thread(new ThreadStart(WorkForwarded)) { IsBackground = true }.Start();
            }
        }

        void WorkForwarded()
        {
            int Ox = first.Width;
            int Oy = first.Height;

            SetProgressBarMaximum(Ox * Oy);

            int pb = 0;

            Bitmap sec = new Bitmap(first);
            for (int y = 0; y < Oy; y++)
            {
                for (int x = 0; x < Ox; x++)
                {
                    var pixel = sec.GetPixel(x, y);
                    
                    Color selected = new Color();

                    Dictionary<double, Color> closest = new Dictionary<double, Color>();
                    foreach(var color in colors)
                    {
                        try
                        {
                            closest.Add(color.ColourDistance(pixel), color.ReturnColor());
                        }
                        catch
                        { }
                    }
                    var ordered = closest.OrderBy(a => a.Key).First();
                    selected = ordered.Value;

                    sec.SetPixel(x, y, selected);
                    ++pb;
                    SetProgressBar(pb);
                    secondary = sec;
                }
            }
            SetImage2(secondary);
            SetProgressBar(0);
            blocked = false;
        }
        
        void SetProgressBar(int value)
        {
            progressBar1.Invoke(new Action<int>((s) => progressBar1.Value = s), value);
        }

        void SetProgressBarMaximum(int max)
        {
            progressBar1.Invoke(new Action<int>((s) => progressBar1.Maximum = s), max);
        }

        void SetImage2(Image im)
        {
            pictureBox2.Invoke(new Action<Image>((s) => pictureBox2.Image = s), im);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog.Title = "Save an Image File";
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = saveFileDialog.FileName;
                pictureBox2.Image.Save(file);
            }
        }

        void SetB10(bool status)
        {
            button10.Invoke(new Action<bool>((s) => button10.Enabled = s), status);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "All files (*.*)|*.*|Txt|*.txt";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    colors = new List<Color_RGB_HSV>();
                    string file = openFileDialog.FileName;
                    string raw = File.ReadAllText(file);
                    string[] rgb = raw.Split('|');
                    for(int i = 0; i < rgb.Length; i++)
                    {
                        try
                        {
                            int R = int.Parse(rgb[i].Substring(0, 3));
                            int G = int.Parse(rgb[i].Substring(3, 3));
                            int B = int.Parse(rgb[i].Substring(6, 3));
                            colors.Add(new Color_RGB_HSV(R,G,B));
                        }
                        catch
                        {

                        }
                    }
                    _1 = true;
                    label2.Text = "Loaded RGB File";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
