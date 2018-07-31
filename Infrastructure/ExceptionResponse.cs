using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure
{
    public class ExceptionResponse
    {
        public string Message { get; set; }
        public string TranslateKey { get; set; }

        public ExceptionResponse(string message)
        {
            Message = message;
        }
        public ExceptionResponse(string message , string translateKey)
        {
            Message = message;
            TranslateKey = translateKey;
        }
    }
}
