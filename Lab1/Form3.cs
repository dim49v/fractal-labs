using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form3 : Form
    {
        string filename;
        Mat mat;
        public Form3()
        {
            InitializeComponent();
        }
        private void loadImage()
        {
            Bitmap bmp;
            try
            {
                bmp = new Bitmap(filename);
            }
            catch (Exception exception)
            {
                label1.Text = "Load error";
                return;
            }
            if (panel1.BackgroundImage != null)
            {
                panel1.BackgroundImage.Dispose();
            }
            panel1.BackgroundImage = bmp;
            mat = CvInvoke.Imread(filename);
            Image<Gray, Byte> img1 = mat.ToImage<Gray, Byte>();
            if (panel2.BackgroundImage != null)
            {
                panel2.BackgroundImage.Dispose();
            }
            panel2.BackgroundImage = bmp;
            img1.Save("edited.png");
            img1.Dispose();
            panel2.BackgroundImage = new Bitmap("edited.png");
        }
        private void Form3_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            filename = textBox1.Text;
            loadImage();
        }
    }
}
