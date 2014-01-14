using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace N2Hasher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = ComputeSHA1Hash(textBox1.Text);
        }

        public string ComputeSHA1Hash(string input)
        {
            var encrypter = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            using (var sw = new StringWriter())
            {
                foreach (byte b in encrypter.ComputeHash(Encoding.UTF8.GetBytes(input)))
                    sw.Write(b.ToString("x2"));
                return sw.ToString();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.UseSystemPasswordChar = checkBox1.Checked;
        }
    }
}
