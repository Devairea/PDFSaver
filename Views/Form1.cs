using PDFSaver.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFSaver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            PDFManager manager = new PDFManager();
            //MessageBox.Show(manager.SaveWebsite("https://en.wikipedia.org/wiki/Turtle", "TutlePDF", "D:\\").ToString());
            MessageBox.Show(manager.SaveWebsite("https://en.wikipedia.org/wiki/Turtle").ToString());

            manager.OpenPDF(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\"+ "Turtle - Wikipedia.pdf");
        }
    }
}
