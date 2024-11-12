namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "�Ϲ����(JPeg. Gif. Bmp, etc|*.jpg;*.jpeg;*.gif;*.bmp;*.tif;*.tiff;*.png|�Ҧ����(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap MyBitmap = new Bitmap(openFileDialog.FileName);
                    this.pictureBox1.Image = MyBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "�T�����");
            }

            try
            {
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldBitmap = (Bitmap)this.pictureBox1.Image;
                Color pixel;
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        pixel = oldBitmap.GetPixel(x, y);
                        int r, g, b, Result = 0;
                        r = pixel.R;
                        g = pixel.G;
                        b = pixel.B;
                        Result = (299 * r + 587 * g + 114 * b) / 1000;
                        newBitmap.SetPixel(x, y, Color.FromArgb(Result, Result, Result));
                    }
                }
                this.pictureBox1.Image = newBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "�T�����");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int Height = this.pictureBox1.Image.Height;
                int Width = this.pictureBox1.Image.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                Bitmap oldbitmap = (Bitmap)this.pictureBox1.Image;
                int[] pixel_mask = new int[9]; //�v���B�n
                int pixGx, pixGy;
                double pixEdge;
                double maxEdge = 0;  // �ΨӫO�s�̤j����t�j�׭�

                // Prewitt��t�˴��B�n
                int[] Prewitt_Gx = new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
                int[] Prewitt_Gy = new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 };

                // �p��Ҧ���������t�j�סA�ç��̤j����t�j�׭�
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        // ���o9�Ӭ۾F�������Ƕ���
                        pixel_mask[0] = oldbitmap.GetPixel(x - 1, y - 1).G;
                        pixel_mask[1] = oldbitmap.GetPixel(x, y - 1).G;
                        pixel_mask[2] = oldbitmap.GetPixel(x + 1, y - 1).G;
                        pixel_mask[3] = oldbitmap.GetPixel(x - 1, y).G;
                        pixel_mask[4] = oldbitmap.GetPixel(x, y).G;
                        pixel_mask[5] = oldbitmap.GetPixel(x + 1, y).G;
                        pixel_mask[6] = oldbitmap.GetPixel(x - 1, y + 1).G;
                        pixel_mask[7] = oldbitmap.GetPixel(x, y + 1).G;
                        pixel_mask[8] = oldbitmap.GetPixel(x + 1, y + 1).G;

                        // �p��Gx�PGy (Prewitt��t�˴�)
                        pixGx = 0;
                        pixGy = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            pixGx += pixel_mask[i] * Prewitt_Gx[i];
                            pixGy += pixel_mask[i] * Prewitt_Gy[i];
                        }

                        // �p����t�j��
                        pixEdge = Math.Sqrt(pixGx * pixGx + pixGy * pixGy);

                        // ��s�̤j��t�j�׭�
                        if (pixEdge > maxEdge)
                        {
                            maxEdge = pixEdge;
                        }
                    }
                }

                // ���W����t�j�רåͦ��G�ȤƵ��G
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        // ���o9�Ӭ۾F�������Ƕ���
                        pixel_mask[0] = oldbitmap.GetPixel(x - 1, y - 1).G;
                        pixel_mask[1] = oldbitmap.GetPixel(x, y - 1).G;
                        pixel_mask[2] = oldbitmap.GetPixel(x + 1, y - 1).G;
                        pixel_mask[3] = oldbitmap.GetPixel(x - 1, y).G;
                        pixel_mask[4] = oldbitmap.GetPixel(x, y).G;
                        pixel_mask[5] = oldbitmap.GetPixel(x + 1, y).G;
                        pixel_mask[6] = oldbitmap.GetPixel(x - 1, y + 1).G;
                        pixel_mask[7] = oldbitmap.GetPixel(x, y + 1).G;
                        pixel_mask[8] = oldbitmap.GetPixel(x + 1, y + 1).G;

                        // �p��Gx�PGy (Prewitt��t�˴�)
                        pixGx = 0;
                        pixGy = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            pixGx += pixel_mask[i] * Prewitt_Gx[i];
                            pixGy += pixel_mask[i] * Prewitt_Gy[i];
                        }

                        // �p����t�j��
                        pixEdge = Math.Sqrt(pixGx * pixGx + pixGy * pixGy);

                        // ���W����t�j�ר� 0-255 �d��
                        int normalizedEdge = (int)((pixEdge / maxEdge) * 255);

                        // �i��G�ȤƳB�z�G�N�j��128���]��255�A�p��128���]��0
                        int edgeValue = (normalizedEdge >= 128) ? 255 : 0;

                        // �]�w�s����
                        newBitmap.SetPixel(x, y, Color.FromArgb(edgeValue, edgeValue, edgeValue));
                    }
                }

                // ��ܳB�z�᪺�v��
                this.pictureBox2.Image = newBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "�T�����");
            }
        }


    }
}