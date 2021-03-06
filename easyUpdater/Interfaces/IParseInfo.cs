﻿using System;
using System.Collections.Generic;

namespace easyUpdater.Interfaces
{
    public interface IParseInfo
    {
        /// <summary>
        ///     The update version #
        /// </summary>
        Version Version { get; }

        IList<IParsedFile> Files { get; }

        /// <summary>
        ///     The update's description
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     The arguments to pass to the updated application on startup
        /// </summary>
        string LaunchArgs { get; }
    }
}