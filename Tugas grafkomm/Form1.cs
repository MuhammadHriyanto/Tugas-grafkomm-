using System.Drawing.Drawing2D;

namespace Tugas_grafkomm
{
    public partial class Form1 : Form
    {

        private Point[] originalTrianglePoints;
        private Point[] currentTrianglePoints;

        // ------------- Lingkaran (Bresenham) -----------------
        private Point originalCircleCenter;
        private int originalCircleRadius;
        private Point currentCircleCenter;
        private int currentCircleRadius;

        // ------------- Persegi (Bresenham) -----------------
        private Point[] originalSquarePoints;
        private Point[] currentSquarePoints;

        // Matriks Transformasi (opsional, tapi lebih baik untuk putar dan skala kompleks)
        private Matrix triangleMatrix;
        private Matrix circleMatrix; // Untuk pusat lingkaran
        private Matrix squareMatrix;
        public Form1()
        {
            InitializeComponent();
            InitializeGeometricObjects();
            InitializeMatrices();
            SetupPaintEvents();

        }

        private void InitializeGeometricObjects()
        {
            originalTrianglePoints = new Point[]
           {
                new Point(pictureBox1.Width / 2, 50),
                new Point(pictureBox1.Width / 2 - 80, 200),
                new Point(pictureBox1.Width / 2 + 80, 200)
           };
            currentTrianglePoints = (Point[])originalTrianglePoints.Clone();

            // Inisialisasi Lingkaran Awal
            originalCircleCenter = new Point(pictureBox2.Width / 2, pictureBox2.Height / 2);
            originalCircleRadius = 70;
            currentCircleCenter = originalCircleCenter;
            currentCircleRadius = originalCircleRadius;

            // Inisialisasi Persegi Awal (Koordinat relatif terhadap PictureBox)
            int squareSize = 100;
            originalSquarePoints = new Point[]
            {
                new Point(pictureBox3.Width / 2 - squareSize / 2, pictureBox3.Height / 2 - squareSize / 2), // Top-left
                new Point(pictureBox3.Width / 2 + squareSize / 2, pictureBox3.Height / 2 - squareSize / 2), // Top-right
                new Point(pictureBox3.Width / 2 + squareSize / 2, pictureBox3.Height / 2 + squareSize / 2), // Bottom-right
                new Point(pictureBox3.Width / 2 - squareSize / 2, pictureBox3.Height / 2 + squareSize / 2)  // Bottom-left
            };
            currentSquarePoints = (Point[])originalSquarePoints.Clone();
        }
        private void InitializeMatrices()
        {
            triangleMatrix = new Matrix();
            circleMatrix = new Matrix();
            squareMatrix = new Matrix();
        }

        private void SetupPaintEvents()
        {
            // Tambahkan event Paint untuk setiap PictureBox
            // Ini akan memastikan objek digambar ulang setiap kali PictureBox membutuhkan refresh
            pictureBox1.Paint += PictureBox_Paint_Segitiga;
            pictureBox2.Paint += PictureBox_Paint_Lingkaran;
            pictureBox3.Paint += PictureBox_Paint_Persegi;
        }
        private void PictureBox_Paint_Segitiga(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias; // Untuk garis yang lebih halus

            // Menggambar segitiga menggunakan algoritma DDA
            DrawLineDDA(g, currentTrianglePoints[0].X, currentTrianglePoints[0].Y, currentTrianglePoints[1].X, currentTrianglePoints[1].Y);
            DrawLineDDA(g, currentTrianglePoints[1].X, currentTrianglePoints[1].Y, currentTrianglePoints[2].X, currentTrianglePoints[2].Y);
            DrawLineDDA(g, currentTrianglePoints[2].X, currentTrianglePoints[2].Y, currentTrianglePoints[0].X, currentTrianglePoints[0].Y);
        }
        private void PictureBox_Paint_Lingkaran(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Menggambar lingkaran menggunakan algoritma Bresenham
            DrawCircleBresenham(g, currentCircleCenter.X, currentCircleCenter.Y, currentCircleRadius);
        }
        private void PictureBox_Paint_Persegi(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Menggambar persegi menggunakan algoritma Bresenham (menggambar 4 garis)
            DrawLineBresenham(g, currentSquarePoints[0].X, currentSquarePoints[0].Y, currentSquarePoints[1].X, currentSquarePoints[1].Y);
            DrawLineBresenham(g, currentSquarePoints[1].X, currentSquarePoints[1].Y, currentSquarePoints[2].X, currentSquarePoints[2].Y);
            DrawLineBresenham(g, currentSquarePoints[2].X, currentSquarePoints[2].Y, currentSquarePoints[3].X, currentSquarePoints[3].Y);
            DrawLineBresenham(g, currentSquarePoints[3].X, currentSquarePoints[3].Y, currentSquarePoints[0].X, currentSquarePoints[0].Y);
        }
        private void DrawLineDDA(Graphics g, int x0, int y0, int x1, int y1)
        {
            float dx = x1 - x0;
            float dy = y1 - y0;

            float steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            float xIncrement = dx / steps;
            float yIncrement = dy / steps;

            float x = x0;
            float y = y0;

            for (int i = 0; i <= steps; i++)
            {
                // Gambar piksel
                g.FillRectangle(Brushes.Black, (int)Math.Round(x), (int)Math.Round(y), 1, 1);
                x += xIncrement;
                y += yIncrement;
            }
        }

