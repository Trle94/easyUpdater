using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using easyUpdater.Common;
using easyUpdater.Core.Management;
using easyUpdater.Interfaces;
using easyUpdater.UI;

namespace easyUpdater.Core.Handling
{
    public class UpdateHandler
    {
        /// <summary>
        ///     Holds the program-to-update's info
        /// </summary>
        private IEUpdatable _applicationInfo;

        private UpdateManager _manager;

        public UpdateHandler(UpdateManager manager, IEUpdatable appliationInfo = null)
        {
            _manager = manager;
            _applicationInfo = appliationInfo;
        }

        public async Task<MethodResult<IParseInfo>> CheckForUpdate()
        {
            var result = new MethodResult<IParseInfo>();

            // Check if exists
            var exists = await ParseHandler.ExistsOnServerAsync(_applicationInfo.UpdateFileLocation);

            if (!exists)
            {
                result.Success = false;
                result.Error = false;
                result.Message = "No update information was found!";

                return result;
            }

            // Get the parse information
            var parseInfo =
                await ParseHandler.ParseAsync(_applicationInfo.UpdateFileLocation, _applicationInfo.ApplicationName);

            if (parseInfo == null)
            {
                result.Success = false;
                result.Error = true;
                result.Message = "You have the latest version!";

                return result;
            }

            result.Success = true;
            result.Error = false;
            result.Message = "Successfully obtained update info!";
            result.Data = parseInfo;

            // Check if version is newer than application's version
            if (!parseInfo.Version.IsNewerThan(_applicationInfo.ApplicationAssembly.GetName().Version))
            {
                // Not newer, so return result
                result.Message = "You have the latest version!";
                return result;
            }

            // Newer than application version, so launch MainForm (ask about update)
            if (AskAboutUpdate(parseInfo)) DownloadUpdate(parseInfo);

            return result;
        }

        public void SetApplicationInfo(IEUpdatable iEUpdatable)
        {
            _applicationInfo = iEUpdatable;
        }

        private bool AskAboutUpdate(IParseInfo updateInfo)
        {
            return new MainForm(_applicationInfo, updateInfo).ShowDialog(_applicationInfo.Context) == DialogResult.Yes;
        }

        private void DownloadUpdate(IParseInfo updateInfo)
        {
            // Show DownloadForm
            var form = new DownloadForm(updateInfo, _applicationInfo.ApplicationIcon);
            var result = form.ShowDialog(_applicationInfo.Context);

            // Download update
            if (result == DialogResult.OK)
            {
                var updateFolderName = string.Format("{0}_update_{1}", _applicationInfo.ApplicationName,
                    Utilities.CleanFileName(updateInfo.Version.ToString()));

                var currentPath = _applicationInfo.ApplicationAssembly.Location;
                var newFolderPath = Path.GetDirectoryName(currentPath);
                // string newPath = Path.GetDirectoryName(currentPath) + "\\" + updateFolderName;

                // Update
                UpdateApplication(updateInfo.Files, currentPath, newFolderPath, updateInfo.LaunchArgs);

                Application.Exit();
            }
            else if (result == DialogResult.Abort)
            {
                MessageBox.Show("The update download was cancelled.\nThis program has not been modified.",
                    "Update Download Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("There was a problem downloading the update.\nPlease try again later.",
                    "Update Download Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        ///     Hack to close program, delete original, move the new one to that location
        /// </summary>
        /// <param name="files">Collection of update files</param>
        /// <param name="currentPath">The path of the current application</param>
        /// <param name="newFolderPath">The new path for the new file</param>
        /// <param name="launchArgs">The launch arguments</param>
        private void UpdateApplication(IList<IParsedFile> files, string currentPath, string newFolderPath,
            string launchArgs)
        {
            // string deleteArg = string.Format("/C choice /C Y /N /D Y /T 4 & Del /F /Q {0}", currentPath);

            var deleteArg = "";

            // create delete arg
            foreach (var file in files)
            {
                // Delete old files
                // /C choice /C Y /N /D Y /T 4 & Del /F /Q \"{0}\" ///// timeout 4 seconds then removes old file

                // Call for each update file
                // & choice /C Y /N /D Y /T 2 & Move /Y \"{1}\" \"{2}\" ///// Move update application into folder timeout 2 seconds after remove command is called

                // Executes program
                // & Start \"\" /D \"{3}\" \"{4}\" {5} ///// Executes file, should be called only on .exe i guess

                // Create delete args
                var delete = string.Format("/C choice /C Y /N /D Y /T 1 & Del /F /Q \"{0}\\{1}\" & ", newFolderPath,
                    file.FileName);

                deleteArg += delete;
            }

            var moveArg = string.Empty;

            // create moveFile arg
            foreach (var file in files)
            {
                var newPath = string.Format("{0}\\{1}", newFolderPath, file.FileName);

                var moveFileArg = string.Format("choice /C Y /N /D Y /T 2 & Move /Y \"{0}\" \"{1}\" & ", file.TempPath,
                    newPath);

                moveArg += moveFileArg;
            }

            var executeArg = string.Format("Start \"\" /D \"{0}\" \"{1}\" {2}",
                Path.GetDirectoryName(_applicationInfo.ApplicationAssembly.Location),
                _applicationInfo.ApplicationAssembly.Location, launchArgs);

            var arg = deleteArg + moveArg + executeArg;
            var info = new ProcessStartInfo
            {
                Arguments = arg,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };

            Process.Start(info);
        }
    }
}