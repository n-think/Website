using System;
using System.Collections.Generic;
using System.Text;

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

        public static OperationResult Success() => new OperationResult(true, null, null);
    }
}