        private void DrawLineBresenham(Graphics g, int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = (x0 < x1) ? 1 : -1;
            int sy = (y0 < y1) ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                g.FillRectangle(Brushes.Black, x0, y0, 1, 1);

                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
        private void DrawCircleBresenham(Graphics g, int xc, int yc, int r)
        {
            int x = 0;
            int y = r;
            int p = 3 - 2 * r; // Initial decision parameter

            // Menggambar 8 titik simetris
            void drawPoints(int cx, int cy, int px, int py)
            {
                g.FillRectangle(Brushes.Black, cx + px, cy + py, 1, 1);
                g.FillRectangle(Brushes.Black, cx - px, cy + py, 1, 1);
                g.FillRectangle(Brushes.Black, cx + px, cy - py, 1, 1);
                g.FillRectangle(Brushes.Black, cx - px, cy - py, 1, 1);
                g.FillRectangle(Brushes.Black, cx + py, cy + px, 1, 1);
                g.FillRectangle(Brushes.Black, cx - py, cy + px, 1, 1);
                g.FillRectangle(Brushes.Black, cx + py, cy - px, 1, 1);
                g.FillRectangle(Brushes.Black, cx - py, cy - px, 1, 1);
            }

            drawPoints(xc, yc, x, y);

            while (y >= x)
            {
                x++;
                if (p > 0)
                {
                    y--;
                    p = p + 4 * (x - y) + 10;
                }
                else
                {
                    p = p + 4 * x + 6;
                }
                drawPoints(xc, yc, x, y);
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < currentTrianglePoints.Length; i++)
            {
                currentTrianglePoints[i].X -= 10; // Geser 10 piksel ke kiri
            }
            pictureBox1.Invalidate(); // Memicu event Paint
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < currentTrianglePoints.Length; i++)
            {
                currentTrianglePoints[i].X += 10;
            }
            pictureBox1.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < currentTrianglePoints.Length; i++)
            {
                currentTrianglePoints[i].Y -= 10;
            }
            pictureBox1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < currentTrianglePoints.Length; i++)
            {
                currentTrianglePoints[i].Y += 10;
            }
            pictureBox1.Invalidate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            float scaleFactor = 1.1f; // Skala 10% lebih besar
            ApplyTriangleTransformation(scaleFactor, scaleFactor, 0); // Scale (sx, sy, angle)
        }
        private void button6_Click(object sender, EventArgs e)
        {
            float scaleFactor = 0.9f; // Skala 10% lebih kecil
            ApplyTriangleTransformation(scaleFactor, scaleFactor, 0); // Scale (sx, sy, angle)
        }
        private void button7_Click(object sender, EventArgs e)
        {
            ApplyTriangleTransformation(1, 1, 5);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ApplyTriangleTransformation(1, 1, -5);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            currentTrianglePoints = (Point[])originalTrianglePoints.Clone();
            triangleMatrix.Reset(); // Reset matrix juga
            pictureBox1.Invalidate();
        }

        private void ApplyTriangleTransformation(float scaleX, float scaleY, float angle)
        {
            // Tentukan titik tengah objek untuk rotasi/skala yang benar
            float centerX = (currentTrianglePoints[0].X + currentTrianglePoints[1].X + currentTrianglePoints[2].X) / 3f;
            float centerY = (currentTrianglePoints[0].Y + currentTrianglePoints[1].Y + currentTrianglePoints[2].Y) / 3f;

            // Terapkan translasi ke titik asal, skala, putar, translasi kembali
            triangleMatrix.Translate(-centerX, -centerY, MatrixOrder.Append);
            triangleMatrix.Scale(scaleX, scaleY, MatrixOrder.Append);
            triangleMatrix.Rotate(angle, MatrixOrder.Append);
            triangleMatrix.Translate(centerX, centerY, MatrixOrder.Append);

            // Terapkan matriks ke setiap titik segitiga
            PointF[] tempPoints = new PointF[currentTrianglePoints.Length];
            for (int i = 0; i < currentTrianglePoints.Length; i++)
            {
                tempPoints[i] = new PointF(currentTrianglePoints[i].X, currentTrianglePoints[i].Y);
            }

            triangleMatrix.TransformPoints(tempPoints);

            for (int i = 0; i < currentTrianglePoints.Length; i++)
            {
                currentTrianglePoints[i] = new Point((int)Math.Round(tempPoints[i].X), (int)Math.Round(tempPoints[i].Y));
            }

            // Penting: Reset matrix setelah diterapkan jika ingin transformasi inkremental
            // Jika Anda ingin transformasi relatif terhadap objek saat ini, tidak perlu reset.
            // Untuk demonstrasi ini, kita akan terus menambahkan transformasi ke matrix yang sama.
            // triangleMatrix.Reset(); // Hapus ini jika Anda ingin transformasi terus menerus

            pictureBox1.Invalidate();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            currentCircleCenter.X -= 10;
            pictureBox2.Invalidate();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            currentCircleCenter.X += 10;
            pictureBox2.Invalidate();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            currentCircleCenter.Y -= 10;
            pictureBox2.Invalidate();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            currentCircleCenter.Y += 10;
            pictureBox2.Invalidate();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            currentCircleRadius = (int)(currentCircleRadius * 1.1);
            pictureBox2.Invalidate();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            currentCircleRadius = (int)(currentCircleRadius * 0.9);
            pictureBox2.Invalidate();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            currentCircleCenter = originalCircleCenter;
            currentCircleRadius = originalCircleRadius;
            pictureBox2.Invalidate();
        }

        private void button27_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < currentSquarePoints.Length; i++)
            {
                currentSquarePoints[i].X -= 10;
            }
            pictureBox3.Invalidate();
        }

        private void button26_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < currentSquarePoints.Length; i++)
            {
                currentSquarePoints[i].X += 10;
            }
            pictureBox3.Invalidate();
        }

        private void button25_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < currentSquarePoints.Length; i++)
            {
                currentSquarePoints[i].Y -= 10;
            }
            pictureBox3.Invalidate();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < currentSquarePoints.Length; i++)
            {
                currentSquarePoints[i].Y += 10;
            }
            pictureBox3.Invalidate();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            float scaleFactor = 1.1f; // Skala 10% lebih besar
            ApplySquareTransformation(scaleFactor, scaleFactor, 0); // Scale (sx, sy, angle)
        }

        private void button22_Click(object sender, EventArgs e)
        {
            float scaleFactor = 0.9f; // Skala 10% lebih besar
            ApplySquareTransformation(scaleFactor, scaleFactor, 0); // Scale (sx, sy, angle)
        }

        private void button21_Click(object sender, EventArgs e)
        {
            ApplySquareTransformation(1, 1, 5);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            ApplySquareTransformation(1, 1, -5);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            currentSquarePoints = (Point[])originalSquarePoints.Clone();
            squareMatrix.Reset();
            pictureBox3.Invalidate();
        }
        private void ApplySquareTransformation(float scaleX, float scaleY, float angle)
        {
            // Tentukan titik tengah objek
            float centerX = (currentSquarePoints[0].X + currentSquarePoints[2].X) / 2f;
            float centerY = (currentSquarePoints[0].Y + currentSquarePoints[2].Y) / 2f;

            squareMatrix.Translate(-centerX, -centerY, MatrixOrder.Append);
            squareMatrix.Scale(scaleX, scaleY, MatrixOrder.Append);
            squareMatrix.Rotate(angle, MatrixOrder.Append);
            squareMatrix.Translate(centerX, centerY, MatrixOrder.Append);

            PointF[] tempPoints = new PointF[currentSquarePoints.Length];
            for (int i = 0; i < currentSquarePoints.Length; i++)
            {
                tempPoints[i] = new PointF(currentSquarePoints[i].X, currentSquarePoints[i].Y);
            }

            squareMatrix.TransformPoints(tempPoints);

            for (int i = 0; i < currentSquarePoints.Length; i++)
            {
                currentSquarePoints[i] = new Point((int)Math.Round(tempPoints[i].X), (int)Math.Round(tempPoints[i].Y));
            }

            pictureBox3.Invalidate();
        }
    }
}
