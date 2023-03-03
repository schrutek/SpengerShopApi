using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Domain.Exceptions
{
    public class MiddlewareWrongConfigurationException : Exception
    {
        private readonly ILogger _logger;

        public MiddlewareWrongConfigurationException(ILogger logger, string message)
            : base(message)
        {
            _logger = logger;
            Log(message);
        }

        public MiddlewareWrongConfigurationException(ILogger logger, string message, Exception innerException)
            : base(message,innerException)
        {
            _logger = logger;
            Log(message, innerException);
        }

        private void Log(string? message = null, Exception? innerException = null)
        {
            // Log mit _logger
            _logger.LogInformation("MiddlewareWrongConfigurationException was thrown!");
        }
    }
}
