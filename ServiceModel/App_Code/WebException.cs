using System;
using System.Net;

namespace Definitif.ServiceModel
{
    /// <summary>
    /// Represents service WebException.
    /// </summary>
    public class WebException : Exception
    {
        private HttpStatusCode status = HttpStatusCode.NotImplemented;
        private string message;

        /// <summary>
        /// Gets HTTP status.
        /// </summary>
        public HttpStatusCode Status
        {
            get { return this.status; }
        }
        /// <summary>
        /// Gets user-friendly error message.
        /// </summary>
        public override string Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Creates new instance of service WebException.
        /// </summary>
        /// <param name="status">HTTP status code to use.</param>
        /// <param name="message">User-friendly message.</param>
        public WebException(HttpStatusCode status, string message)
        {
            this.status = status;
            this.message = message;
        }
    }
}
