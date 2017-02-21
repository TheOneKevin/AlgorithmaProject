using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SVGtoTuring
{
    public partial class EULA : Form
    {
        public EULA()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveSetting("SVGtoTuring", "EULA", "true");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void EULA_Load(object sender, EventArgs e)
        {
            if(GetSetting("SVGtoTuring", "EULA", "false").ToString() == "true")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            
            else
            {
                SaveSetting("SVGtoTuring", "EULA", "false");
                richTextBox1.LoadFile("EULA.rtf");
            }
        }

        // Save a value.
        public static void SaveSetting(string app_name, string name,
            object value)
        {
            RegistryKey reg_key =
                Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey(app_name);
            sub_key.SetValue(name, value);
        }

        // Get a value.
        public static object GetSetting(string app_name, string name,
            object default_value)
        {
            RegistryKey reg_key =
                Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey(app_name);
            return sub_key.GetValue(name, default_value);
        }
    }
}
