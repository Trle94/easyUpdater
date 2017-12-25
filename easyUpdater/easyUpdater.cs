using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using easyUpdater.Interfaces;

namespace easyUpdater
{
    class easyUpdater
    {
        /// <summary>
        ///     Holds the program-to-update's info
        /// </summary>
        private IEUpdatable applicationInfo;

        /// <summary>
        ///     Thread to find update
        /// </summary>
        private BackgroundWorker bgWorker;

        /// <summary>
        ///     Creates a new SharpUpdater object
        /// </summary>
        /// <param name="applicationName">The name of the application so it can be displayed on dialog boxes to user</param>
        /// <param name="appId">A unique Id for the application, same as in update xml</param>
        /// <param name="updateXmlLocation">The Uri for the program's update.xml</param>
        /// <param name="applicationInfo">IEUpdatable object</param>
        public easyUpdater(IEUpdatable applicationInfo)
        {
            this.applicationInfo = applicationInfo;

            // Set up backgroundworker
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
        }

        /// <summary>
        ///     Checks for an update for the program passed.
        ///     If there is an update, a dialog asking to download will appear
        /// </summary>
        public void DoUpdate()
        {
            if (!bgWorker.IsBusy)
                bgWorker.RunWorkerAsync(applicationInfo);
        }

        /// <summary>
        ///     Checks for/parses update.xml on server
        /// </summary>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            IEUpdatable application = (IEUpdatable)e.Argument;

            // Check for update on server
            if (!easyUpdateXML.ExistsOnServer(application.UpdateFileLocation))
                e.Cancel = true;
            else // Parse update xml
                e.Result = easyUpdateXML.Parse(application.UpdateFileLocation, application.ApplicationId);
        }

        /// <summary>
        ///     After the background worker is done, prompt to update if there is one
        /// </summary>
        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // If there is a file on the server
            if (!e.Cancelled)
            {
                easyUpdateXML update = (easyUpdateXML)e.Result;

                // Check if the update is not null and is a newer version than the current application
                if (update != null && update.IsNewerThan(applicationInfo.ApplicationAssembly.GetName().Version))
                {
                    // Ask to accept the update
                    //  if (new MainForm(applicationInfo, update).ShowDialog(applicationInfo.Context) == DialogResult.Yes)
                    //     DownloadUpdate(update); // Do the update
                }
                else
                    MessageBox.Show("You have the latest version already!");
            }
            else
                MessageBox.Show("No update information found!");
        }

        /// <summary>
        ///     Downloads update and installs the update
        /// </summary>
        /// <param name="update">The update xml info</param>
        private void DownloadUpdate(easyUpdateXML update)
        {
            //DownloadForm form = new DownloadForm(update.Uri, update.MD5, applicationInfo.ApplicationIcon);
            //DialogResult result = form.ShowDialog(applicationInfo.Context);

            //// Download update
            //if (result == DialogResult.OK)
            //{
            //    string currentPath = applicationInfo.ApplicationAssembly.Location;
            //    string newPath = Path.GetDirectoryName(currentPath) + "\\" + update.FileName;

            //    // "Install" it
            //    UpdateApplication(form.TempFilePath, currentPath, newPath, update.LaunchArgs);

            //    Application.Exit();
            //}
            //else if (result == DialogResult.Abort)
            //{
            //    MessageBox.Show("The update download was cancelled.\nThis program has not been modified.",
            //        "Update Download Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //else
            //{
            //    MessageBox.Show("There was a problem downloading the update.\nPlease try again later.",
            //        "Update Download Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        /// <summary>
        ///     Hack to close program, delete original, move the new one to that location
        /// </summary>
        /// <param name="tempFilePath">The temporary file's path</param>
        /// <param name="currentPath">The path of the current application</param>
        /// <param name="newPath">The new path for the new file</param>
        /// <param name="launchArgs">The launch arguments</param>
        private void UpdateApplication(string tempFilePath, string currentPath, string newPath, string launchArgs)
        {
            string argument =
                "/C choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" & choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" & Start \"\" /D \"{3}\" \"{4}\" {5}";

            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = string.Format(argument, currentPath, tempFilePath, newPath, Path.GetDirectoryName(newPath),
                Path.GetFileName(newPath), launchArgs);
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Process.Start(Info);
        }
    }
}
