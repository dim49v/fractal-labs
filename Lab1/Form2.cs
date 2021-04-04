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
    public partial class Form2 : Form
    {
        string filename;
        Mat mat;
        public Form2()
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
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Image<Bgr, Byte> img1 = mat.ToImage<Bgr, Byte>();
            Image<Bgr, Byte> img2;
            switch (comboBox1.SelectedIndex)
            {
                
                case 1:
                    img2 = new Image<Bgr, Byte>(img1.Width, img1.Height, new Bgr(0, 0, 255));
                    break; 
                case 2:
                    img2 = new Image<Bgr, Byte>(img1.Width, img1.Height, new Bgr(0, 255, 0));
                    break;    
                case 3:
                    img2 = new Image<Bgr, Byte>(img1.Width, img1.Height, new Bgr(255, 0, 0));
                    break;
                case 0:
                default:
                    img2 = new Image<Bgr, Byte>(img1.Width, img1.Height, new Bgr(255, 255, 255));
                    break;
            }
            img1 = img1.And(img2);
            img2.Dispose();
            if (panel2.BackgroundImage != null)
            {
                panel2.BackgroundImage.Dispose();
            }
            img1.Save("edited.png");
            img1.Dispose();
            panel2.BackgroundImage = new Bitmap("edited.png");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            filename = textBox1.Text;
            loadImage();
        }
    }
}
