using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Website.Service.Infrastructure
{
    public class OperationResult
    {
        public OperationResult(bool succeeded, string message, string prop)
        {
            Succeeded = succeeded;
            Message = message;
            Property = prop;
        }
        public bool Succeeded { get; private set; }
        public string Message { get; private set; }
        public string Property { get; private set; }

        public static OperationResult Success(string message = null, string property = null) => new OperationResult(true, message, property);
        public static OperationResult Failure(string message, string property = null) => new OperationResult(false, message, property);
    }
}
