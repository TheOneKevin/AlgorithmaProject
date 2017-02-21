// Yet more wrapper

using System.Windows.Forms;

namespace SVGtoTuring
{
    public partial class Code : Form
    {
        public Code(string text)
        {
            InitializeComponent();
            textBox1.Text = text;
        }
    }
}
