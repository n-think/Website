using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Website.Service.Infrastructure
{
    public class OperationResult
    {
        private List<OperationError> _errors = new List<OperationError>();

        public bool Succeeded { get; private set; }
        public IEnumerable<OperationError> Errors => _errors;

        public static OperationResult Success() => new OperationResult() {Succeeded = true};

        public static OperationResult Failure(params OperationError[] errors)
        {
            var result = new OperationResult() {Succeeded = false};
            if (errors != null)
            {
                result._errors.AddRange(errors);
            }
            return result;
        }

        public override string ToString()
        {
            return Succeeded ?
                "Succeeded" :
                string.Format("{0} : {1}", "Failed", string.Join(",", Errors.Select(x => x.Code).ToList()));
        }
    }
}
