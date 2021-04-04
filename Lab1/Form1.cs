using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        const int N = 5;
        const int IMG_SIZE = 1024;
        string filename;
        double[] xi = new double[N];
        double[] yi = new double[N];
        int[] size = new int[N] { 2, 8, 16, 32, 64 };
        public Form1()
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
            label1.Text = bmp.Size.ToString();
            if (panel1.BackgroundImage != null)
            {
                panel1.BackgroundImage.Dispose();
            }
            label1.Text += " Capacitive dimension = " + capacitiveDimension().ToString();
            bmp = new Bitmap("edited.png");               
            panel1.BackgroundImage = bmp;
        }

        private double capacitiveDimension()
        {  
            Mat mat = CvInvoke.Imread(filename);                  
            CvInvoke.Resize(mat, mat, new Size(IMG_SIZE, IMG_SIZE));
            Image<Gray, Byte> img1 = mat.ToImage<Gray, Byte>();
            for (int i = img1.Rows - 1; i >= 0; i--)
            {
                for (int j = img1.Cols - 1; j >= 0; j--)
                {
                    if (img1.Data[i, j, 0] > byte.MaxValue / 2)
                    {
                        img1.Data[i, j, 0] = byte.MaxValue;
                    }
                    else
                    {
                        img1.Data[i, j, 0] = 0;
                    }

                }
            }
            bool next;
            int ne;
            for (int i = 0; i < N; i++)
            {
                int gridSize = IMG_SIZE / size[i];
                ne = 0;
                for (int u1 = 0; u1 < gridSize; u1++)
                {
                    for (int u2 = 0; u2 < gridSize; u2++)
                    {
                        next = false;
                        for (int x = 0; x < size[i] && !next; x++)
                        {
                            for(int y = 0; y < size[i] && !next; y++)
                            {
                                if (img1.Data[u1 * size[i] + x, u2 * size[i] + y, 0] == 0)
                                {
                                    next = true;
                                    ne++;
                                }
                            }
                        }
                    }
                }
                xi[i] = Math.Log(1.0/size[i]);
                yi[i] = Math.Log(ne);
            }
            img1.Save("edited.png");
            return MNK();
        }
        private double MNK()
        {
            double sx = 0,
                sx2 = 0,
                sxy = 0,
                sy = 0;
            for (int i = 0; i < N; i++)
            {
                sx += xi[i];
                sx2 += xi[i] * xi[i];
                sxy += xi[i] * yi[i];
                sy += yi[i];
            }
            sx /= N;
            sx2 /= N;
            sxy /= N;
            sy /= N;
            return (sxy - sx * sy) / (sx2 - sx * sx);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            filename = textBox1.Text;
            loadImage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {        
            Form6 form6 = new Form6();
            form6.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form7 form7 = new Form7();
            form7.Show();
        }
    }
}
