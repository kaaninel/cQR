using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Colored_Quick_Response
{
    public partial class Form1 : Form
    {
        Util cQRUtil;
        FilterInfoCollection WebcamColl;
        VideoCaptureDevice Device;
        Stopwatch stopwatch;

        public Form1()
        {
            InitializeComponent();
            cQRUtil = new Util(); 
            stopwatch = new Stopwatch();
            tabControl1.Selected += new TabControlEventHandler(tabControl1_Selected);
            WebcamColl = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            Device = new VideoCaptureDevice(WebcamColl[0].MonikerString);
            Device.NewFrame += Device_NewFrame;
        }

        private void Device_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            stopwatch.Restart();
            Bitmap cQR = (Bitmap)eventArgs.Frame.Clone();
            pictureBox12.Image = (Bitmap)cQR.Clone();
            var QRs = cQRUtil.c2QR((Bitmap)cQR.Clone());
            pictureBox9.Image = QRs[0];
            pictureBox10.Image = QRs[1];
            pictureBox11.Image = QRs[2];
            string s = cQRUtil.ReadcQR((Bitmap)cQR.Clone());
            if (!string.IsNullOrEmpty(s)) SetText(this.textBox4,s);
            stopwatch.Stop();
        }

        delegate void SetTextCallback(TextBox tb, string text);
        private void SetText(TextBox tb,string text)
        {
            if (tb.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
                tb.Text = text;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == tabPage3.Name)
                Device.Start();
            else
                Device.Stop();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text)) {
                stopwatch.Restart();
                if (checkBox1.Checked) {
                    var cQRs = cQRUtil.CreateAll(textBox1.Text, 239, 239);
                    pictureBox4.Image = cQRs[0];
                    pictureBox3.Image = cQRs[1];
                    pictureBox2.Image = cQRs[2];
                    pictureBox1.Image = cQRs[3];
                }
                else
                    pictureBox1.Image = cQRUtil.CreateQR(textBox1.Text, 239, 239);
                stopwatch.Stop();
                label2.Text = "Oluşturma süresi: " + stopwatch.Elapsed;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (!string.IsNullOrEmpty(openFileDialog1.FileName) && System.IO.File.Exists(openFileDialog1.FileName))
            {
                stopwatch.Restart();
                textBox3.Text = openFileDialog1.FileName;
                Bitmap cQR = (Bitmap)Bitmap.FromFile(openFileDialog1.FileName);
                if (checkBox2.Checked)
                {
                    pictureBox8.Image = cQR;
                    var QRs = cQRUtil.c2QR(cQR);
                    pictureBox5.Image = QRs[0];
                    pictureBox6.Image = QRs[1];
                    pictureBox7.Image = QRs[2];
                    textBox2.Text = cQRUtil.ReadcQR(cQR);
                }
                else
                {
                    pictureBox8.Image = cQR;
                    pictureBox5.Image = cQR;
                    pictureBox6.Image = cQR;
                    pictureBox7.Image = cQR;
                    textBox2.Text = cQRUtil.ReadQR(cQR);
                }
                stopwatch.Stop();
                label2.Text = "Okuma süresi: " + stopwatch.Elapsed;
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            if (!string.IsNullOrEmpty(saveFileDialog1.FileName))
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                if (checkBox1.Checked)
                {
                    var cQRs = cQRUtil.CreateAll(textBox1.Text, 239, 239);
                    pictureBox4.Image = cQRs[0];
                    pictureBox3.Image = cQRs[1];
                    pictureBox2.Image = cQRs[2];
                    pictureBox1.Image = cQRs[3];
                }
                else
                    pictureBox1.Image = cQRUtil.CreateQR(textBox1.Text, 239, 239);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Device.IsRunning)
            {
                Device.Stop();
            }
        }
    }
}
