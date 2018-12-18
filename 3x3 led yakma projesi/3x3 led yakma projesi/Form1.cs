using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using Point = System.Drawing.Point;
using System.Windows.Forms;

namespace _3x3_led_yakma_projesi
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection VideoCapTureDevices;
        private VideoCaptureDevice Finalvideo;
        SerialPort ardino = new SerialPort();

        public Form1()
        {
            InitializeComponent();
        }
        int R;
        int G;
        int B;

        private void Form1_Load(object sender, EventArgs e)
        {

            VideoCapTureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCapTureDevices)
            {

                comboBox1.Items.Add(VideoCaptureDevice.Name);

            }

            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Finalvideo = new VideoCaptureDevice(VideoCapTureDevices[comboBox1.SelectedIndex].MonikerString);
            Finalvideo.NewFrame += new NewFrameEventHandler(Finalvideo_NewFrame);
            Finalvideo.DesiredFrameRate = 20;//saniyede kaç görüntü alsın istiyorsanız. FPS
            Finalvideo.DesiredFrameSize = new Size(360, 275);//görüntü boyutları
            Finalvideo.Start();
        }
        void Finalvideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;

            if (radioButton1.Checked)
            {

                // create filter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center colol and radius
                filter.CenterColor = new RGB(Color.FromArgb(215, 30, 30));
                filter.Radius = 100;
                // apply the filter
                filter.ApplyInPlace(image1);


                nesnebul(image1);

            }

            if (radioButton3.Checked)
            {

                // create filter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center color and radius
                filter.CenterColor = new RGB(Color.FromArgb(30, 144, 255));
                filter.Radius = 100;
                // apply the filter
                filter.ApplyInPlace(image1);

                nesnebul(image1);

            }
            if (radioButton2.Checked)
            {

                // create filter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set center color and radius
                filter.CenterColor = new RGB(Color.FromArgb(0, 215, 0));
                filter.Radius = 100;
                // apply the filter
                filter.ApplyInPlace(image1);

                nesnebul(image1);


            }
        }
        public void nesnebul(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));
            image.UnlockBits(objectsData);


            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            pictureBox2.Image = image;

            foreach (Rectangle recs in rects)
            {
                if (rects.Length > 0)
                {
                    Rectangle objectRect = rects[0];
                    //Graphics g = Graphics.FromImage(image);
                    Graphics g = pictureBox1.CreateGraphics();
                    using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                    {
                        g.DrawRectangle(pen, objectRect);
                    }
                    //Cizdirilen Dikdörtgenin Koordinatlari aliniyor.
                    int objectX = objectRect.X + (objectRect.Width / 2);
                    int objectY = objectRect.Y + (objectRect.Height / 2);
                    //  g.DrawString(objectX.ToString() + "X" + objectY.ToString(), new Font("Arial", 12), Brushes.Red, new System.Drawing.Point(250, 1));

                    g.Dispose();

                    if (objectX < 108 && objectY < 90)
                    {
                        ardino.Write("1");

                    }
                    else if ((objectX > 108 && objectX < 212) && (objectY < 90))
                    {
                        ardino.Write("2");
                    }
                    else if ((objectX > 212 && objectX < 360) && (objectY < 90))
                    {
                        ardino.Write("3");
                    }
                    else if ((objectX < 108) && (objectY > 90 && objectY < 185))
                    {
                        ardino.Write("4");
                    }
                    else if ((objectX > 108 && objectX < 212) && (objectY > 90 && objectY < 185))
                    {
                        ardino.Write("5");
                    }
                    else if ((objectX > 212 && objectX < 360) && (objectY > 90 && objectY < 185))
                    {
                        ardino.Write("6");
                    }
                    else if ((objectX < 108) && (objectY > 185 && objectY < 275))
                    {
                        ardino.Write("7");
                    }
                    else if ((objectX > 108 && objectX < 212) && (objectY > 185 && objectY < 275))
                    {
                        ardino.Write("8");
                    }
                    else if ((objectX > 212 && objectX < 360) && (objectY > 185 && objectY < 275))
                    {
                        ardino.Write("9");
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }

            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                String portName = comboBox2.Text;
                ardino.PortName = portName;
                ardino.BaudRate = 9600;
                ardino.Open();
                toolStripLabel1.Text = "bağlandı";
            }
            catch (Exception)
            {

                toolStripLabel1.Text = " Porta bağlanmadı ,uygun portu seçin";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                ardino.Close();
                toolStripLabel1.Text = "Port bağlantısı kesildi ";
            }
            catch (Exception)
            {

                toolStripLabel1.Text = "İlk önce bağlan sonra bağlantıyı kes";
            }
        }
    }
}
