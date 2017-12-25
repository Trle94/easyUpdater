using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Example
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.Text = ApplicationAssembly.GetName().Version.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        #region Conf Update

        public string ApplicationName
        {
            get { return "Example"; }
        }

        public string ApplicationId
        {
            get { return "Example"; }
        }

        public Assembly ApplicationAssembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        public Icon ApplicationIcon
        {
            get { return Icon; }
        }

        public Uri UpdateFileLocation
        {
            get { return new Uri("http://192.168.0.1/data.json"); }
        }

        public Form Context
        {
            get { return this; }
        }

        #endregion
    }
}
