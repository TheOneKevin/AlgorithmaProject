// Wrapper

using Svg;
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace SVGtoTuring
{
    public partial class MainFrm : Form
    {
        #region Contructors

        public MainFrm()
        {
            InitializeComponent();
        }

        #endregion

        #region User Control Events

        private void button4_Click(object sender, EventArgs e)
        {
            AboutBox1 box = new AboutBox1();
            box.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Scalable Vectors (*.svg)|*.svg";
            fd.FilterIndex = 1;
            fd.Multiselect = false;
            fd.Title = "Choose your scalable vector!";
            fd.ShowDialog();
            textBox1.Text = fd.FileName;
            fd.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text))
            {
                Bitmap b = GetBitmapFromSVG(textBox1.Text);
                pictureBox1.Image = b;
            }

            else
                MessageBox.Show("Please choose an image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text))
            {
                Code c = new Code(ParseSVG.toTuring(GetSvgDocument(textBox1.Text)));
                c.Show();
            }
            else
                MessageBox.Show("Please choose a vector/Vector does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Converts an SVG file to a Bitmap image.
        /// </summary>
        /// <param name="filePath">The full path of the SVG image.</param>
        /// <returns>Returns the converted Bitmap image.</returns>
        private Bitmap GetBitmapFromSVG(string filePath)
        {
            SvgDocument document = GetSvgDocument(filePath);

            Bitmap bmp = document.Draw();
            return bmp;
        }

        /// <summary>
        /// Gets a SvgDocument for manipulation using the path provided.
        /// </summary>
        /// <param name="filePath">The path of the Bitmap image.</param>
        /// <returns>Returns the SVG Document.</returns>
        private SvgDocument GetSvgDocument(string filePath)
        {
            SvgDocument document = SvgDocument.Open(filePath);
            return document;
        }

        /// <summary>
        /// Makes sure that the image does not exceed the maximum size, while preserving aspect ratio.
        /// </summary>
        /// <param name="document">The SVG document to resize.</param>
        /// <returns>Returns a resized or the original document depending on the document.</returns>
        private SvgDocument AdjustSize(SvgDocument document)
        {
            if (document.Height > MaximumSize.Height)
            {
                document.Width = (int)((document.Width / (double)document.Height) * MaximumSize.Height);
                document.Height = MaximumSize.Height;
            }
            return document;
        }

        #endregion

        private void MainFrm_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

            EULA eu = new EULA();
            DialogResult dr = eu.ShowDialog();
            if(dr == DialogResult.OK)
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
            }
        }
    }
}
