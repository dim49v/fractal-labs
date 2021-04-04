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
    public partial class Form4 : Form
    {
        const int Delta = 4;
        const int IMG_SIZE = 1024;
        string filename;
        double[] xi;
        double[] yi;
        double[,] volume;
        double[,,] u;
        double[,,] b;
        int size = 8;
        int gridSize;
        public Form4()
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
            label1.Text = " Minkovski dimension = " + MinkovskiDimension().ToString();
        }

        private double MinkovskiDimension()
        {
            Mat mat = CvInvoke.Imread(filename);
            CvInvoke.Resize(mat, mat, new Size(IMG_SIZE, IMG_SIZE));
            Image<Gray, Byte> img1 = mat.ToImage<Gray, Byte>();
            gridSize = IMG_SIZE / size;
            xi = new double[Delta - 1];
            yi = new double[Delta - 1];
            volume = new double[gridSize * gridSize, Delta + 1];
            u = new double[gridSize * gridSize, (size + 2) * (size + 2), Delta + 1];
            b = new double[gridSize * gridSize, (size + 2) * (size + 2), Delta + 1];
            int pixel;
            double s;
            for (int cell = 0; cell < gridSize * gridSize; cell++)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        pixel = img1.Data[(cell / gridSize) * size + x, (cell % gridSize) * size + y, 0];
                        u[cell, (x + 1) * size + y + 1, 0] = pixel;
                        b[cell, (x + 1) * size + y + 1, 0] = pixel;
                    }
                }
                // рамка вокруг сегмента
                for (int x = 0; x < size + 2; x++)
                {
                    u[cell, x * size, 0] = 0;
                    u[cell, x * size + size + 2, 0] = 0;
                    u[cell, x, 0] = 0;
                    u[cell, (size + 2) * size + x, 0] = 0;
                    b[cell, x * size, 0] = 300;
                    b[cell, x * size + size + 2, 0] = 300;
                    b[cell, x, 0] = 300;
                    b[cell, (size + 2) * size + x, 0] = 300;
                }
            }
            for (int delta = 1; delta <= Delta; delta++)
            {
                setU(delta);
                setB(delta);
                for (int cell = 0; cell < gridSize * gridSize; cell++)
                {
                    s = 0;
                    for (int x = 1; x <= size; x++)
                    {
                        for (int y = 1; y <= size; y++)
                        {
                            s += u[cell, x * size + y, delta] - b[cell, x * size + y, delta];
                        }
                    }
                    volume[cell, delta] = s;
                    if (delta > 1)
                    {
                        yi[delta - 2] += (volume[cell, delta] - volume[cell, delta - 1]) / 2;
                    }
                }
                if (delta > 1)
                {
                    yi[delta - 2] = Math.Log(yi[delta - 2]);
                    xi[delta - 2] = -Math.Log(delta);
                }
            }
            return 2 - MNK();
        }
        private double MNK()
        {
            double sx = 0,
                sx2 = 0,
                sxy = 0,
                sy = 0;
            for (int i = 0; i < Delta - 1; i++)
            {
                sx += xi[i];
                sx2 += xi[i] * xi[i];
                sxy += xi[i] * yi[i];
                sy += yi[i];
            }
            sx /= (Delta - 1);
            sx2 /= (Delta - 1);
            sxy /= (Delta - 1);
            sy /= (Delta - 1);
            return (sxy - sx * sy) / (sx2 - sx * sx);
        }
        private void setB(int delta)
        {
            for (int cell = 0; cell < gridSize * gridSize; cell++)
            {
                for (int x = 1; x <= size; x++)
                {
                    for (int y = 1; y <= size; y++)
                    {
                        b[cell, x * size + y, delta] = Math.Min(
                            b[cell, x * size + y, delta - 1] - 1,
                            Math.Min(
                                Math.Min(
                                    b[cell, (x - 1) * size + y, delta - 1],
                                    b[cell, x * size + (y - 1), delta - 1]
                                ),
                                Math.Min(
                                    b[cell, (x + 1) * size + y, delta - 1],
                                    b[cell, x * size + (y + 1), delta - 1]
                                )
                            )
                        );
                    }
                }
            }
        }
        private void setU(int delta)
        {
            for (int cell = 0; cell < gridSize * gridSize; cell++)
            {
                for (int x = 1; x <= size; x++)
                {
                    for (int y = 1; y <= size; y++)
                    {
                        u[cell, x * size + y, delta] = Math.Max(
                            b[cell, x * size + y, delta - 1] + 1,
                            Math.Max(
                                Math.Max(
                                    u[cell, (x - 1) * size + y, delta - 1],
                                    u[cell, x * size + (y - 1), delta - 1]
                                ),
                                Math.Max(
                                    u[cell, (x + 1) * size + y, delta - 1],
                                    u[cell, x * size + (y + 1), delta - 1]
                                )
                            )
                        );
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            filename = textBox1.Text;
            loadImage();
        }
    }
}
