using System;
using System.Windows.Forms;
using easyUpdater.Interfaces;

namespace easyUpdater.UI
{
    internal partial class MainForm : Form
    {
        /// <summary>
        ///     The program to update's info
        /// </summary>
        private IEUpdatable _applicationInfo;

        /// <summary>
        ///     The update info from the update.xml
        /// </summary>
        private IParseInfo _updateInfo;

        /// <summary>
        ///     The update info display form
        /// </summary>
        /// <summary>
        ///     Creates a new SharpUpdateAcceptForm
        /// </summary>
        /// <param name="applicationInfo"></param>
        /// <param name="updateInfo"></param>
        public MainForm(IEUpdatable applicationInfo, IParseInfo updateInfo)
        {
            InitializeComponent();

            // Sets the icon if it's not null
            if (applicationInfo.ApplicationIcon != null)
                Icon = applicationInfo.ApplicationIcon;

            // Fill in the UI
            Text = "Check for " + applicationInfo.ApplicationName + " updates...";
            label2.Text = string.Format("A new version is available:\nOld version: {0}\nNew version: {1}",
                applicationInfo.ApplicationAssembly.GetName().Version,
                updateInfo.Version);
            txtDescription.Text = updateInfo.Description;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}