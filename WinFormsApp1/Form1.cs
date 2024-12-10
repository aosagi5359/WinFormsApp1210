using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Bitmap edgeImage;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap loadedImage = new Bitmap(openFileDialog.FileName);
                pictureBox1.Image = loadedImage;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            Bitmap grayscaleImage = ConvertToGrayscale(new Bitmap(pictureBox1.Image));

            edgeImage = DetectEdges(grayscaleImage);
            pictureBox2.Image = edgeImage;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (edgeImage == null)
            {
                MessageBox.Show("Please perform edge detection first.");
                return;
            }

            Bitmap circlesImage = DetectCircles(edgeImage);
            pictureBox2.Image = circlesImage;
        }

        private Bitmap ConvertToGrayscale(Bitmap source)
        {
            Bitmap grayscale = new Bitmap(source.Width, source.Height);
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color original = source.GetPixel(x, y);
                    int gray = (int)(original.R * 0.3 + original.G * 0.59 + original.B * 0.11);
                    grayscale.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            return grayscale;
        }

        private Bitmap DetectEdges(Bitmap grayscale)
        {
            Bitmap edges = new Bitmap(grayscale.Width, grayscale.Height);
            int[] gx = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
            int[] gy = { -1, -2, -1, 0, 0, 0, 1, 2, 1 };

            for (int y = 1; y < grayscale.Height - 1; y++)
            {
                for (int x = 1; x < grayscale.Width - 1; x++)
                {
                    int sumX = 0, sumY = 0;

                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            int gray = grayscale.GetPixel(x + kx, y + ky).R;
                            int kernelIndex = (ky + 1) * 3 + (kx + 1);
                            sumX += gray * gx[kernelIndex];
                            sumY += gray * gy[kernelIndex];
                        }
                    }

                    int gradient = Math.Min(255, (int)Math.Sqrt(sumX * sumX + sumY * sumY));
                    edges.SetPixel(x, y, Color.FromArgb(gradient, gradient, gradient));
                }
            }
            return edges;
        }

        private Bitmap DetectCircles(Bitmap edgeImage)
        {
            Bitmap resultImage = new Bitmap(edgeImage);
            int width = edgeImage.Width, height = edgeImage.Height;
            int minRadius = 10, maxRadius = 100;
            int threshold = 50;

            int[,,] accumulator = new int[width, height, maxRadius];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (edgeImage.GetPixel(x, y).R > 128)
                    {
                        for (int radius = minRadius; radius < maxRadius; radius++)
                        {
                            for (int angle = 0; angle < 360; angle += 5)
                            {
                                int a = x - (int)(radius * Math.Cos(angle * Math.PI / 180));
                                int b = y - (int)(radius * Math.Sin(angle * Math.PI / 180));
                                if (a >= 0 && a < width && b >= 0 && b < height)
                                {
                                    accumulator[a, b, radius]++;
                                }
                            }
                        }
                    }
                }
            }

            using (Graphics g = Graphics.FromImage(resultImage))
            {
                for (int a = 0; a < width; a++)
                {
                    for (int b = 0; b < height; b++)
                    {
                        for (int r = minRadius; r < maxRadius; r++)
                        {
                            if (accumulator[a, b, r] > threshold)
                            {
                                g.DrawEllipse(Pens.Red, a - r, b - r, r * 2, r * 2);
                            }
                        }
                    }
                }
            }
            return resultImage;
        }
    }
}
