using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace easyUpdater.Interfaces
{
    public interface IEUpdatable
    {
        /// <summary>
        ///     The name of your application as you want it displayed on the update form
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        ///     An identifier string to use to identify your application in the update file.
        ///     Should be the same as your appId in the update file.
        /// </summary>
        string ApplicationId { get; }


        /// <summary>
        ///     The current assembly
        /// </summary>
        Assembly ApplicationAssembly { get; }

        /// <summary>
        ///     The application's icon to be displayed in the top left
        /// </summary>
        Icon ApplicationIcon { get; }

        /// <summary>
        ///     The location of the update file on a server
        /// </summary>
        Uri UpdateFileLocation { get; }

        /// <summary>
        ///     The context of the program.
        ///     For Windows Forms Applications, use 'this'
        ///     Console Apps, reference System.Windows.Forms and return null.
        /// </summary>
        Form Context { get; }
    }
}