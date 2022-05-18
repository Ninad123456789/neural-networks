using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;
using mate;
namespace facerecog
{
 
    public partial class Form1 : Form
    {
        public static Matrix sigmoid(Matrix X)
        {
            int row = X.getRowCount();
            int col = X.getColCount();
            //Matrix t = new Matrix(row,col);
            double[,] data = new double[row, col];
            double[,] d = X.getData();
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    data[a, b] = (1 / (1 + Math.Pow(2.718281828459045, -d[a, b])));
                }
            }
            Matrix t = new Matrix(row, col, data);
            return t;
        }
        private static int  data_size =400,  number_of_layers = 3;
        private static Matrix theta1, theta2, theta3;
        public static double[,] predict(Matrix theta1, Matrix theta2, Matrix theta3, Matrix X)
        {
            double[,] pred;

            Matrix a, hyp, b;
           // X = X.addBiascol();

            a = sigmoid((X * theta1.T()));


            a = a.addBiascol();

            b = sigmoid(a * theta2.T());

            b = b.addBiascol();
            hyp = sigmoid(b * theta3.T());
            pred = hyp.getData();

            return pred;
        }
        public static Matrix activation(Matrix X)
        {
            int row = X.getRowCount();
            int col = X.getColCount();
            //Matrix t = new Matrix(row,col);
            double[,] data = new double[row, col];
            double[,] d = X.getData();
            for (int a = 0; a < row; a++)
            {
                for (int b = 0; b < col; b++)
                {
                    if (d[a, b] > 0)
                    {
                        data[a, b] = d[a, b];
                    }
                    else
                    {
                        data[a, b] = 0.001;
                    }
                    //data[a, b] = (1 / (1 + Math.Pow(2.718281828459045, -d[a, b])));
                }
            }
            Matrix t = new Matrix(row, col, data);
            return t;
        }
        public static Matrix[] getThetas()
        {
            //fixed sized thetas
            Matrix theta1, theta2, theta3;
            double[,] data1 = new double[25, data_size + 1];
            double[,] data2 = new double[10, 26];
            double[,] data3 = new double[2, 11];
            Matrix[] ret = new Matrix[number_of_layers];
            ////////*****************
            using (TextReader reader = File.OpenText("C:\\games\\source\\theta1.txt"))
            {

                string text;
                string[] bits;
                int count = 0;
                while ((text = reader.ReadLine()) != null)
                {
                    bits = text.Split(' ');

                    for (int a = 1; a <= data_size + 1; a++)
                    {
                        data1[count, a-1] = double.Parse(bits[a]);
                    }

                    count++;
                }

            }
            ////////****************
            using (TextReader reader = File.OpenText("C:\\games\\source\\theta2.txt"))
            {

                string text;
                string[] bits;
                int count = 0;
                while ((text = reader.ReadLine()) != null)
                {
                    bits = text.Split(' ');//a<400
                    if (bits.ToString().Equals(""))
                    {
                        break;
                    }
                    for (int a = 1; a <= 26; a++)
                    {
                        data2[count, a-1] = double.Parse(bits[a]);
                    }


                    count++;

                }

            }
            ////////****************
            using (TextReader reader = File.OpenText("C:\\games\\source\\theta3.txt"))
            {

                string text;
                string[] bits;
                int count = 0;
                while ((text = reader.ReadLine()) != null)
                {
                    bits = text.Split(' ');//a<400
                    if (bits.ToString().Equals(""))
                    {
                        break;
                    }
                    for (int a = 1; a <= 11; a++)
                    {
                        data3[count, a-1] = double.Parse(bits[a]);
                    }


                    count++;

                }

            }
            theta1 = new Matrix(25, data_size + 1, data1);
            theta2 = new Matrix(10, 26, data2);
            theta3 = new Matrix(2, 11, data3);
            ret[0] = theta1;
            ret[1] = theta2;
            ret[2] = theta3;
            return ret;
        }
        public static double[,] examin(Matrix Theta1, Matrix Theta2, Matrix Theta3, Bitmap bitmap)
        {
            //This is your bitmap
            Image<Bgr, byte> imageCV = new Image<Bgr, byte>(bitmap);
            Size s = new Size(20, 20);

            //StreamWriter filew = new StreamWriter("C:\\games\\source\\exam.txt");
            // string file = "C:\\games\\source\\trial1" + ".jfif";

            // doomer = CvInvoke.Imread(file, Emgu.CV.CvEnum.LoadImageType.AnyColor);
            //Image Class from Emgu.CV
            Mat doomer = imageCV.Mat;

            doomer.GetData();

            //  bool istrue = doomer.IsEmpty();

            CvInvoke.CvtColor(doomer, doomer, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray, 0);
            CvInvoke.Resize(doomer, doomer, s);


            Array name = doomer.GetData();
            double[,] X = new double[1, data_size];
          
            for (int a = 0; a<20; a++)
            {
                for(int b = 0;b<20;b++)
                {
                   
                   X[0,a*20+b] = double.Parse(name.GetValue(a,b).ToString());
                }
            }
            Matrix x = new Matrix(1, data_size, X);
            x = x.addBiascol();
            double[,] p = predict(Theta1, Theta2, Theta3, x);
            

            return p;

        }
        private  void Predict_face(Matrix theta1,Matrix theta2,Matrix theta3,Bitmap bitmap)
        {


            Bitmap bit = new Bitmap(500,pictureBox1.Height);

            for (int a = 0; a < pictureBox1.Width - 500; a += 50)
            {

                for (int b = 0; b < bit.Width; b++)
                {
                    for (int c = 0; c < bit.Height; c++)
                    {
                        Color color = bitmap.GetPixel(b + a, c);
                        bit.SetPixel(b, c, color);
                    }
                }
                double[,] pred = examin(theta1, theta2, theta3, bit);
                if(pred[0,0]>1)
                {
                     pred = examin(theta1, theta2, theta3, bit);
                }
                if (pred[0, 0] > 0.999 && pred[0, 1] < 0.01)
                {
                    face.Text = "True face  " + pred[0, 0] + "\nfalse face  " + pred[0, 1];
                    for (int b = 0; b < bit.Width; b++)
                    {
                        bitmap.SetPixel(a + b, 0, Color.Red);
                        bitmap.SetPixel(a + b, bitmap.Height - 1, Color.Red);
                    }
                    for (int b = 0; b < bit.Height; b++)
                    {
                        bitmap.SetPixel(a, b, Color.Red);
                        bitmap.SetPixel(a + bit.Width - 1, b, Color.Red);
                    }
                    pictureBox2.Image = bitmap;
                    return;
                }
                
            }
            
            



        }
        public Form1()
        {
            InitializeComponent();
        }

        FilterInfoCollection filter;

        private void button3_Click(object sender, EventArgs e)
        {
         //   Bitmap bitmap = (Bitmap)pictureBox1.Image;
         //   double[,] p = examin(theta1, theta2, theta3, bitmap);
           // face.Text = "True face  "+p[0,0] + "\nfalse face  "+p[0,1];
        //    Predict_face(theta1, theta2, theta3, bitmap);
        }

        VideoCaptureDevice device;
      
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        private void button1_Click(object sender, EventArgs e)
        {
            //pictureBox1.Image = (Bitmap)e.Frame.Clone();
            device = new VideoCaptureDevice(filter[cboDevice.SelectedIndex].MonikerString);
             
             device.NewFrame += VideoCaptureDevice_NewFrame;
            
            device.Start();
        }
        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }
        static int bitmap_width = 300, bitmap_height = 300;
        private Bitmap start(int X,int Y)
        {
          
        //  Bitmap bitm = new Bitmap(pictureBox1.Width,pictureBox1.Width);
          //  bitm = bitmap;
            // Bitmap bit = new Bitmap(200, 200);
            Bitmap bit = new Bitmap(bitmap_width, bitmap_width);
            int left = (X - bitmap_width/2), right = X + bitmap_width/2, top = Y - bitmap_width/2, bottom = Y + bitmap_width/2;
            int height = bitmap.Height;
            int width = bitmap.Width;
            int m = pictureBox1.Left;

            if ((X - bitmap_width/2) < 0)
            {
                left = 0;
                right += (bitmap_width / 2 - (X - width));
            }


            if (X + bitmap_width/2 > width)
            {
                right = width;
                left -= (bitmap_width / 2 - (width - X));
            }


            if (Y - bitmap_height/2 < 0)
            {
                top = 0;
                bottom += bitmap_height/2 - Y;
            }


            if (Y + bitmap_width/2 > height)
            {
                bottom = height;
                top -= (bitmap_height/2 - (height - Y));
            }
       
            //face.Text ="top="+ e.Y +" height=" + pictureBox1.Height + " bottom=" + pictureBox1.Bottom + " picturetop=" + pictureBox1.Top;
            for (int b = left; b < right; b++)
            {
                for (int c = top; c < bottom; c++)
                {
                    Color color = bitmap.GetPixel(b, c);
                    bit.SetPixel(b - left, c - top, color);

                }
            }
          
            double[,] p = examin(theta1, theta2, theta3, bit);
            face.Invoke((MethodInvoker)delegate

            {
              
                    face.Text = p[0, 0] + "  " + p[0, 1];
                
               

            });

            pictureBox2.Invoke((MethodInvoker)delegate

            {

                pictureBox2.Image = bit;

            });
            if (p[0,0] > p[0,1] && p[0,0] >0.99)
            {
               textBox1.Invoke((MethodInvoker)delegate

                {

                    textBox1.Text = p[0,0] + "    " + p[0,1];

                });
                return bit;
            }
            return null;
        //    face.Text = p[0, 0] + "  " + p[0, 1];
        }
        public static Bitmap bitmap;
        int captured = 0;
        private void scanpicture()
        {
            for (int a = bitmap_width/2; a < pictureBox1.Width; a += bitmap_width/2)
            {
                for (int b = bitmap_height/2; b < pictureBox1.Height; b += bitmap_height/4)
                {
                  Bitmap bit  =  start(a, b);

                    if(bit!=null )
                    {
                        pictureBox3.Invoke((MethodInvoker)delegate

                        {
                            captured = 1;
                            pictureBox3.Image = bit;
                            return;
                        });
                        
                    }
                }
            }
            
        }
        public Thread childThread=null;

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            captured = 0;
           // bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bitmap = (Bitmap)pictureBox1.Image;
            ThreadStart childref = new ThreadStart(scanpicture);
            if(childThread!=null)
            {
                childThread.Abort();
            }
             childThread = new Thread(childref);
            
            childThread.SetApartmentState(ApartmentState.STA);
             childThread.Start();
            while(childThread.IsAlive)
            {
                Application.DoEvents();
            }
         
        }

       

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(device.IsRunning)
            {
                device.Stop();
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            Matrix []ret = getThetas();
           
            theta1 = ret[0];
            theta2 = ret[1];
            theta3 = ret[2];

            foreach (FilterInfo device in filter)
                cboDevice.Items.Add(device.Name);
            cboDevice.SelectedIndex = 0;
            device = new VideoCaptureDevice();
        }
    }   
   
}
