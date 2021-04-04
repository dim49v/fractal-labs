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
    public partial class Form5 : Form
    {
        const int Delta = 4;
        const int IMG_SIZE = 1024;
        string filename;
        double[,] volume;
        double[] area;
        double[,,] u;
        double[,,] b;
        int size = 8;
        int gridSize;
        public Form5()
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
            Segmentation();
        }

        private void Segmentation()
        {
            Mat mat = CvInvoke.Imread(filename);
            Size trueImgSize = mat.Size; 
            CvInvoke.Resize(mat, mat, new Size(IMG_SIZE, IMG_SIZE));
            Image<Gray, Byte> img1 = mat.ToImage<Gray, Byte>();
            gridSize = IMG_SIZE / size;
            volume = new double[gridSize * gridSize, Delta + 1];
            area = new double[gridSize * gridSize];
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
                }
            }
            s = 0;
            for (int cell = 0; cell < gridSize * gridSize; cell++)
            {
                area[cell] = (volume[cell, Delta] - volume[cell, Delta - 1]) / 2;
                s += area[cell];
            }
            s /= gridSize * gridSize;
            int intense;
            for (int cell = 0; cell < gridSize * gridSize; cell++)
            {
                if (area[cell] < s)
                {
                    intense = 0;
                }
                else
                {
                    intense = 255;
                }
                img1.Draw(new Rectangle((cell % gridSize) * size, (cell / gridSize) * size, size, size), new Gray(intense), -1);
            }
            CvInvoke.Resize(img1, img1, trueImgSize);  
            if (panel2.BackgroundImage != null)
            {
                panel2.BackgroundImage.Dispose();
            }
            img1.Save("edited.png");
            Bitmap bmp = new Bitmap("edited.png");
            panel2.BackgroundImage = bmp;
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
