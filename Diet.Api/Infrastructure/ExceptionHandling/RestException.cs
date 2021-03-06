﻿using System;
using System.Net;

namespace Diet.Api.Infrastructure.ExceptionHandling
{
    /// <summary>
    /// A machine-readable format for specifying errors in HTTP API responses based on https://tools.ietf.org/html/rfc7807.
    /// </summary>
    public class RestException : Exception
    {
        public RestException(
            HttpStatusCode httpStatus = HttpStatusCode.InternalServerError, 
            string title = null, 
            string detail = null, 
            string type = null, 
            string instance = null)
        {
            HttpStatus = httpStatus;
            Title = title;
            Detail = detail;
            Type = type;
            Instance = instance;
        }

        /// <summary>
        /// A URI reference [RFC3986] that identifies the problem type.
        /// Human-readable documentation for the problem type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A short, human-readable summary for the problem type.
        /// It SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization
        /// (e.g., using proactive content negotiation; see[RFC7231], Section 3.4)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The HTTP status code([RFC7231], Section 6) generated by the origin server for the problem.
        /// </summary>
        public HttpStatusCode HttpStatus { get; set; }

        /// <summary>
        /// A human-readable explanation specific to the problem.
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// A URI reference that identifies the specific occurrence of the problem.
        /// It may or may not yield further information.
        /// </summary>
        public string Instance { get; set; }
    }
}