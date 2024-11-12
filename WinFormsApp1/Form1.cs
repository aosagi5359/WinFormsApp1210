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
                openFileDialog.Filter = "圖像文件(JPeg. Gif. Bmp, etc|*.jpg;*.jpeg;*.gif;*.bmp;*.tif;*.tiff;*.png|所有文件(*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap MyBitmap = new Bitmap(openFileDialog.FileName);
                    this.pictureBox1.Image = MyBitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息顯示");
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
                MessageBox.Show(ex.Message, "訊息顯示");
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
                int[] pixel_mask = new int[9]; //影像遮罩
                int pixGx, pixGy;
                double pixEdge;
                double maxEdge = 0;  // 用來保存最大的邊緣強度值

                // Prewitt邊緣檢測遮罩
                int[] Prewitt_Gx = new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
                int[] Prewitt_Gy = new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 };

                // 計算所有像素的邊緣強度，並找到最大的邊緣強度值
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        // 取得9個相鄰像素的灰階值
                        pixel_mask[0] = oldbitmap.GetPixel(x - 1, y - 1).G;
                        pixel_mask[1] = oldbitmap.GetPixel(x, y - 1).G;
                        pixel_mask[2] = oldbitmap.GetPixel(x + 1, y - 1).G;
                        pixel_mask[3] = oldbitmap.GetPixel(x - 1, y).G;
                        pixel_mask[4] = oldbitmap.GetPixel(x, y).G;
                        pixel_mask[5] = oldbitmap.GetPixel(x + 1, y).G;
                        pixel_mask[6] = oldbitmap.GetPixel(x - 1, y + 1).G;
                        pixel_mask[7] = oldbitmap.GetPixel(x, y + 1).G;
                        pixel_mask[8] = oldbitmap.GetPixel(x + 1, y + 1).G;

                        // 計算Gx與Gy (Prewitt邊緣檢測)
                        pixGx = 0;
                        pixGy = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            pixGx += pixel_mask[i] * Prewitt_Gx[i];
                            pixGy += pixel_mask[i] * Prewitt_Gy[i];
                        }

                        // 計算邊緣強度
                        pixEdge = Math.Sqrt(pixGx * pixGx + pixGy * pixGy);

                        // 更新最大邊緣強度值
                        if (pixEdge > maxEdge)
                        {
                            maxEdge = pixEdge;
                        }
                    }
                }

                // 正規化邊緣強度並生成二值化結果
                for (int x = 1; x < Width - 1; x++)
                {
                    for (int y = 1; y < Height - 1; y++)
                    {
                        // 取得9個相鄰像素的灰階值
                        pixel_mask[0] = oldbitmap.GetPixel(x - 1, y - 1).G;
                        pixel_mask[1] = oldbitmap.GetPixel(x, y - 1).G;
                        pixel_mask[2] = oldbitmap.GetPixel(x + 1, y - 1).G;
                        pixel_mask[3] = oldbitmap.GetPixel(x - 1, y).G;
                        pixel_mask[4] = oldbitmap.GetPixel(x, y).G;
                        pixel_mask[5] = oldbitmap.GetPixel(x + 1, y).G;
                        pixel_mask[6] = oldbitmap.GetPixel(x - 1, y + 1).G;
                        pixel_mask[7] = oldbitmap.GetPixel(x, y + 1).G;
                        pixel_mask[8] = oldbitmap.GetPixel(x + 1, y + 1).G;

                        // 計算Gx與Gy (Prewitt邊緣檢測)
                        pixGx = 0;
                        pixGy = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            pixGx += pixel_mask[i] * Prewitt_Gx[i];
                            pixGy += pixel_mask[i] * Prewitt_Gy[i];
                        }

                        // 計算邊緣強度
                        pixEdge = Math.Sqrt(pixGx * pixGx + pixGy * pixGy);

                        // 正規化邊緣強度到 0-255 範圍
                        int normalizedEdge = (int)((pixEdge / maxEdge) * 255);

                        // 進行二值化處理：將大於128的設為255，小於128的設為0
                        int edgeValue = (normalizedEdge >= 128) ? 255 : 0;

                        // 設定新像素
                        newBitmap.SetPixel(x, y, Color.FromArgb(edgeValue, edgeValue, edgeValue));
                    }
                }

                // 顯示處理後的影像
                this.pictureBox2.Image = newBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息顯示");
            }
        }


    }
}