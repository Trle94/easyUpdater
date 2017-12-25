using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using easyUpdater.Interfaces;

namespace easyUpdater.UI
{
    internal partial class DownloadForm : Form
    {
        private readonly IParseInfo _updateInfo;

        private int _filesDownloaded;

        /// <summary>
        ///     Creates a new DownloadForm
        /// </summary>
        /// <summary>
        ///     Creates a new DownloadForm
        /// </summary>
        internal DownloadForm(IParseInfo updateInfo, Icon programIcon)
        {
            InitializeComponent();

            if (programIcon != null)
            {
                Icon = programIcon;
            }


            _updateInfo = updateInfo;


            // Download files
            DownloadFiles();
        }

        public IParseInfo UpdateInfo
        {
            get { return _updateInfo; }
        }

        // Don't need?
        /// <summary>
        ///     Formats the byte count to closest byte type
        /// </summary>
        /// <param name="bytes">The amount of bytes</param>
        /// <param name="decimalPlaces">How many decimal places to show</param>
        /// <param name="showByteType">Add the byte type on the end of the string</param>
        /// <returns>The bytes formatted as specified</returns>
        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "B";

            // Check if best size in KB
            if (newBytes > 1024 && newBytes < 1048576)
            {
                newBytes /= 1024;
                byteType = "KB";
            }
            else if (newBytes > 1048576 && newBytes < 1073741824)
            {
                // Check if best size in MB
                newBytes /= 1048576;
                byteType = "MB";
            }
            else
            {
                // Best size in GB
                newBytes /= 1073741824;
                byteType = "GB";
            }

            // Show decimals
            if (decimalPlaces > 0)
                formatString += ":0.";

            // Add decimals
            for (int i = 0; i < decimalPlaces; i++)
                formatString += "0";

            // Close placeholder
            formatString += "}";

            // Add byte type
            if (showByteType)
                formatString += byteType;

            return string.Format(formatString, newBytes);
        }

        private async Task<bool> CheckHash(string file, string md5)
        {
            bool result = false;

            await Task.Run(() =>
            {
                // Hash the file and compare to the hash in the update
                result = Hasher.HashFile(file, HashType.MD5).ToUpper() == md5.ToUpper();
            });

            return result;
        }


        private async void DownloadFiles()
        {
            bool success = true;

            foreach (var file in _updateInfo.Files)
            {
                // Create Progress object to relay progress
                var progress = new Progress<int>();
                progress.ProgressChanged += ProgressOnProgressChanged;

                // reset ProgressBar & labels
                progressBar.Value = 0;
                lblProgress.Text = "Downloading Update...";
                progressBar.Style = ProgressBarStyle.Continuous;

                // Show how many files have been downloaded
                lblProgress.Text = string.Format("Downloaded {0} of {1}", _filesDownloaded, _updateInfo.Files.Count);

                // Create a tempFileName and download the file
                file.TempPath = Path.GetTempFileName();
                bool downloadResult = await DownloadFile(file.Url.ToString(), file.TempPath, progress);

                if (!downloadResult)
                {
                    // Download failed
                    // Log? Show to user?
                    success = false;
                    break;
                }

                // Increment total downloaded
                _filesDownloaded++;

                // Notify that file is being verified
                lblProgress.Text = "Verifying Download...";
                progressBar.Style = ProgressBarStyle.Marquee;

                // check hash of downloaded file
                bool hashResult = await CheckHash(file.TempPath, file.Md5);

                // If Hash is incorrect, ask user if they want to continue or not (not recommended)
                if (!hashResult)
                {
                    DialogResult dialogResult =
                        MessageBox.Show(
                            "A file's hash sum did not match what was expected. Do you want to proceed? (NOT RECOMMENDED)",
                            "Do you want to proceed?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        success = false;
                        break;
                    }
                }
            }

            //MessageBox.Show("Success: " + success.ToString());

            // Done
            DialogResult = !success ? DialogResult.Abort : DialogResult.OK;
            Close();
        }


        private async Task<bool> DownloadFile(string url, string path, IProgress<int> progress = null)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) => { progress?.Report(e.ProgressPercentage); };

                    await client.DownloadFileTaskAsync(url, path);

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log?
                return false;
            }
        }

        private void ProgressOnProgressChanged(object sender, int i)
        {
            progressBar.Value = i;
        }
    }
}
