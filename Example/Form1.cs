using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using easyUpdater.Core.Management;
using easyUpdater.Interfaces;


namespace Example
{
    public partial class Form1 : Form, IEUpdatable
    {
        // private easyUpdater updater;
        private UpdateManager _updateManager;
        public Form1()
        {
            InitializeComponent();
            label1.Text = ApplicationAssembly.GetName().Version.ToString();
            _updateManager = new UpdateManager(this);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await _updateManager.Updater.CheckForUpdate();
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
