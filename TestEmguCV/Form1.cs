using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.Util;
using DirectShowLib;

using Emgu.CV.UI;

namespace TestEmguCV
{
    public partial class Form1 : Form
    {
        private static CascadeClassifier classifierFace = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        //private static CascadeClassifier classifierFace = new CascadeClassifier("haarcascade_frontalface_default.xml"); 
        private static CascadeClassifier classifierEye = new CascadeClassifier("haarcascade_eye_tree_eyeglasses.xml");
        private static CascadeClassifier classifierSmile = new CascadeClassifier("haarcascade_smile.xml");

        private VideoCapture capture = null;

        private DsDevice[] webCams = null;

        private int selectedCameraId = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                DialogResult res = openFileDialog1.ShowDialog();

                if(res == DialogResult.OK)
                {
                    string path = openFileDialog1.FileName;

                    pictureBox1.Image = Image.FromFile(path);

                    Bitmap bitmap1 = new Bitmap(pictureBox1.Image);

                    Image<Bgr, byte> grayImage = new Image<Bgr, byte>(path);

                    Rectangle[] faces = classifierFace.DetectMultiScale(grayImage, 1.4, 0);

                    Rectangle[] eyes = classifierEye.DetectMultiScale(grayImage, 1.4, 0);

                    //Rectangle[] smiles = classifierSmile.DetectMultiScale(grayImage, 1.4, 0);

                    foreach (Rectangle face in faces )
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap1))
                        {
                            using (Pen pen = new Pen(Color.Yellow, 3))
                            {
                                graphics.DrawRectangle(pen, face);
                            }
                        }
                    }

                    foreach (Rectangle eye in eyes)
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap1))
                        {
                            using (Pen pen = new Pen(Color.Red, 3))
                            {
                                graphics.DrawRectangle(pen, eye);
                            }
                        }
                    }

                   /* foreach (Rectangle smile in smiles)
                    {
                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            using (Pen pen = new Pen(Color.Orange, 3))
                            {
                                graphics.DrawRectangle(pen, smile);
                            }
                        }
                    }*/




                    pictureBox1.Image = bitmap1;
                }
                else
                {
                    MessageBox.Show("Изображение не выбрано!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webCams = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            for (int i =0; i<webCams.Length; i++)
            {
                toolStripComboBox1.Items.Add(webCams[i].Name);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCameraId = toolStripComboBox1.SelectedIndex;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if(webCams.Length == 0)
                {
                    throw new Exception("Нет доступных камер");
                }
                else if (toolStripComboBox1.SelectedItem == null)
                {
                    throw new Exception("Нужно выбрать камеру");
                }
                else if (capture != null)
                {
                    capture.Start();
                }
                else
                {
                    capture = new VideoCapture(selectedCameraId);

                    capture.ImageGrabbed += Capture_ImageGrabbed;

                    capture.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                Mat m = new Mat();

                capture.Retrieve(m);

                Bitmap bitmap = new Bitmap(m.ToImage<Bgr, byte>().Flip(Emgu.CV.CvEnum.FlipType.Horizontal).AsBitmap());

                Image<Bgr, byte> grayImage = m.ToImage<Bgr, byte>().Convert<Bgr, byte>().Flip(Emgu.CV.CvEnum.FlipType.Horizontal); 

                Rectangle[] faces = classifierFace.DetectMultiScale(grayImage, 1.4, 0);

                Rectangle[] eyes = classifierEye.DetectMultiScale(grayImage, 1.4, 0);

                foreach (Rectangle face in faces)
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        using (Pen pen = new Pen(Color.Yellow, 3))
                        {
                            graphics.DrawRectangle(pen, face);
                        }
                    }
                }

                foreach (Rectangle eye in eyes)
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        using (Pen pen = new Pen(Color.Red, 3))
                        {
                            graphics.DrawRectangle(pen, eye);
                        }
                    }
                }

                pictureBox2.Image = bitmap;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if(capture != null)
                {
                    capture.Pause();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (capture != null)
                {
                    capture.Pause();

                    capture.Dispose();

                    capture = null;

                    pictureBox2.Image.Dispose();

                    pictureBox2.Image = null;

                    selectedCameraId = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                Mat m = new Mat();

                capture.Retrieve(m);

                MakeScreenShotForm makeScreenShotForm = new MakeScreenShotForm(m.ToImage<Bgr, byte>().Flip(Emgu.CV.CvEnum.FlipType.Horizontal));

                makeScreenShotForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
