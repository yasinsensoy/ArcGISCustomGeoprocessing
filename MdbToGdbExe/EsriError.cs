///*********************************************************************************
///
/// The MIT License (MIT)
///
/// Copyright (c) 2015 Sander Struijk
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
///
///*********************************************************************************

namespace MdbToGdb
{
    /// <summary>
    ///     Esri error.
    /// </summary>
    /// <remarks>
    ///     Sander Struijk, 02.08.2013.
    /// </remarks>
    public class EsriError
    {
        /// <summary>
        ///     Gets or sets errorEnum for the.
        /// </summary>
        /// <value>
        ///     The errorEnum.
        /// </value>
        public string ErrorEnum { get; set; }

        /// <summary>
        ///     Gets or sets the error code.
        /// </summary>
        /// <value>
        ///     The error code.
        /// </value>
        public int ErrorCode { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>
        ///     The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <remarks>
        ///     Sander Struijk, 05.08.2013.
        /// </remarks>
        /// <param name="errorEnum">       The errorEnum. </param>
        /// <param name="errorCode">    The error code. </param>
        /// <param name="message">      The message. </param>
        public EsriError(string errorEnum, int errorCode, string message)
        {
            ErrorEnum = errorEnum;
            ErrorCode = errorCode;
            Message = message;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <remarks>
        ///     Sander Struijk, 05.08.2013.
        /// </remarks>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", ErrorEnum, ErrorCode, Message);
        }
    }
}