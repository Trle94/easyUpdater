using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyUpdater.Common
{
    /// <summary>
    ///     An object to provide the result of a method or action
    /// </summary>
    public class MethodResult
    {
        public MethodResult(bool success = false, string message = null, bool error = false)
        {
            Success = success;
            Message = message;
            Error = error;
        }

        public bool Success { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    ///     An object to provide the result of a method or action
    /// </summary>
    /// <typeparam name="T">Type of data included in the method result</typeparam>
    public class MethodResult<T> : MethodResult
    {
        public MethodResult()
        {
        }

        public MethodResult(T data, bool success = false, string message = null, bool error = false)
        {
            Data = data;
            Success = success;
            Message = message;
            Error = error;
        }

        public T Data { get; set; }
    }
}
