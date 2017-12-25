using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using easyUpdater.Core.Management;
using easyUpdater.Interfaces;
using easyUpdater.Common;
using easyUpdater.Models;
using Newtonsoft.Json;

namespace easyUpdater.Core.Handling
{
    public class ParseHandler
    {
        private UpdateManager _manager;

        public ParseHandler(UpdateManager manager)
        {
            _manager = manager;
        }

        private static IParseInfo _parseInfo;


        /// <summary>
        ///     Checks the Uri to make sure file exist
        /// </summary>
        /// <param name="location">The Uri of the update.xml</param>
        /// <returns>If the file exists</returns>
        public static bool ExistsOnServer(Uri location)
        {
            try
            {
                // Request the update.xml
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(location.AbsoluteUri);

                // Read for response
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                resp.Close();

                return resp.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Parses the update file into ParseInfoObject object
        /// </summary>
        /// <param name="location">Uri of update file on server (ex. update.xml)</param>
        /// <param name="appId">The application's ID</param>
        /// <param name="parseType">Type of document to parse</param>
        /// <returns>The SharpUpdateXml object with the data, or null of any errors</returns>
        internal static async Task<IParseInfo> ParseAsync(Uri location, string appId)
        {
            IParseInfo parseInfo = null;
            var parseType = GetDocType(location.AbsoluteUri);

            switch (parseType)
            {
                case ParseType.Xml:
                    parseInfo = await ParseXml(location, appId);
                    break;
                case ParseType.Json:
                    parseInfo = await ParseJson(location, appId);
                    break;
            }

            _parseInfo = parseInfo;

            return parseInfo;
        }

        private static async Task<IParseInfo> ParseJson(Uri location, string appId)
        {
            try
            {
                // Get json object
                string doc = await GetInfoDoc(location.AbsoluteUri);
                ParseInfoObject parseInfo = null;

                await Task.Run(() =>
                {
                    var updateJson = JsonConvert.DeserializeObject<RootObject>(doc);
                    var appUpdate = updateJson.eUpdate.update.FirstOrDefault(x => x.appID == appId);

                    if (appUpdate == null)
                    {
                        return;
                    }

                    // Parse data
                    // If Version is null, then we can't compare assembly versions for update
                    string versionString = appUpdate.version;
                    if (string.IsNullOrWhiteSpace(versionString))
                    {
                        return;
                    }

                    Version version = Version.Parse(versionString);

                    var fileList = new List<IParsedFile>();
                    foreach (var file in appUpdate.files.file)
                    {
                        ParsedFile parsedFile = new ParsedFile
                        {
                            FileName = file.fileName,
                            Md5 = file.md5,
                            Url = new Uri(file.url)
                        };

                        fileList.Add(parsedFile);
                    }

                    string description = appUpdate.description;

                    parseInfo = new ParseInfoObject(version, fileList, description, null);
                });

                return parseInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<IParseInfo> ParseXml(Uri location, string appId)
        {
            try
            {
                ParseInfoObject parseInfo = null;
                await Task.Run(() =>
                {
                    // Load the document
                    XmlDocument doc = new XmlDocument();
                    doc.Load(location.AbsoluteUri);

                    // Gets the appId's node with the update info
                    // This allows you to store all program's update nodes in one file
                    XmlNode updateNode = doc.DocumentElement?.SelectSingleNode("//update[@appID='" + appId + "']");

                    // If the node doesn't exist, there is no update
                    if (updateNode == null)
                    {
                        return;
                    }

                    // Parse data
                    // If Version is null, then we can't compare assembly versions for update
                    string versionString = updateNode["version"]?.InnerText;
                    if (string.IsNullOrWhiteSpace(versionString))
                    {
                        return;
                    }

                    Version version = Version.Parse(versionString);

                    var files = updateNode["files"]?.ChildNodes;
                    if (files == null)
                    {
                        return;
                    }

                    var fileList = new List<IParsedFile>();

                    foreach (XmlNode file in files)
                    {
                        Uri uri = null;
                        string url = file["url"]?.InnerText;

                        if (!string.IsNullOrEmpty(url))
                        {
                            uri = new Uri(url);
                        }

                        ParsedFile parsedFile = new ParsedFile
                        {
                            FileName = file["fileName"]?.InnerText,
                            Md5 = file["md5"]?.InnerText,
                            Url = uri
                        };

                        fileList.Add(parsedFile);
                    }

                    string md5 = updateNode["md5"]?.InnerText;
                    string description = updateNode["description"]?.InnerText;
                    string launchArgs = updateNode["launchArgs"]?.InnerText;

                    parseInfo = new ParseInfoObject(version, fileList, description, launchArgs);
                });

                return parseInfo;
            }
            catch (Exception ex)
            {
                // Log or display
                return null;
            }
        }

        private static async Task<string> GetInfoDoc(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            try
            {
                using (var client = new WebClient())
                {
                    var doc = await client.DownloadStringTaskAsync(url);

                    return doc;
                }
            }
            catch (Exception ex)
            {
                // Log & display
                return null;
            }
        }

        private static ParseType GetDocType(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Url cannot be null!");
            }

            var ext = Path.GetExtension(url);

            switch (ext.ToLower())
            {
                case ".xml":
                    return ParseType.Xml;
                case ".json":
                    return ParseType.Json;
            }

            return ParseType.Xml;
        }

        #region Async Methods

        public static async Task<bool> ExistsOnServerAsync(Uri location)
        {
            bool result = false;

            await Task.Run(() => { result = ExistsOnServer(location); });

            return result;
        }

        #endregion
    }
}
